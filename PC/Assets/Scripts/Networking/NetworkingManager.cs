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
                StartCoroutine(SendToScenario());
            }
        }
        
    }


    //method called by Create button
    public void CreateRoom()
    {
        if(createInput != null && createInput.text != "")
        {
            Debug.Log("Creating and joining room...");
            //room settings so only 2 people can be in one room
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            //create hashtable to keep shared values e.g. feedback value in
            Hashtable SharedValues = new Hashtable();
            SharedValues.Add("FeedbackValue", 0f); //shared feedback value
            SharedValues.Add("ChosenScenario", NetworkingManager.Scenario.notChosenYet); //the scenario chosen
            SharedValues.Add("ScenarioNotChosenYet", true); //false if scenario has been chosen
            SharedValues.Add("ObjectInstantiated", false); //true when spider object has been instantiated by VR user
            SharedValues.Add("StoppedScenario", false); //true when supervisor pushed Stop button
            SharedValues.Add("ResetScenario", false); //true when supervisor pushed Reset button
            roomOptions.CustomRoomProperties = SharedValues;

            //create and join room with input text as name
            PhotonNetwork.CreateRoom(createInput.text, roomOptions);
            //enable having supervisor and VR user in different scenes
            PhotonNetwork.AutomaticallySyncScene = false;

            Debug.Log("Room successfully created. Waiting for scholar/patient...");
        }  
    }


    //method called by Join button
    public void JoinRoom()
    {
        if (joinInput != null && createInput.text != "")
            Debug.Log("Joining room...");
            //join room with name like input text
            PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");

        //supervisor (Pc/Mac/Linux user) created a room - go to ChooseScenario scene
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.LoadScene("ChooseScenario");
        }
        //patient/scholar (VR = Android user) joined a room - send to waiting screen
        else
        {
            joined = true; //scholar/patient joined
            //wait until scenario has been chosen by supervisor and then sent to map
            SceneManager.LoadScene("WaitingScene");
        }
    }

    //coroutine until scenario has been set by supervisor
    public IEnumerator SendToScenario()
    {
        Debug.Log("Wait for supervisor to choose scenario...");
        while ((bool)PhotonNetwork.CurrentRoom.CustomProperties["ScenarioNotChosenYet"])
        {
            yield return null;
        }
        Debug.Log("Scenario chosen. Go to scenario...");

        if ((Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == Scenario.Arachnophobia)
            SceneManager.LoadScene("MapPhobia");
        else if ((Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == Scenario.MachineOperating)
            SceneManager.LoadScene("MapLearning");
    }
}