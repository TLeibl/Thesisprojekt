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
    [SerializeField] private Button spawnButton = null;
    [SerializeField] private Button despawnButton = null;
    [SerializeField] private Button fleeButton = null;

    private SpiderAC spider = null; //TODO get spider AC

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
        Debug.Log("Stop scenario...");

        //send the supervisor to the scene to analyse the scenario
        SceneManager.LoadScene("ScenarioEvaluation");

        //stop application for patient
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "StoppedScenario", true } });
    }


    //Reset button - reset scenario
    public void ResetButton()
    {
        Debug.Log("Reset scenario...");

        //restart scene for patient
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ResetScenario", true } });
    }


    //Draw button - if chosen let supervisor draw onto camera view
    public void DrawButton()
    {
        Debug.Log("Start drawing...");

        //TODO
        //Knopf soll gewählt werden können und währenddessen gehighlighted sein, statt Maus Pinsel, danach wd
        //anklicken Knopf und alles wd normal, andere Knöpfe solange malen nicht anklickbar außer Stop!

        //highlight button if not already pressed - else make it normal again

        //draw
        Draw();
    }


    //Spawn button - let the supervisor spawn the phobia object (e.g. spider)
    public void SpawnButton()
    {
        Debug.Log("Spawn object...");

        //TODO

        //command object to spawn at chosen position
        spider.SpawnSpider(ChoosePosition());

        //Gray button out - can only spawn one object
        spawnButton.interactable = false;
    }


    //Despawn button - let the supervisor instantly despawn the phobia object (e.g. spider)
    public void DespawnButton()
    {
        Debug.Log("Despawn object...");

        //command object to despawn
        spider.DespawnSpider();

        //reactivate SpawnButton and deactivate despawn buttons
        spawnButton.interactable = true;
        despawnButton.interactable = false;
        fleeButton.interactable = false;
    }


    //Flee button - command phobia object (e.g. spider) to flee
    public void FleeButton()
    {
        Debug.Log("Let object flee...");

        //command object to flee
        spider.Flee();

        //reactivate SpawnButton and deactivate despawn buttons
        spawnButton.interactable = true;
        despawnButton.interactable = false;
        fleeButton.interactable = false;
    }


    //LookAtPerson button - command phobia object (e.g. spider) to turn towards the VR user
    public void LookAtPersonButton()
    {
        Debug.Log("Let object look at person...");

        //command object to look at person
        spider.LookAtPerson();
    }

    //MoveToPosition button - command phobia object (e.g. spider) to move to a chosen position
    public void MoveToPosButton()
    {
        Debug.Log("Let object move to position...");

        //command object  to move to chosen position
        spider.MoveToPosition(ChoosePosition());
    }


    //MoveToPerson button - command phobia object (e.g. spider) to walk towards VR user
    public void MoveToPersonButton()
    {
        Debug.Log("Let object move to person...");

        spider.MoveToPatient();
    }


    //MoveOntoPerson button - command phobia object (e.g. spider) to climb onto the VR user avatar to a destined position
    public void MoveOntoPersonButton()
    {
        Debug.Log("Let object move onto person...");

        //TODO MoveOntoPerson Button
    }



    //--------------------Functionalities--------------------


    //method used to draw onto camera view - shall be seen by patient/scholar in scene
    private void Draw()
    {
        Debug.Log("Drawing...");

        //TODO Draw functionality
    }


    //method used to let supervisor choose a position in the camera view 
    private Vector3 ChoosePosition()
    {
        Debug.Log("Choose position...");

        Vector3 position = new Vector3();

        //TODO

        return position;
    }

}
