using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SupervisorUIManager : MonoBehaviour
{
    [SerializeField] private Image feedbackBar = null; //the feedback display

    // Update is called once per frame
    private void Update()
    {
        //update FeedbackBar value
        feedbackBar.fillAmount = (float)PhotonNetwork.CurrentRoom.CustomProperties["FeedbackValue"];
    }

    //--------------------Arachnophobia Scenario Buttons--------------------

    //Stop button - finish scenario and go to analysis scene
    public void StopButton()
    {
        //TODO

        //sends the supervisor to the scene to analyse the scenario
        SceneManager.LoadScene("ChooseScenario");
    }


    //Reset button - reset scenario
    public void ResetButton()
    {
        //TODO
    }


    //Draw button - if chosen let supervisor draw onto camera view
    public void DrawButton()
    {
        //TODO
        //Knopf soll gewählt werden können und währenddessen gehighlighted sein, statt Maus Pinsel, danach wd
        //anklicken Knopf und alles wd normal
    }


    //Spawn button - let the supervisor spawn the phobia object (e.g. spider)
    public void SpawnButton()
    {
        //TODO
    }


    //Despawn button - let the supervisor instantly despawn the phobia object (e.g. spider)
    public void DespawnButton()
    {
        //TODO
    }


    //Flee button - command phobia object (e.g. spider) to flee
    public void FleeButton()
    {
        //TODO
    }


    //LookAtPerson button - command phobia object (e.g. spider) to turn towards the VR user
    public void LookAtPersonButton()
    {
        //TODO
    }

    //MoveToPosition button - command phobia object (e.g. spider) to move to a chosen position
    public void MoveToPosButton()
    {
        //TODO
        //choose position

        //command phobia object 

    }


    //MoveToPerson button - command phobia object (e.g. spider) to walk towards VR user
    public void MoveToPersonButton()
    {
        //TODO
    }


    //MoveOntoPerson button - command phobia object (e.g. spider) to climb onto the VR user avatar to a destined position
    public void MoveOntoPersonButton()
    {
        //TODO MoveOntoPerson Button
    }



    //--------------------Draw Functionality--------------------


    //method used to draw onto camera view - shall be seen by patient/scholar in scene
    private void Draw()
    {
        //TODO Draw functionality
    }
}
