using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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


    //all possible scenarios that can be chosen by the supervisor
    public enum Scenario
    {
        notChosenYet,
        Arachnophobia,
        MachineOperating
    }

    private static Scenario chosenScenario = Scenario.notChosenYet; //the scenario the supervisor has chosen

    //the in the ChooseScenario scene chosen scenario 
    public static Scenario ChosenScenario
    {
        get => chosenScenario;
        set
        {
            chosenScenario = value;
        }
    }



    private void Awake()
    {
        //keep values during whole run
        DontDestroyOnLoad(this.gameObject);
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
            SharedValues.Add("FeedbackValue", 0f);
            SharedValues.Add("ChosenScenario", NetworkingManager.Scenario.notChosenYet);
            roomOptions.CustomRoomProperties = SharedValues;

            //create and join room with input text as name
            PhotonNetwork.CreateRoom(createInput.text, roomOptions);

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

        //supervisor (Pc/Mac/Linux user) created a room - go to UISupervisor scene
        //TODO wenn so nicht funzt: mit PhotonNetwork.isMasterClient 
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            SceneManager.LoadScene("UISupervisor");
        }
        //patient/scholar (VR = Android user) joined a room - send to map
        else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) 
        {
            //TODO Warten wenn notChosenYet
            if (chosenScenario == Scenario.Arachnophobia)
                SceneManager.LoadScene("MapPhobia");
            else if (chosenScenario == Scenario.MachineOperating)
                SceneManager.LoadScene("MapLearning");
        }
    }
}