using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//class used to sense contact with the VR character layer and start RPCs
public class Lever : MonoBehaviour
{
    private PhotonView pv = null; //own PhotonView component


    private void Awake()
    {
        //set PhotonView for sending RPCs
        pv = gameObject.GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //VR user started interacting with the lever - send RPC to Room View Controller
        if(collision.gameObject.layer == 8) //= "VRUser" layer
            pv.RPC("SetObjectUsed", RpcTarget.MasterClient, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        //VR user stopped interacting with the lever - send RPC to Room View Controller
        if (collision.gameObject.layer == 8) //= "VRUser" layer
            pv.RPC("SetObjectUsed", RpcTarget.MasterClient, false);
    }
}
