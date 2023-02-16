using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

//script to call in supervisor UI to let spider move, stop or despawn and control the spider animator
public class SpiderController : MonoBehaviour
{
    //references
    public Vector3 groundedPosition = Vector3.zero; //position of the spider when sitting on the floor to return to 
    private Animator animator = null; //the spider animator
    private NavMeshAgent agent = null; //NavMeshAgent of spider
    private Transform patient = null; //the patient transform (VR player object - OVRPlayerController)
    private PhotonView pv = null; //the photon view of the spider (to send RPCs)

    //distance to patient
    [SerializeField] private float patientDistance = 0.0f; //distance of spider to patient
    private float nearPatient = 2.0f; //value in which spider is near the patient 
    private bool inPatientRange = false; //if true spider is near patient

    private float fleeDistance = 4f; //distance for spider to flee
    //private float despawnDelay = 2.5f; //delay when despawning

    private bool dead = false; //true when spider is spawned and dead
    public bool Dead
    {
        get { return dead; }
        set { dead = value; }
    }


    private void Awake()
    {
        //set references
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (SceneManager.GetActiveScene().name == "MapPhobia") //if in MapPhobia scene - search patient and set own PhotonView
        {
            patient = GameObject.Find("OVRPlayerController").transform;
            pv = gameObject.GetComponent<PhotonView>();
        }

        groundedPosition = this.transform.position;
    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            //update distance to patient
            CalculatePatientDistance();

            //update values for supervisor UI room view
            UpdateRPCValues();
        }
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

    //------------------------RPCs---------------------------

    //RPC called by SupervisorUIManager to let spider (de)spawn
    [PunRPC]
    protected void Spawn(bool spawn)
    {
        transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().enabled = spawn;
    }


    //method used to let spider return to the floor (e.g. when dying)
    private void DropToFloor()
    {
        //if not already on floor: set y to be on floor (reset y to groundedPosition.y)
        if (!(transform.position.y == groundedPosition.y))
        {
            transform.position = new Vector3(transform.position.x, groundedPosition.y, transform.position.z); //change y axis value so spider is grounded
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

        Debug.Log("Spider look at: " + lookAtPosition);

        try
        {
            transform.LookAt(lookAtPosition);
        }
        catch
        {
            Debug.Log("Spider of supervisor scene (shall not move) or something went wrong!");
        }
    }


    //RPC called by supervisor to let spider stop and return to idle
    [PunRPC]
    protected void Stop()
    {
        try
        {
            //stop animations
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);

            //stop movement
            agent.SetDestination(transform.position);
        }
        catch
        {
            Debug.Log("Spider of supervisor scene (shall not move) or something went wrong!");
        }
    }


    //method to let spider struggle if taken by VR character
    [PunRPC]
    protected void Struggle()
    {
        try
        {
            //struggle animation
            animator.SetBool("isStruggling", true);
        }
        catch { }
    }


    // Let the spider look at a position, e.g. the patient.
    [PunRPC]
    protected void LookAtPerson()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
            LookAt(patient.position);

    }

    //method called by supervisor to let spider walk to patient
    [PunRPC]
    protected bool MoveToPerson()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
            MoveToPosition(patient.position);
        return true;
    }


    //method called by supervisor to let spider walk to chosen position
    [PunRPC]
    protected bool MoveToPos(Vector3 chosenPos)
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
            MoveToPosition(chosenPos);
        return true;
    }


    //method called by supervisor to let spider walk towards patient and climb onto him or her
    //TODO
    [PunRPC]
    protected void WalkOnto()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            //if not already there - look and move into direction of patient
            WalkTowards();

            //climb onto patient
            //if hand down - climb onto hand and up the arm
            //TODO try-catch

            //else climb up the legs and onto the arm

        }
    }


    //method called by supervisor to let spider run away from patient and despawn 
    [PunRPC]
    protected void Flee()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            try
            {
                //move away from patient
                Vector3 dirAway = (transform.position - patient.transform.position).normalized; //determine normalized direction away from patient
                Vector3 newPos = transform.position + (dirAway * fleeDistance); //define new position for spider to run to 
                LookAt(newPos); //look at pos
                agent.SetDestination(newPos); //move there

                //Walk animation
                animator.SetBool("isWalking", true);
            }
            catch
            {
                Debug.Log("Spider of supervisor scene (shall not move) or something went wrong!");
            }
        }
    }


    //method called by supervisor to let spider die
    [PunRPC]
    protected void Die()
    {
        try
        {
            //if not already there drop to the ground
            DropToFloor();

            //Death animation
            animator.SetBool("dead", true);
            dead = true;
        }
        catch { }
    }


    //---------------------------USED METHODS-----------------------------

    //method used to let spider walk to chosen position
    public bool MoveToPosition(Vector3 position)
    {
        Debug.Log("Move To Position: " + position);

        try
        {
            LookAt(position);
            //Walk animation
            animator.SetBool("isWalking", true);


            agent.SetDestination(position);
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                //walk until position reached
            }
            animator.SetBool("isWalking", false);
            return true;
        }
        catch
        {
            Debug.Log("Spider of supervisor scene (shall not move) or something went wrong!");
        }

        return false;
    }


    //method called by supervisor to let spider walk towards patient
    private void WalkTowards()
    {
        if (SceneManager.GetActiveScene().name == "MapPhobia")
        {
            try
            {
                //look at patient
                LookAt(patient.position);

                //if not already there - move into direction of patient
                if (!inPatientRange)
                {
                    MoveToPosition(patient.position);
                }
            }
            catch
            {
                Debug.Log("Spider of supervisor scene (shall not move) or something went wrong!");
            }
        }
    }


    //------------------------UPDATE RPCs--------------------

    private void UpdateRPCValues()
    {
        //TODO Fix
        //pv.RPC("SetCurrentObjectPosition", RpcTarget.MasterClient, gameObject.transform.position);
        //pv.RPC("SetCurrentObjectRotation", RpcTarget.MasterClient, gameObject.transform.rotation);
    }

}

