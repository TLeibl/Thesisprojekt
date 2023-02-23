using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SupervisorUIManager : MonoBehaviour
{
    //UI components
    [SerializeField] private Image feedbackBar = null; //the feedback display
    [SerializeField] private Texture2D mouseCrosshair; //mouse cursor texture for choosing a position in room view
    private Camera mainCamera = null;
    //Arachnophobia
    [SerializeField] private GameObject phobiaRoomViewComponents = null;
    [SerializeField] private GameObject arachnophobiaButtons = null;
    [SerializeField] private Button spawnButton = null;
    [SerializeField] private Button spawnOntoPersonButton = null;
    [SerializeField] private Button despawnButton = null;
    [SerializeField] private Button fleeButton = null;
    [SerializeField] private Button lookAtButton = null;
    [SerializeField] private Button moveToPosButton = null;
    [SerializeField] private Button moveToPatButton = null;
    [SerializeField] private Button moveOntoPatButton = null;
    [SerializeField] private Button stopSpiderButton = null;
    //Machine Operating
    [SerializeField] private GameObject learningRoomViewComponents = null;
    [SerializeField] private GameObject machineButtons;
    [SerializeField] private Button alarmButton = null;

    //spawned object (e.g. spider or machine)
    private GameObject spawnedObject = null; //the currently spawned object
    private SpiderController spiderController = null; //current spider controller
    private MachineController machineController = null; //current machine controller
    private float despawnDelay = 2.5f; //delay when despawning
    //needed variables for controlling object
    [SerializeField] private LayerMask roomViewEnvironmentWalkable;
    private PhotonView objectPV = null; //photon view to send RPCs to (ergo of the spawned object)

    private bool uiInitialized = false; //check if UI has been initialized

    //evaluation value manager to save current values 
    private EvaluationValueManager valueManager = null;


    private void Awake()
    {
        //set needed objects and own mouse cursor
        valueManager = GameObject.Find("EvaluationManager").GetComponent<EvaluationValueManager>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }


    // Update is called once per frame
    private void Update()
    {
        //try to set SupervisorUI components (buttons and cameras) 
        if (!uiInitialized)
        {
            TrySetUIComponents();
            //if initialized - disable them until spawned object set
            if(uiInitialized)
                DisableObjectRelatedButtons();
        }
        //try to set spawned object if not set yet (possible after VR user spawned it)
        if (spawnedObject == null) 
            TrySetSpawnObject();
            

        //update FeedbackBar value
        if(feedbackBar != null)
            feedbackBar.fillAmount = (float)PhotonNetwork.CurrentRoom.CustomProperties["FeedbackValue"];


        //if phobia object (e.g. spider) is dead - can only be despawned
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            if (spawnedObject != null)
            {
                if (spiderController.Dead)
                {
                    //update EvaluationValueManager value
                    valueManager.SpiderDead = true;

                    //disable buttons
                    spawnButton.interactable = false;
                    spawnOntoPersonButton.interactable = false;
                    fleeButton.interactable = false;
                    lookAtButton.interactable = false;
                    moveToPosButton.interactable = false;
                    moveToPatButton.interactable = false;
                    moveOntoPatButton.interactable = false;
                    stopSpiderButton.interactable = false;
                }

                //if spider is not onto patient anymore - update EvaluationValueManager value
                if(!spiderController.OntoPatient)
                    valueManager.SpiderOntoPatient = false;
            }
        }
    }


    //Set UI components like buttons and camera views
    private void TrySetUIComponents()
    {
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            //Buttons
            arachnophobiaButtons.SetActive(true); //show correct buttons
            DisableObjectRelatedButtons(); //no actions before spider spawned
            //Room views
            phobiaRoomViewComponents.SetActive(true); //show correct room view
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "RoomViewSet", true } }); //set value for VR app to know the room view is set

            uiInitialized = true; //success
        }
        else if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
        {
            //Buttons
            machineButtons.SetActive(true); //show correct buttons
            alarmButton.interactable = false; //until machine has been instantiated
            //Room views
            learningRoomViewComponents.SetActive(true); //show correct room view

            uiInitialized = true; //success
        }
    }


    //method used to set object spawned by VR user (e.g. spider or machine)
    private void TrySetSpawnObject()
    {
        //when object to spawn instantiated by VR user - set object and enable functionalities
        if (spawnedObject == null)
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["ObjectInstantiated"] == true)
            {
                //try to set spawned GameObject to control
                try
                {
                    spawnedObject = PhotonView.Find(2001).gameObject; //runtime ViewID of spawned object is 2001
                }
                catch
                {
                    Debug.Log("Spawned Object not found yet.");
                }

                //Arachnophobia
                if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
                {
                    if (spawnedObject != null)
                    {
                        //set components of spawned object
                        spiderController = spawnedObject.GetComponent<SpiderController>(); 
                        objectPV = spawnedObject.GetComponent<PhotonView>(); 
                        //enable spawn buttons
                        spawnButton.interactable = true; 
                        spawnOntoPersonButton.interactable = true;
                    }
                }
                //Machine Operating
                else if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
                {
                    if (spawnedObject != null)
                    {
                        machineController = spawnedObject.GetComponent<MachineController>(); //set machine controller
                        objectPV = spawnedObject.GetComponent<PhotonView>();
                        alarmButton.interactable = true;
                    }
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
        SceneManager.LoadScene("UISupervisor");
    }


    //-------------------------------ARACHNOPHOBIA BUTTONS---------------------------------


    //Spawn button - let the supervisor spawn the phobia object (e.g. spider)
    public void SpawnButton()
    {
        Debug.Log("Spawn object...");

        if(spawnedObject != null) //if spider successfully set
        {
            //RPC call to make Game Object visible
            objectPV.RPC("Spawn", RpcTarget.All, true);

            //update EvaluationValueManager value
            valueManager.SpiderSpawned = true;

            //Gray button out - can only spawn one object
            spawnButton.interactable = false;
            spawnOntoPersonButton.interactable = false;
            //enable other buttons for spider control
            despawnButton.interactable = true;
            fleeButton.interactable = true;
            lookAtButton.interactable = true;
            moveToPosButton.interactable = true;
            moveToPatButton.interactable = true;
            moveOntoPatButton.interactable = true;
            stopSpiderButton.interactable = true;
        } 
    }

    //let the phobia object (e.g. spider) spawn at a defined point on the VR avatar
    public void SpawnOntoPersonButton()
    {
        Debug.Log("Spawn object onto person...");

        if (spawnedObject != null) //if spider successfully set
        {
            //RPC call to make Game Object visible
            objectPV.RPC("SpawnOntoPerson", RpcTarget.All, true);

            //update EvaluationValueManager value
            valueManager.SpiderSpawned = true;

            //Gray buttons out - can only spawn one object & can not move that object away from the VR avatar spot
            spawnButton.interactable = false;
            spawnOntoPersonButton.interactable = false;
            fleeButton.interactable = false;
            lookAtButton.interactable = false;
            moveToPosButton.interactable = false;
            moveToPatButton.interactable = false;
            moveOntoPatButton.interactable = false;
            stopSpiderButton.interactable = false;
            //enable only despawn button
            despawnButton.interactable = true;
        }
    }


    //Despawn button - let the supervisor instantly despawn the phobia object (e.g. spider)
    public void DespawnButton()
    {
        Debug.Log("Despawn object...");

        //RPC call to make Game Object invisible
        objectPV.RPC("Spawn", RpcTarget.All, false);
        objectPV.RPC("Stop", RpcTarget.All); //stop potential current action

        //update EvaluationValueManager value
        valueManager.SpiderSpawned = false;

        //reactivate SpawnButton and deactivate other buttons
        ResetButtonsArachnophobia();
    }


    //Flee button - command phobia object (e.g. spider) to flee
    public void FleeButton()
    {
        Debug.Log("Let object flee...");

        //RPC call to object - command to flee
        if(!spiderController.Dead)
            objectPV.RPC("Flee", RpcTarget.All);

        //despawn spider after short time
        StartCoroutine(DespawnAfterTime());

        //reactivate SpawnButton and deactivate despawn buttons
        ResetButtonsArachnophobia();
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

        if (!spiderController.Dead)
        {
            //RPC call to object
            objectPV.RPC("Stop", RpcTarget.All);

            //update EvaluationValueManager value
            valueManager.SpiderMovingToPos = false;
            valueManager.SpiderMovingToPatient = false;
        }
    }

    //LookAtPerson button - command phobia object (e.g. spider) to turn towards the VR user
    public void LookAtPersonButton()
    {
        Debug.Log("Let object look at person...");

        //command object to look at person
        if (!spiderController.Dead)
        {
            //RPC call to object
            objectPV.RPC("LookAtPerson", RpcTarget.All);

            //update EvaluationValueManager value
            valueManager.SpiderLooking = true;
            StartCoroutine(valueManager.ResetBoolAfterTime(valueManager.SpiderLooking));
        }
    }


    //MoveToPosition button - command phobia object (e.g. spider) to move to a chosen position
    public void MoveToPosButton()
    {
        Debug.Log("Let object move to position...");

        //command object  to move to chosen position
        if (!spiderController.Dead)
        {
            //change cursor
            Vector2 cursorOffset = new Vector2(mouseCrosshair.width / 2, mouseCrosshair.height / 2); //set the cursor origin to its center. (default is upper left corner)
            Cursor.SetCursor(mouseCrosshair, cursorOffset, CursorMode.Auto); //adapt mouse texture

            DisableObjectRelatedButtons(); //grey buttons out until position has been chosen

            Debug.Log("Choose position...");
            StartCoroutine(WaitforClick()); //wait until user has chosen a position with left mouse click

            //RPC call to object
            objectPV.RPC("MoveToPos", RpcTarget.All, ChoosePosition());

            ResetButtonsRoomView(); //reset buttons after choosing position
            Cursor.SetCursor(default, default, CursorMode.Auto); //reset cursor
            valueManager.SpiderMovingToPos = true; //update EvaluationValueManager value

            //pos reached - update EvaluationValueManager value
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["SpiderReachedCurrentGoal"])
                StartCoroutine(valueManager.ResetBoolAfterTime(valueManager.SpiderLooking));
        } 
    }

    //Coroutine used to select a position only after the left mouse button has been pushed.
    private IEnumerator WaitforClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                yield break;
            }
            yield return null;
        }
    }


    //MoveToPerson button - command phobia object (e.g. spider) to walk towards VR user
    public void MoveToPersonButton()
    {
        Debug.Log("Let object move to person...");
        
        if (!spiderController.Dead)
        {
            //RPC call to object
            objectPV.RPC("MoveToPerson", RpcTarget.All);

            //update EvaluationValueManager value
            valueManager.SpiderMovingToPos = true;

            //pos reached - update EvaluationValueManager value
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["SpiderReachedCurrentGoal"])
                StartCoroutine(valueManager.ResetBoolAfterTime(valueManager.SpiderLooking));
        }
    }


    //MoveOntoPerson button - command phobia object (e.g. spider) to climb onto the VR user avatar to a destined position
    public void MoveOntoPersonButton()
    {
        Debug.Log("Let object move onto person...");

        if (!spiderController.Dead)
        {
            //RPC call to object
            objectPV.RPC("WalkOnto", RpcTarget.All);

            //update EvaluationValueManager value
            valueManager.SpiderOntoPatient = true;
        }
    }


    //-------------------------------MACHINE LEARNING BUTTONS---------------------------------

    public void AlarmButton()
    {
        //disable button until alarm is over
        alarmButton.interactable = false;

        //RPC call to object to start alarm manually
        objectPV.RPC("StartAlarmManually", RpcTarget.All);
        //update EvaluationValueManager value
        valueManager.MachineAlarmActive = true;

        //if alarm has been solved - reset value manager bool
        if (!machineController.WarningLampEnabled)
        {
            alarmButton.interactable = true; //reset button
            //update EvaluationValueManager value
            StartCoroutine(valueManager.ResetBoolAfterTime(valueManager.MachineAlarmActive));
        }
    }

    //--------------------Functionalities--------------------


    //method used to let supervisor choose a position in the camera view 
    private Vector3 ChoosePosition()
    {
        Debug.Log("Set Position");

        Vector3 position = new Vector3();

        //set ray from camera to where mouse position is pointing
        Ray rayToEnvironment = mainCamera.ScreenPointToRay(Input.mousePosition);
        //if ray hits something with correct layer - choose that position
        if(Physics.Raycast(rayToEnvironment, out RaycastHit hit, float.MaxValue, roomViewEnvironmentWalkable))
        {
            position = hit.point;
        }

        return position;
    }


    //Arachnophobia: reset buttons after spawned spider object has been destroyed
    private void ResetButtonsArachnophobia()
    {
        //reactivate SpawnButton and deactivate other buttons
        spawnButton.interactable = true;
        spawnOntoPersonButton.interactable = true;
        despawnButton.interactable = false;
        fleeButton.interactable = false;
        lookAtButton.interactable = false;
        moveToPosButton.interactable = false;
        moveToPatButton.interactable = false;
        moveOntoPatButton.interactable = false;
        stopSpiderButton.interactable = false;
    }


    //Arachnophobia: reset buttons after chosen a position on the room view
    private void ResetButtonsRoomView()
    {
        //deactivate SpawnButton and reactivate other buttons
        spawnButton.interactable = false;
        spawnOntoPersonButton.interactable = false;
        despawnButton.interactable = true;
        fleeButton.interactable = true;
        lookAtButton.interactable = true;
        moveToPosButton.interactable = true;
        moveToPatButton.interactable = true;
        moveOntoPatButton.interactable = true;
        stopSpiderButton.interactable = true;
    }


    //method used in Awake() to disable all object related buttons until the object has been instantiated and set
    private void DisableObjectRelatedButtons()
    {
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            spawnButton.interactable = false;
            spawnOntoPersonButton.interactable = false;
            despawnButton.interactable = false;
            fleeButton.interactable = false;
            lookAtButton.interactable = false;
            moveToPosButton.interactable = false;
            moveToPatButton.interactable = false;
            moveOntoPatButton.interactable = false;
            stopSpiderButton.interactable = false;
        }
        else
        {
            alarmButton.interactable = false;
        }
    }


    //method used by RoomViewController script to get the spawned object
    public GameObject GetSpawnedObject()
    {
        return spawnedObject;
    }

    //method used by RoomViewController script to get the EvaluationValueManager
    public EvaluationValueManager GetValueManager()
    {
        return valueManager;
    }
}
