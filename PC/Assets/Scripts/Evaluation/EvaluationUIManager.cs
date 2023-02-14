using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EvaluationUIManager : MonoBehaviour
{
    //Back button functionality
    public void BackButton()
    {
        //supervisor: leave room and go back to main menu 
        Debug.Log("Leave room and go back to main menu...");
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenuPc");
    }
}
