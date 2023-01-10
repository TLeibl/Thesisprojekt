using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Spawner : MonoBehaviour
{
    PhotonView ownPView = null;

    //instantiate needed objects and set them for the master client (supervisor)
    private void Awake()
    {
        //get own PhotonView
        //ownPView = GetComponent<PhotonView>();

        //instantiate spider for master client to use
        GameObject spawnedSpider = PhotonNetwork.Instantiate("Spider", new Vector3(-0.15f, 0.03f, 13.75f), Quaternion.identity, 0);
        
        if(spawnedSpider != null)
            spawnedSpider.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
            spawnedSpider.SetActive(false);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "SpiderInstantiated", true } });

        //ownPView.RPC("SetSpider", RpcTarget.MasterClient, spawnedSpider);

    }
}
