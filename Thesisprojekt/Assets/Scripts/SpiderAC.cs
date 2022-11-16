using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAC : MonoBehaviour
{
    private Animator animator = null; //the spider animator

    private float patientDistance = 0.0f;
    private float nearPatient = 3.0f; //TODO set //value in which spider is near the patient 


    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CalculatePatientDistance();
    }


    //method used to calculate the distance of the spider to the patient
    private void CalculatePatientDistance()
    {
        //TODO
    }



    //---------------------------MOVEMENT-----------------------------


    //method to let spider struggle if taken by VR character
    public void Struggle()
    {
        //TODO 

        //let the spider hang in the grip

        //struggle animation
        animator.SetBool("isStruggling", true);
    }


    //method called by supervisor to let spider stop and return to idle
    public void Stop()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }

    //method called by supervisor to let spider walk towards patient
    public void WalkTowards()
    {
        //TODO

        //Walk animation
        animator.SetBool("isWalking", true);
        //if not already there - move into direction of patient
        if(patientDistance > nearPatient)
        {

        }

    }


    //method called by supervisor to let spider walk towards patient and climb onto him or her
    public void WalkOnto()
    {
        //TODO

        //Walk animation
        animator.SetBool("isWalking", true);
        //if not already there - move into direction of patient
        if(patientDistance > nearPatient)
        {

        }

        //climb onto patient
        //if hand down - climb onto hand and up the arm

        //else climb up the legs and onto the arm
    }


    //method called by supervisor to let spider die
    public void Die()
    {
        //TODO

        //if not already there drop to the ground

        //Death animation
        animator.SetBool("dead", true);
    }


    //---------------------------GETTER AND SETTER-----------------------------

    public float GetPatientDistance()
    {
        return patientDistance;
    }


}
