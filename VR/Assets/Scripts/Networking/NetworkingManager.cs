using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;


//script used by ConnectionManagers to let user create or join a lobby and keep shared variables synchronized
//e.g. feedback value and currently chosen scenario
public class NetworkingManager : MonoBehaviourPunCallbacks
{
    //input fields
    [SerializeField] private TMP_InputField createInput = null; //create lobby text input field
    [SerializeField] private TMP_InputField joinInput = null; //join lobby text input field

    private bool joined = false; //scholar/patient has joined - true


    //all possible scenarios that can be chosen by the supervisor
    public enum Scenario
    {
        notChosenYet,
        Arachnophobia,
        MachineOperating
    }


    private void Awake()
    {
        //keep object during whole run
        DontDestroyOnLoad(this.gameObject);
    }


    //Update method to check whether a scenario has been chosen and send scholar/patient to it
    private void Update()
    {
        //patient/scholar (VR = Android user) joined a room - send to scenario when chosen
        if (joined)
        {
            //only if scholar/patient - send to scenario
            if (!PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(sendToScenario());
            }
        }
    }


    //method called by Join button
    public void JoinRoom()
    {
        //TODO ZU TESTZWECKEN! RAUS WENN TEST TASTATUR! 
        joinInput.text = "a";


        if (joinInput != null && joinInput.text != "")
            Debug.Log("Joining room...");
        //join room with name like input text
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");

        joined = true; //scholar/patient joined
        //wait until scenario has been chosen by supervisor and then sent to map
        SceneManager.LoadScene("WaitingScene");
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("JoinRoom failed!");
    }

    //coroutine until scenario has been set by supervisor
    public IEnumerator sendToScenario()
    {
        Debug.Log("Wait for supervisor to choose scenario...");
        if (PhotonNetwork.CurrentRoom != null)
        {
            while ((bool)PhotonNetwork.CurrentRoom.CustomProperties["ScenarioNotChosenYet"])
            {
                yield return null;
            }
            Debug.Log("Scenario chosen. Go to scenario...");

            joined = false; //don't join scenario twice or more

            if ((Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == Scenario.Arachnophobia)
                SceneManager.LoadScene("MapPhobia");
            else if ((Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == Scenario.MachineOperating)
                SceneManager.LoadScene("MapLearning");
        }     
    }
}