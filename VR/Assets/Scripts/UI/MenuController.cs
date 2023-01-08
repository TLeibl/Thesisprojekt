using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


//Script that containts the menu components functionality
public class MenuController : MonoBehaviour
{
    //--------------------Main Menu Buttons--------------------

    public void ScholarButtonVR()
    {
        //sends the scholar/patient (= person using VR) to a waiting scene until the supervisor has chosen a scenario
        SceneManager.LoadScene("FindRoom");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
