using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerBodyAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    //body parts position weights
    [SerializeField] [Range(0, 1)] private float leftFootPositionWeight;
    [SerializeField] [Range(0, 1)] private float rightFootPositionWeight;
    //body parts rotation weights
    [SerializeField] [Range(0, 1)] private float leftFootRotationWeight;
    [SerializeField] [Range(0, 1)] private float rightFootRotationWeight;

    [SerializeField] private Vector3 footOffset; //general foot offset

    //raycasts to determine how near the VR player is the floor to let avatar crouch if necessary
    [SerializeField] private Vector3 raycastLeftOffset;
    [SerializeField] private Vector3 raycastRightOffset;


    private void OnAnimatorIK(int layerIndex)
    {
        //get foot positions
        Vector3 leftFootPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 rightFootPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);

        //calculate if VR player's foots are touching the floor by using the raycasts
        RaycastHit hitLeftFoot;
        RaycastHit hitRightFoot;

        bool isLeftFootDown = Physics.Raycast(leftFootPosition + raycastLeftOffset, Vector3.down, out hitLeftFoot);
        bool isRightFootDown = Physics.Raycast(rightFootPosition + raycastRightOffset, Vector3.down, out hitRightFoot);

        CalculateLeftFoot(isLeftFootDown, hitLeftFoot);
        CalculateRightFoot(isRightFootDown, hitRightFoot);
       
    }

    //method to calculate left foot position and rotation
    private void CalculateLeftFoot(bool isLeftFootDown, RaycastHit hitLeftFoot)
    {
        //if left foot is down: set weights, position and rotation of it
        if (isLeftFootDown)
        {
            //position
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, hitLeftFoot.point + footOffset);

            //rotation
            //project position of left foot onto plane (floor) and look at rotation of the foot
            Quaternion leftFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitLeftFoot.normal), hitLeftFoot.normal);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);
        }
        else //if not: do not calculate weights
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
        }
    }


    //method to calculate left foot position and rotation
    private void CalculateRightFoot(bool isRightFootDown, RaycastHit hitRightFoot)
    {
        //if right foot is down: set weights, position and rotation of it
        if (isRightFootDown)
        {
            //position
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPositionWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, hitRightFoot.point + footOffset);

            //rotation
            //project position of right foot onto plane (floor) and look at rotation of the foot
            Quaternion rightFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitRightFoot.normal), hitRightFoot.normal);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootRotationWeight);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRotation);
        }
        else //if not: do not calculate weights
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
}
