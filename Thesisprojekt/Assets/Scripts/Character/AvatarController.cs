using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //show it in editor

//transforms used for mapping VR components to the avatar IKs
public class MapTransforms
{
    [SerializeField] private Transform vrTarget;
    [SerializeField] private Transform ikTarget;
    //set in editor
    [SerializeField] private Vector3 trackingPositionOffset; 
    [SerializeField] private Vector3 trackingRotationOffset;

    public void VRMapping()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset); 
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

//class to map the avatar to the VR movement so it can be controlled via VR controllers or hands
public class AvatarController : MonoBehaviour
{
    [SerializeField] private MapTransforms head;
    [SerializeField] private MapTransforms leftHand;
    [SerializeField] private MapTransforms rightHand;

    [SerializeField] private float turnSmoothness; //for turning off the head
    [SerializeField] private Transform ikHead;
    [SerializeField] private Vector3 headBodyOffset; //height offset

    private void LateUpdate()
    {
        //change avatar position and rotation according to VR movement
        transform.position = ikHead.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(ikHead.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness); //line body to head via y axis rotation, smooth rotation via lerp

        //map IKs to VR movement
        head.VRMapping();
        leftHand.VRMapping();
        rightHand.VRMapping();
    }
}
