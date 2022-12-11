using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script that containts the menu components functionality
public class MenuController : MonoBehaviour
{
    //--------------------Main Menu Buttons--------------------

    public void SupervisorButtonPc()
    {
        //sends the supervisor (= person on pc) to the scene to choose a scenario
        SceneManager.LoadScene("ChooseScenario");
    }

    public void ScholarButtonVR()
    {
        //sends the scholar/patient (= person using VR) to a waiting scene until the supervisor has chosen a scenario
        SceneManager.LoadScene("WaitingScene");
    }


    //--------------------ChooseScenario Menu Buttons--------------------

    //TODO überarbeiten
    public void ArachnophobiaButton()
    {
        //send supervisor in UISupervisor scene
        SceneManager.LoadScene("UISupervisor");
        //send patient into phobia scene
        GameManager.ChosenScenario = GameManager.Scenario.Arachnophobia;
    }

    public void MachineOperatingButton()
    {
        //send supervisor in UISupervisor scene
        SceneManager.LoadScene("UISupervisor");
        //send patient into learning scene
        GameManager.ChosenScenario = GameManager.Scenario.MachineOperating;
    }
}
