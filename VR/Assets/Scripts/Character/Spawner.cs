using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    //instantiate needed objects and set them for the master client (supervisor)
    private void Awake()
    {    
        //Map Arachnophobia
        if (SceneManager.GetActiveScene().name == "MapPhobia" && (bool)PhotonNetwork.CurrentRoom.CustomProperties["ObjectInstantiated"] == false)
        {
            //instantiate spider for master client to use
            GameObject spawnedSpider = PhotonNetwork.Instantiate("Spider", new Vector3(-0.15f, 0.03f, 13.75f), Quaternion.identity, 0);

            if (spawnedSpider != null)
            {
                spawnedSpider.GetComponent<PhotonView>().ViewID = 3000; //set custom ViewID
                spawnedSpider.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient); //give object to MasterClient
                spawnedSpider.SetActive(false);
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ObjectInstantiated", true } });
            } 
        }
        //Map Learning
        else if (SceneManager.GetActiveScene().name == "MapLearning" && (bool)PhotonNetwork.CurrentRoom.CustomProperties["ObjectInstantiated"] == false)
        {
            //instantiate machine for master client to use 
            GameObject spawnedMachine = PhotonNetwork.Instantiate("Machine", new Vector3(1.509f, 0.308f, 2.686f), Quaternion.identity, 0);

            if (spawnedMachine != null)
            {
                spawnedMachine.GetComponent<PhotonView>().ViewID = 3000; //set custom ViewID
                spawnedMachine.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient); //give object to MasterClient

                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ObjectInstantiated", true } });
            }
        }

    }
}
