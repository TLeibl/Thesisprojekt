using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Manager that saves the current value of the feedback and occurring events for each time interval for
//later evaluation.
public class EvaluationValueManager : MonoBehaviour
{
    //current VR user and spider object
    [SerializeField] private VRCharController vrUser = null;
    [SerializeField] private SpiderController spider = null;

    //value lists (list for feedback values and every event occuring)
    private List<float> feedbackValues = new List<float>();
    private List<bool> spiderSpawned = new List<bool>();
    private List<bool> spiderLooking = new List<bool>();
    private List<bool> spiderMovingToPos = new List<bool>();
    private List<bool> spiderMovingToPatient = new List<bool>();
    private List<bool> spiderOntoPatient = new List<bool>();
    private List<bool> spiderDied = new List<bool>();

    //save a value for each time interval
    private float timeElapsed = 0f; 
    private float timeInterval = 1f; //every time interval a value is saved


    private void Awake()
    {
        //keep object during whole run
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        //update lists in each time interval
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeInterval) 
        {
            //update lists
            feedbackValues.Add(vrUser.GetCurrentFeedbackValue());
            //if phobia scenario - update spider values
            if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
            {
                spiderSpawned.Add(spider.IsSpawned());
                spiderLooking.Add(spider.IsLooking());
                spiderMovingToPos.Add(spider.IsMovingToPos());
                spiderMovingToPatient.Add(spider.IsMovingToPatient());
                spiderOntoPatient.Add(spider.IsOntoPatient());
                spiderDied.Add(spider.IsDead());
            }

            timeElapsed = 0;
        }
    }
}
