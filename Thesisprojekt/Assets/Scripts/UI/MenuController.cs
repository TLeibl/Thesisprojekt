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

    public void ArachnophobiaButton()
    {
        //send supervisor in UISupervisor scene
        SceneManager.LoadScene("UISupervisor");
        //send patient into phobia scene
        LoadVRPlayerIntoScene("MapPhobia");
    }

    public void MachineOperatingButton()
    {
        //send supervisor in UISupervisor scene
        SceneManager.LoadScene("UISupervisor");
        //send patient into learning scene
        LoadVRPlayerIntoScene("MapLearning");
    }

    //method used to spawn player/scholar into the chosen scenario scene
    private void LoadVRPlayerIntoScene(string sceneName)
    {
        //TODO nach networking: Methode nur für VR Player ausführen lassen!
        SceneManager.LoadScene(sceneName);
    }
}
