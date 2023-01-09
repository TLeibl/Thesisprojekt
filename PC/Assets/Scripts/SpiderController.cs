using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

//script to call in supervisor UI to let spider move, stop or despawn and control the spider animator
public class SpiderController : MonoBehaviour
{
    //bools for occuring events
    private bool dead = false; 
    private bool spiderSpawned = false;
    private bool spiderLooking = false;
    private bool spiderMovingToPos = false;
    private bool spiderMovingToPatient = false;
    private bool spiderOntoPatient = false;

    //references
    private Animator animator = null; //the spider animator
    private NavMeshAgent agent = null; //NavMeshAgent of spider
    private Transform patient = null; //the patient transform (VR player object - OVRPlayerController)
    private Vector3 groundedPosition = Vector3.zero; //position of the spider when sitting on the floor to return to (only usage of y value)

    //distance to patient
    [SerializeField] private float patientDistance = 0.0f; //distance of spider to patient
    private float nearPatient = 2.0f; //value in which spider is near the patient 
    private bool inPatientRange = false; //if true spider is near patient

    private float fleeDistance = 4f; //distance for spider to flee
    //private float despawnDelay = 2.5f; //delay when despawning


    private void Awake()
    {
        //set references
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if(SceneManager.GetActiveScene().name == "MapPhobia") //if in MapPhobia scene - search patient
            patient = GameObject.Find("OVRPlayerController").transform;

        groundedPosition = this.transform.position;
    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
            //update distance to patient
            CalculatePatientDistance();
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


    //------------------------SPAWN AND DESPAWN-----------------------

    /// <summary>
    /// Method to let the spider spawn e.g. enable the game object 
    /// </summary>
    /// <param name="position">the position the spider shall be spawned</param>
    //public void SpawnSpider(Vector3 position)
    //{
    //    //set disabled spider to choosen position
    //    gameObject.transform.position = position;

    //    //enable spider
    //    gameObject.SetActive(true);

    //    //set bool
    //    spiderSpawned = true;
    //}


    //disable spider game object
    //public void DespawnSpider()
    //{
    //    gameObject.SetActive(false);
    //    //reset bools
    //    spiderSpawned = false;
    //    spiderLooking = false;
    //    spiderMovingToPatient = false;
    //    spiderMovingToPos = false;
    //    spiderOntoPatient = false;
    //}


    //---------------------------MOVEMENT-----------------------------

    //method used to let spider return to the floor (e.g. when dying)
    private void ReturnToFloor()
    {
        //if not already on floor: set y to be on floor (reset y to groundedPosition.y)
        if(!(transform.position.y == groundedPosition.y))
        {
            transform.position = new Vector3(transform.position.x, groundedPosition.y, transform.position.z);
        }
    }

    /// <summary>
    /// Let the spider look at a position, e.g. the patient.
    /// </summary>
    /// <param name="position"> The position the spider shall look at.</param>
    private void LookAt(Vector3 position)
    {
        //reset y axis so spider does not turn away from the ground
        Vector3 lookAtPosition = position;
        lookAtPosition.y = transform.position.y;

        Debug.Log("SPIDER LOOK AT: " + lookAtPosition);

        transform.LookAt(lookAtPosition);
    }


    //method called by supervisor to let spider stop and return to idle
    public void Stop()
    {
        //stop animations
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        //stop movement
        agent.SetDestination(transform.position);

        //set bool
        spiderMovingToPos = false;
        spiderMovingToPatient = false;
    }


    //method to let spider struggle if taken by VR character
    public void Struggle()
    {
        //struggle animation
        animator.SetBool("isStruggling", true);
    }


    // Let the spider look at a position, e.g. the patient.
    public void LookAtPerson()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
            LookAt(patient.position);

        //set bool
        spiderLooking = true;
        StartCoroutine("ResetSpiderLooking");
    }


    //reset spiderLooking after amount of time so EvaluationValueManager value is changed in time interval
    private IEnumerator ResetSpiderLooking()
    {
        yield return new WaitForSecondsRealtime(1f);
        spiderLooking = false;
    }


    //method called by supervisor to let spider walk to chosen position
    public void MoveToPosition(Vector3 position)
    {
        Debug.Log("SPIDER MOVE TO PATIENT POSITION: " + position);

        LookAt(position);
        //Walk animation
        animator.SetBool("isWalking", true);
        

        agent.SetDestination(position);
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            //walk until position reached
        }
        animator.SetBool("isWalking", false);
        //reset bools
        spiderMovingToPos = false;
        spiderMovingToPatient = false;
    }


    //method called by supervisor to let spider walk to patient
    public void MoveToPatient()
    {
        spiderMovingToPatient = true;

        if (SceneManager.GetActiveScene().name == "MapPhobia")
            MoveToPosition(patient.position);
    }


    //method called by supervisor to let spider walk towards patient
    public void WalkTowards()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
            //look at patient
            LookAt(patient.position);

        //set bool
        spiderLooking = true;
        StartCoroutine("ResetSpiderLooking");

        //if not already there - move into direction of patient
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            if (!inPatientRange)
            {
                MoveToPosition(patient.position);
                spiderMovingToPatient = true;
            }
        }
    }


    //method called by supervisor to let spider walk towards patient and climb onto him or her
    //TODO
    public void WalkOnto()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            //if not already there - look and move into direction of patient
            WalkTowards();

            //climb onto patient
            //if hand down - climb onto hand and up the arm

            //else climb up the legs and onto the arm

        }

        spiderOntoPatient = true;
    }


    //method called by supervisor to let spider run away from patient and despawn 
    public void Flee()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            //Walk animation
            animator.SetBool("isWalking", true);

            //move away from patient
            //if not already on floor: get down
            ReturnToFloor();

            //run away
            Vector3 dirAway = (transform.position - patient.transform.position).normalized; //determine normalized direction away from patient
            Vector3 newPos = transform.position + (dirAway * fleeDistance); //define new position for spider to run to 
            LookAt(newPos); //look at pos
            agent.SetDestination(newPos); //move there
        }

        Debug.Log("SPIDER FLEE");
            
        spiderMovingToPos = true;

        //despawn spider after short time
        //StartCoroutine(DespawnAfterTime());
    }

    ////Coroutine used when spider flees to despawn the spider after some time
    //private IEnumerator DespawnAfterTime()
    //{
    //    yield return new WaitForSeconds(despawnDelay);
    //    DespawnSpider();
    //}


    //method called by supervisor to let spider die
    public void Die()
    {
        //if not already there drop to the ground
        ReturnToFloor();

        //Death animation
        animator.SetBool("dead", true);

        //set bool
        dead = true;
    }


    //---------------------------GETTER-----------------------------


    public bool IsDead()
    {
        return dead;
    }

    //public bool IsSpawned()
    //{
    //    return spiderSpawned;
    //}

    public bool IsLooking()
    {
        return spiderLooking;
    }

    public bool IsMovingToPos()
    {
        return spiderMovingToPos;
    }

    public bool IsMovingToPatient()
    {
        return spiderMovingToPatient;
    }

    public bool IsOntoPatient()
    {
        return spiderOntoPatient;
    }

}
