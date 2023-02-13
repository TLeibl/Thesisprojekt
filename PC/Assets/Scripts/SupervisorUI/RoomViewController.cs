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
    private GameObject spawnedObjectOriginal; //the original spawned Game Object

    //used materials
    [SerializeField] private Material objectActivatedMaterial; //used when object is activated e.g. alarm or button
    [SerializeField] private Material actionMaterial; //used when object does something

    [SerializeField] private SupervisorUIManager uiManager = null; //SupervisorUIManager
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
    }

    // Update is called once per frame
    private void Update()
    {
        //try to set spawned object (possible after VR user instantiated it and supervisor set it)
        if(spawnedObjectOriginal == null)
        {
            TrySetSpawnedObject();
        }

        if (currentScenario == NetworkingManager.Scenario.Arachnophobia)
        {
            UpdateSpiderPosAndRot();
        }

        UpdateStatus();
    }

    //Get spawned object from SupervisorUIManager.
    private void TrySetSpawnedObject()
    {
        spawnedObject = uiManager.GetSpawnedObject();
    }


    //Update the position and rotation of the spider object. 
    private void UpdateSpiderPosAndRot()
    {

    }


    //Update the current status of all objects and change their color accordingly.
    private void UpdateStatus()
    {

    }
}
