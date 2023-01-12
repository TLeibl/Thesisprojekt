using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SupervisorUIManager : MonoBehaviour
{
    //UI components
    [SerializeField] private Image feedbackBar = null; //the feedback display
    [SerializeField] private Button spawnButton = null;
    [SerializeField] private Button despawnButton = null;
    [SerializeField] private Button fleeButton = null;
    [SerializeField] private Button lookAtButton = null;
    [SerializeField] private Button moveToPosButton = null;
    [SerializeField] private Button moveToPatButton = null;
    [SerializeField] private Button stopSpiderButton = null;

    //spider
    private GameObject spawnedSpider = null; //the currently spawned spider
    private SpiderController spiderController = null; //current spider controller
    private float despawnDelay = 2.5f; //delay when despawning


    private void Awake()
    {
        ResetButtons(); //no actions before spider spawned
        spawnButton.interactable = false; //until spider has been instantiated
    }


    // Update is called once per frame
    private void Update()
    {
        //when spider instantiated by VR user - set object and enable functionalities
        if(spawnedSpider == null)
            if((bool)PhotonNetwork.CurrentRoom.CustomProperties["ObjectInstantiated"] == true)
            {
                //set spider GameObject
                spawnedSpider = PhotonView.Find(2001).gameObject; //spider is second PhotonView after VR user
                spiderController = spawnedSpider.GetComponent<SpiderController>();

                if(spawnedSpider != null)
                    spawnButton.interactable = true; //enable button
            }

        //update FeedbackBar value
        if(feedbackBar != null)
            feedbackBar.fillAmount = (float)PhotonNetwork.CurrentRoom.CustomProperties["FeedbackValue"];

        //if phobia object (e.g. spider) is dead - can only be despawned
        if(spawnedSpider != null)
        {
            if (spiderController.IsDead())
            {
                spawnButton.interactable = false;
                fleeButton.interactable = false;
                lookAtButton.interactable = false;
                moveToPosButton.interactable = false;
                moveToPatButton.interactable = false;
                stopSpiderButton.interactable = false;
            }
        }
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

        //restart Supervisor UI
        SceneManager.LoadScene("SupervisorUI");
    }


    //Draw button - if chosen let supervisor draw onto camera view
    //public void DrawButton()
    //{
        //Debug.Log("Start drawing...");

        //TODO
        //Knopf soll gew�hlt werden k�nnen und w�hrenddessen gehighlighted sein, statt Maus Pinsel, danach wd
        //anklicken Knopf und alles wd normal, andere Kn�pfe solange malen nicht anklickbar au�er Stop!

        //highlight button if not already pressed - else make it normal again

        //draw
        //Draw();
    //}


    //Spawn button - let the supervisor spawn the phobia object (e.g. spider)
    public void SpawnButton()
    {
        Debug.Log("Spawn object...");

        //instantiate spider and get spider controller
        //spawnedSpider = PhotonNetwork.Instantiate("Spider", new Vector3(-0.15f, 0.03f, 13.75f), Quaternion.identity, 0);
        //spiderController = spawnedSpider.GetComponent<SpiderController>();

        if(spawnedSpider != null) //if spider successfully spawned
        {
            //make Game Object visible
            spawnedSpider.SetActive(true);

            //Gray button out - can only spawn one object
            spawnButton.interactable = false;
            //enable other buttons for spider control
            despawnButton.interactable = true;
            fleeButton.interactable = true;
            lookAtButton.interactable = true;
            moveToPosButton.interactable = true;
            moveToPatButton.interactable = true;
            stopSpiderButton.interactable = true;
        } 
    }


    //Despawn button - let the supervisor instantly despawn the phobia object (e.g. spider)
    public void DespawnButton()
    {
        Debug.Log("Despawn object...");

        //destroy current spider object
        //PhotonNetwork.Destroy(spawnedSpider);

        //make Game Object invisible
        spawnedSpider.SetActive(false);

        //reactivate SpawnButton and deactivate other buttons
        ResetButtons();
    }


    //Flee button - command phobia object (e.g. spider) to flee
    public void FleeButton()
    {
        Debug.Log("Let object flee...");

        //command object to flee
        if(!spiderController.IsDead())
             spiderController.Flee();

        //despawn spider after short time
        StartCoroutine(DespawnAfterTime());

        //reactivate SpawnButton and deactivate despawn buttons
        ResetButtons();
    }


    //Coroutine used when spider flees to despawn the spider after some time
    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(despawnDelay);
        DespawnButton();
    }


    //StopSpider button - command phobia object (e.g. spider) to stop current action
    public void StopSpiderButton()
    {
        Debug.Log("Spider stop...");

        if (!spiderController.IsDead())
        {
            spiderController.Stop();
        }
    }

    //LookAtPerson button - command phobia object (e.g. spider) to turn towards the VR user
    public void LookAtPersonButton()
    {
        Debug.Log("Let object look at person...");

        //command object to look at person
        if (!spiderController.IsDead())
            spiderController.LookAtPerson();
    }


    //MoveToPosition button - command phobia object (e.g. spider) to move to a chosen position
    public void MoveToPosButton()
    {
        Debug.Log("Let object move to position...");

        //command object  to move to chosen position
        if (!spiderController.IsDead())
            spiderController.MoveToPosition(ChoosePosition());
    }


    //MoveToPerson button - command phobia object (e.g. spider) to walk towards VR user
    public void MoveToPersonButton()
    {
        Debug.Log("Let object move to person...");
        
        if (!spiderController.IsDead())
            spiderController.MoveToPatient();
    }


    //MoveOntoPerson button - command phobia object (e.g. spider) to climb onto the VR user avatar to a destined position
    public void MoveOntoPersonButton()
    {
        Debug.Log("Let object move onto person...");

        //TODO MoveOntoPerson Button
        //if(!spiderController.IsDead())
    }



    //--------------------Functionalities--------------------


    //method used to draw onto camera view - shall be seen by patient/scholar in scene
    //private void Draw()
    //{
        //Debug.Log("Drawing...");

        //TODO Draw functionality
    //}


    //method used to let supervisor choose a position in the camera view 
    private Vector3 ChoosePosition()
    {
        Debug.Log("Choose position...");

        Vector3 position = new Vector3();

        //TODO

        return position;
    }


    //reset buttons after spawned spider object has been destroyed
    private void ResetButtons()
    {
        //reactivate SpawnButton and deactivate other buttons
        spawnButton.interactable = true;
        despawnButton.interactable = false;
        fleeButton.interactable = false;
        lookAtButton.interactable = false;
        moveToPosButton.interactable = false;
        moveToPatButton.interactable = false;
        stopSpiderButton.interactable = false;
    }

}
