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

    public void SupervisorButtonPc()
    {
        //sends the supervisor (= person on pc) to the scene to choose a scenario
        SceneManager.LoadScene("CreateRoom");
    }

    public void ScholarButtonVR()
    {
        //sends the scholar/patient (= person using VR) to a waiting scene until the supervisor has chosen a scenario
        SceneManager.LoadScene("FindRoom");
    }

    public void QuitButton()
    {
        Application.Quit();
    }


    //--------------------ChooseScenario Menu Buttons--------------------

    public void ArachnophobiaButton()
    {
        //send supervisor in UISupervisor scene
        SceneManager.LoadScene("UISupervisor");
        //set chosen scenario for patient/scholar to be sent to
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ChosenScenario", NetworkingManager.Scenario.Arachnophobia } });
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ScenarioNotChosenYet", false } });
    }

    public void MachineOperatingButton()
    {
        //send supervisor in UISupervisor scene
        SceneManager.LoadScene("UISupervisor");
        //set chosen scenario for patient/scholar to be sent to
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ChosenScenario", NetworkingManager.Scenario.MachineOperating } });
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ScenarioNotChosenYet", false } });
    }
}
