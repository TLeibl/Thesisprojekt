using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    private GameObject spawnedObject = null; //the object instantiated

    //instantiate needed objects and set them for the master client (supervisor)
    private void Start()
    {
        //Map Arachnophobia
        if (SceneManager.GetActiveScene().name == "MapPhobia" 
            && (bool)PhotonNetwork.CurrentRoom.CustomProperties["ObjectInstantiated"] == false)
        {
            if(spawnedObject == null)
                //instantiate spider for master client to use
                spawnedObject = PhotonNetwork.Instantiate("Spider", new Vector3(-0.15f, 0.03f, 13.75f), Quaternion.identity, 0).gameObject;

            if (spawnedObject != null)
            {
                spawnedObject.GetComponent<SpiderController>().groundedPosition = spawnedObject.transform.position; //set grounded pos of object
                spawnedObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient); //give object to MasterClient

                spawnedObject.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().enabled = false; //set invisible until officially spawned
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ObjectInstantiated", true } });
            }
        }
        //Map Learning
        else if (SceneManager.GetActiveScene().name == "MapLearning" && (bool)PhotonNetwork.CurrentRoom.CustomProperties["ObjectInstantiated"] == false)
        {
            if(spawnedObject == null)
                //instantiate machine for master client to use 
                spawnedObject = PhotonNetwork.Instantiate("Machine", new Vector3(-1.45f, 0.308f, -1.12f), Quaternion.identity, 0);

            if (spawnedObject != null)
            {
                spawnedObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient); //give object to MasterClient

                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ObjectInstantiated", true } });
            }
        }
    }
}
