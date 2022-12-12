using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

//script used by ConnectionManagers to connect user to photon server
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Connecting to server...");

        //connect to server
        PhotonNetwork.ConnectUsingSettings();
    }


    //gets called automatically when connected to master to join lobby
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server, joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    //gets called automatically when joined a lobby
    public override void OnJoinedLobby()
    {
        //go to scene to find/create room
        //supervisor (Pc/Mac/Linux user) shall create a room - go to CreateRoom scene
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            SceneManager.LoadScene("CreateRoom");
        //patient/scholar (VR = Android user) shall join a room - go to FindRoom scene
        else SceneManager.LoadScene("FindRoom");
    }
}
