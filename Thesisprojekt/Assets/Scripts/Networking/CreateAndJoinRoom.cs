using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

//script used by ConnectionManagers to let user create or join a lobby
public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField createInput = null; //create lobby text input field
    [SerializeField] private TMP_InputField joinInput = null; //join lobby text input field


    //method called by Create button
    public void CreateRoom()
    {
        if(createInput != null)
            //create and join room with input text as name
            PhotonNetwork.CreateRoom(createInput.text);
    }


    //method called by Join button
    public void JoinRoom()
    {
        if(joinInput != null)
            //join room with name like input text
            PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");

        //supervisor created a room - go to UISupervisor scene
        if(joinInput == null)
        {
            SceneManager.LoadScene("CreateRoom");
        }
        //patient/scholar joined a room - send to map
        else if(createInput == null) 
        {
            SceneManager.LoadScene("FindRoom");
        }
    }
}