using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderAC : MonoBehaviour
{
    //references
    private Animator animator = null; //the spider animator
    private NavMeshAgent agent = null; //NavMeshAgent of spider
    private Transform patient = null; //the patient transform (VR player object - OVRPlayerController)

    [SerializeField] private float patientDistance = 0.0f; //distance of spider to patient
    private float nearPatient = 2.0f; //value in which spider is near the patient 
    private bool inPatientRange = false; //if true spider is near patient


    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        patient = GameObject.Find("OVRPlayerController").transform;
    }

    private void Update()
    {
        //update distance to patient
        CalculatePatientDistance();
        Flee();
    }


    //method used to calculate the distance of the spider to the patient
    private void CalculatePatientDistance()
    {
        patientDistance = Vector3.Distance(this.transform.position, patient.transform.position);
        //if in defined range - set inPatientRange true, else false
        if (patientDistance <= nearPatient)
            inPatientRange = true;
        else inPatientRange = false;
    }



    //---------------------------MOVEMENT-----------------------------

    /// <summary>
    /// Let the spider look at a position, e.g. the patient.
    /// </summary>
    /// <param name="position"> The position the spider shall look at.</param>
    private void LookAt(Vector3 position)
    {
        //reset y axis so spider does not turn away from the ground
        Vector3 lookAtPosition = position;
        lookAtPosition.y = transform.position.y;

        transform.LookAt(lookAtPosition);
    }


    //method to let spider struggle if taken by VR character
    public void Struggle()
    {
        //struggle animation
        animator.SetBool("isStruggling", true);
    }


    //method called by supervisor to let spider stop and return to idle
    public void Stop()
    {
        //stop animations
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        //stop movement
        agent.SetDestination(transform.position);
    }

    //method called by supervisor to let spider walk towards patient
    public void WalkTowards()
    {
        //look at patient
        LookAt(patient.position);

        //if not already there - move into direction of patient
        if(!inPatientRange)
        {
            //Walk animation
            animator.SetBool("isWalking", true);

            agent.SetDestination(patient.position);
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                //walk until patient position reached
            }
            animator.SetBool("isWalking", false);
        }

    }


    //method called by supervisor to let spider walk towards patient and climb onto him or her
    //TODO
    public void WalkOnto()
    {
        //if not already there - look and move into direction of patient
        WalkTowards();

        //climb onto patient
        //if hand down - climb onto hand and up the arm

        //else climb up the legs and onto the arm
    }


    //method called by supervisor to let spider run away from patient and despawn 
    //TODO
    public void Flee()
    {
        //Walk animation
        animator.SetBool("isWalking", true);

        //move away from patient
        //if not already on floor: get down

        //run away
        Vector3 dirToPatient = transform.position - patient.transform.position; //calculate current Vector3 from spider to patient
        Vector3 newPos = transform.position + dirToPatient; //define new position for spider to run to 
        LookAt(newPos); //look at pos
        agent.SetDestination(newPos); //move there
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            //walk until flee position reached
        }

        //despawn
        //Destroy(gameObject);
    }


    //method called by supervisor to let spider die
    //TODO
    public void Die()
    {
        //if not already there drop to the ground

        //Death animation
        animator.SetBool("dead", true);
    }

}
