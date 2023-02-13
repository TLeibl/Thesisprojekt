using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomViewController : MonoBehaviour
{
    private GameObject roomViewComponents = null; //current room view components = the parent object of the script
    //room view objects
    [SerializeField] private GameObject vrUser;
    [SerializeField] private GameObject spawnedObject; 
    private GameObject spawnedObjectOriginal; //the original spawned Game Object

    [SerializeField] private SupervisorUIManager uiManager = null; //SupervisorUIManager


    private void Awake()
    {
        //get the parent Game Object = the current room view components
        roomViewComponents = transform.parent.gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        //try to set spawned object (possible after VR user instantiated it and supervisor set it)
        if(spawnedObjectOriginal == null)
        {
            TrySetSpawnedObject();
        }
    }

    //get spawned object from SupervisorUIManager
    private void TrySetSpawnedObject()
    {
        spawnedObject = uiManager.GetSpawnedObject();
    }
}
