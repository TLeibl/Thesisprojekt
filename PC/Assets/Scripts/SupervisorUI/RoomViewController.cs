using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomViewController : MonoBehaviour
{
    private GameObject roomViewComponents = null; //current room view components = the parent object of the script
    //room view objects
    [SerializeField] private GameObject vrUser;
    [SerializeField] private GameObject spawnedObject;
    //current positions, rotations or action status set by RCP calls
    private Vector3 vrUserPos;
    private Vector3 vrUserRot;
    private Vector3 objectPos;
    private Vector3 objectRot;
    private bool performingAction = false;
    private bool objectCurrentlyUsed = false;

    private bool objectInstantiated = false; //check if spider instantiated

    //used materials
    private MeshRenderer vrUserRenderer = null; //to set VRUser material
    private MeshRenderer objectRenderer = null; //to set spawned object material 
    //action colors
    [SerializeField] private Material objectActivatedMaterial; //used when object is activated e.g. alarm or button
    [SerializeField] private Material actionMaterial; //used when object does something
    [SerializeField] private Material inactiveMaterial; //used when object is inactive
    //materials of objects involved in actions
    [SerializeField] private Material vrUserMaterial; 
    [SerializeField] private Material spiderMaterial;
    [SerializeField] private Material buttonMaterial;
    [SerializeField] private Material leverMaterial;
    [SerializeField] private Material alarmLightMaterial;

    [SerializeField] private SupervisorUIManager uiManager = null; //SupervisorUIManager
    private EvaluationValueManager valueManager = null; //EvaluationValueManager
    private NetworkingManager.Scenario currentScenario; //the current scenario


    private void Awake()
    {
        //get the parent Game Object = the current room view components
        roomViewComponents = transform.parent.gameObject;

        //set scenario
        if((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            currentScenario = NetworkingManager.Scenario.Arachnophobia;
        }
        else currentScenario = NetworkingManager.Scenario.MachineOperating;

        //set EvaluationValueManager
        valueManager = uiManager.GetValueManager();

        //set room view object mesh renderers
        vrUserRenderer = vrUser.GetComponent<MeshRenderer>();
        objectRenderer = spawnedObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Arachnophobia: check if spider is ready if not instantiated yet 
        if (!objectInstantiated && currentScenario == NetworkingManager.Scenario.Arachnophobia)
        {
            if(uiManager.GetSpawnedObject() != null)
            {
                objectInstantiated = true;
            }
        }


        //update values if object instantiated
        if (objectInstantiated)
        {
            if (currentScenario == NetworkingManager.Scenario.Arachnophobia)
            {
                UpdateSpiderPosAndRot();
            }

            UpdateStatus();
            UpdateVRUser();
        }
    }


    //Update the position and rotation of the spider object. 
    private void UpdateSpiderPosAndRot()
    {
        if(spawnedObject != null)
        {
            //TODO Anpassen Position
            spawnedObject.transform.position = objectPos;
            spawnedObject.transform.rotation = Quaternion.Euler(objectRot);
        }
    }


    //Update the current status of all objects and change their color accordingly.
    private void UpdateStatus()
    {
        //Arachnophobia
        if(spawnedObject != null)
        {
            //if Arachnophobia - check if spider is currently spawned and set it (in)visible accordingly
            if(currentScenario == NetworkingManager.Scenario.Arachnophobia)
            {
                UpdateSpiderStatus();
            }
            else //Machine Learning
            {
                UpdateMachinePartsStatus();
            }
        }
    }


    //method used by UpdateStatus to update spider visibility and color
    private void UpdateSpiderStatus()
    {
        try
        {
            if(spawnedObject)
            //show spider if spawned
            if (valueManager.SpiderSpawned)
                spawnedObject.SetActive(true);
            else spawnedObject.SetActive(false);

            //update object color
            if (valueManager.SpiderSpawned || valueManager.SpiderLooking || valueManager.SpiderMovingToPos
                || valueManager.SpiderMovingToPatient || valueManager.SpiderOntoPatient)
            {
                objectRenderer.material = actionMaterial;
            }
            else if (valueManager.SpiderDead)
                objectRenderer.material = inactiveMaterial;
            else
            {
                objectRenderer.material = spiderMaterial;
            }
        }
        catch { }
        
    }


    //method used by UpdateStatus to update machine components color
    private void UpdateMachinePartsStatus()
    {
        //Update each child object color of the room view components (machine control surface).
        //If the object is activated, the color changes.
        foreach (GameObject child in roomViewComponents.transform)
        {
            MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
            //Buttons
            if (child.tag == "Button")
            {
                if (objectCurrentlyUsed)
                    childRenderer.material = objectActivatedMaterial;
                else childRenderer.material = buttonMaterial;
            }
            //Levers
            else if(child.tag == "Lever")
            {
                if (objectCurrentlyUsed)
                    childRenderer.material = objectActivatedMaterial;
                else childRenderer.material = leverMaterial;
            }
            //Alarm
            else if(child.tag == "Alarm")
            {
                if (valueManager.MachineAlarmActive)
                    childRenderer.material = objectActivatedMaterial;
                else childRenderer.material = alarmLightMaterial;
            }
        }
    }

    private void UpdateVRUser()
    {
        //update pos and rot
        //TODO Anpassen Position
        vrUser.transform.position = vrUserPos;
        vrUser.transform.rotation = Quaternion.Euler(vrUserRot);

        //update color
        if (performingAction)
        {
            vrUserRenderer.material = actionMaterial;
        }
        else vrUserRenderer.material = vrUserMaterial;
    }


    //--------------------------RPC Setter methods------------------------

    //set by VRCharController - current VR avatar position
    [PunRPC]
    void SetCurrentVRUserPosition(Vector3 pos)
    {
        vrUserPos = pos;
    }


    //set by VRCharController - current VR avatar rotation
    [PunRPC]
    void SetCurrentVRUserRotation(Vector3 rot)
    {
        vrUserRot = rot;
    }


    //set by SpiderController - current spider object position
    [PunRPC]
    void SetCurrentObjectPosition(Vector3 pos)
    {
        objectPos = pos;
    }


    //set by SpiderController - current spider object rotation
    [PunRPC]
    void SetCurrentObjectRotation(Vector3 rot)
    {
        objectRot = rot;
    }


    //set by MachineController - true if e.g. button is used
    [PunRPC]
    void SetObjectUsed(bool currentlyUsed)
    {
        objectCurrentlyUsed = currentlyUsed;
    }
}
