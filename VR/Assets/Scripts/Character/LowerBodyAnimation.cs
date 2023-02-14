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


    //method called by the Animation Controller when an animation is started
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
            SetFootWeightsAndPosition(AvatarIKGoal.LeftFoot, this.leftFootPosWeight, this.leftFootRotWeight, hitLeftFoot);
        }
        else //if not: do not calculate weights
        {
            this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        }

        //if right foot is down - set weights, position and rotation of it
        if (isRightFootDown)
        {
            SetFootWeightsAndPosition(AvatarIKGoal.RightFoot, this.rightFootPosWeight,this.rightFootRotWeight, hitRightFoot);
        }
        else //if not: do not calculate weights
        {
            this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }
    }

    /// <summary>
    /// method used to set the weight, position and rotation of the current foot
    /// </summary>
    /// <param name="goal">the AvatarIKGoal of the foot (e.g. AvatarIKGoal.RightFoot)</param>
    /// <param name="weight">the pos weight of the foot</param>
    /// <param name="goalPosition">where the foot shall stand without the offset</param>
    /// <param name="rotation">rotation of the foot</param>
    private void SetFootWeightsAndPosition(AvatarIKGoal goal, float posWeight, float rotWeight, RaycastHit goalPosition)
    {
        this.animator.SetIKPositionWeight(goal, posWeight);
        this.animator.SetIKPosition(goal, goalPosition.point + this.footOffset);

        Quaternion goalRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, goalPosition.normal), goalPosition.normal);
        this.animator.SetIKRotationWeight(goal, rotWeight);
        this.animator.SetIKRotation(goal, goalRotation);
    }
}
