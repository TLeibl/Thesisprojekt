using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

//script used by ConnectionManagers to let user create or join a lobby
public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField createInput = null; //create lobby text input field
    [SerializeField] private TMP_InputField joinInput = null; //join lobby text input field

    private Scenario chosenScenario; //the scenario the supervisor has chosen

    //all possible scenarios
    private enum Scenario
    {
        Arachnophobia,
        MachineOperating
    }


    //method called by Create button
    public void CreateRoom()
    {
        if(createInput != null)
        {
            Debug.Log("Creating and joining room...");
            //room settings so only 2 people can be in one room
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            //create and join room with input text as name
            PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        }  
    }


    //method called by Join button
    public void JoinRoom()
    {
        if (joinInput != null)
            Debug.Log("Joining room...");
            //join room with name like input text
            PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");

        //supervisor (Pc/Mac/Linux user) created a room - go to UISupervisor scene
        if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            SceneManager.LoadScene("UISupervisor");
        }
        //patient/scholar (VR = Android user) joined a room - send to map
        else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) 
        {
            if (chosenScenario == Scenario.Arachnophobia)
                SceneManager.LoadScene("MapPhobia");
            else if (chosenScenario == Scenario.MachineOperating)
                SceneManager.LoadScene("MapLearning");
        }
    }
}