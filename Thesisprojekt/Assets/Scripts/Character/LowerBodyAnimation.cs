using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerBodyAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    //body parts position weights
    [SerializeField] [Range(0, 1)] private float leftFootPosWeight;
    [SerializeField] [Range(0, 1)] private float rightFootPosWeight;
    //body parts rotation weights
    [SerializeField] [Range(0, 1)] private float leftFootRotWeight;
    [SerializeField] [Range(0, 1)] private float rightFootRotWeight;

    [SerializeField] private Vector3 footOffset; //general foot offset

    //raycasts to determine how near the VR player is the floor to let avatar crouch if necessary
    [SerializeField] private Vector3 raycastOffsetLeft;
    [SerializeField] private Vector3 raycastOffsRight;


    private void OnAnimatorIK(int layerIndex)
    {
        //get foot positions
        Vector3 leftFootPos = this.animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 rightFootPos = this.animator.GetIKPosition(AvatarIKGoal.RightFoot);

        //calculate if VR player's foots are touching the floor by using the raycasts
        RaycastHit hitLeftFoot;
        RaycastHit hitRightFoot;

        bool isLeftFootDown = Physics.Raycast(leftFootPos + this.raycastOffsetLeft, Vector3.down, out hitLeftFoot);
        bool isRightFootDown = Physics.Raycast(rightFootPos + this.raycastOffsRight, Vector3.down, out hitRightFoot);

        //if left foot is down - set weights, position and rotation of it
        if (isLeftFootDown)
        {
            this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, this.leftFootPosWeight);
            this.animator.SetIKPosition(AvatarIKGoal.LeftFoot, hitLeftFoot.point + this.footOffset);

            Quaternion leftFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitLeftFoot.normal), hitLeftFoot.normal);
            this.animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, this.leftFootRotWeight);
            this.animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);
        }
        else //if not: do not calculate weights
        {
            this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        }

        //if right foot is down - set weights, position and rotation of it
        if (isRightFootDown)
        {
            this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, this.rightFootPosWeight);
            this.animator.SetIKPosition(AvatarIKGoal.RightFoot, hitRightFoot.point + this.footOffset);

            Quaternion rightFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitRightFoot.normal), hitRightFoot.normal);
            this.animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, this.rightFootRotWeight);
            this.animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRotation);
        }
        else //if not: do not calculate weights
        {
            this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
}
