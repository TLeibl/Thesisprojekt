using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//script used by ConnectionManagers to let user create or join a lobby
public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField createInput; //create lobby text input field
    [SerializeField] private InputField joinInput; //join lobby text input field


    //method called by Create button
    public void CreateRoom()
    {
        //create and join room with input text as name
        PhotonNetwork.CreateRoom(createInput.text);
    }


    //method called by Join button
    public void JoinRoom()
    {
        //join room with name like input text
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
    }
}