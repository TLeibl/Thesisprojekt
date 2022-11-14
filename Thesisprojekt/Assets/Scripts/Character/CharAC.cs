using UnityEngine;
using UnityEngine.InputSystem;


//Script used to control the animator of the VR character.
public class CharAC : MonoBehaviour
{
    [SerializeField] private InputActionReference move; //use reference of the InputAction for walking
    [SerializeField] private Animator animator; //the VR character animator

    private void OnEnable()
    {
        //animate the legs - walk
        this.move.action.started += this.AnimateLegs;
        this.move.action.canceled += this.StopAnimateLegs;
    }


    private void OnDisable()
    {
        //stop animating the legs - go idle
        this.move.action.started -= this.AnimateLegs;
        this.move.action.canceled -= this.StopAnimateLegs;
    }


    //method to start the walk animation
    private void AnimateLegs(InputAction.CallbackContext obj)
    {
        //check whether movement is forwards or backwards
        bool isWalkingForward = this.move.action.ReadValue<Vector2>().y > 0; //if greater zero bool is true = char should walk forward, else walk backwards

        if (isWalkingForward) //start animation and move forward if true
        {
            this.animator.SetBool("isWalking", true); //start animation
            this.animator.SetFloat("animSpeed", 1.0f); //set speed of animation
        }
        else //move backwards
        {
            this.animator.SetBool("isWalking", true);
            this.animator.SetFloat("animSpeed", -1.0f); 
        }
    }


    //method to stop the walk animation - go back to idle
    private void StopAnimateLegs(InputAction.CallbackContext obj)
    {
        this.animator.SetBool("isWalking", false);
        this.animator.SetFloat("animSpeed", 0.0f);
    }
}
