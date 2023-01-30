using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Manager that saves the current value of the feedback and occurring events for each time interval for
//later evaluation.
public class EvaluationValueManager : MonoBehaviour
{
    //value lists (list for feedback values and every event occuring)
    //Arachnophobia
    public List<float> feedbackValues = new List<float>();
    public List<bool> spiderSpawnedList = new List<bool>();
    public List<bool> spiderLookingList = new List<bool>();
    public List<bool> spiderMovingList = new List<bool>();
    public List<bool> spiderOntoPatientList = new List<bool>();
    public List<bool> spiderDeadList = new List<bool>();
    //Machine Learning
    public List<bool> machineAlarmList = new List<bool>();
    //list to manage all lists in
    public List<List<bool>> eventLists = new List<List<bool>>();

    //bools for occuring events
    //Arachnophobia
    private bool spiderDead = false;
    public bool SpiderDead   
    {
        get { return spiderDead; }  
        set { spiderDead = value; }  
    }
    private bool spiderSpawned = false;
    public bool SpiderSpawned
    {
        get { return spiderSpawned; }
        set { spiderSpawned = value; }
    }
    private bool spiderLooking = false;
    public bool SpiderLooking
    {
        get { return spiderLooking; }
        set { spiderLooking = value; }
    }
    private bool spiderMovingToPos = false;
    public bool SpiderMovingToPos
    {
        get { return spiderMovingToPos; }
        set { spiderMovingToPos = value; }
    }
    private bool spiderMovingToPatient = false;
    public bool SpiderMovingToPatient
    {
        get { return spiderMovingToPatient; }
        set { spiderMovingToPatient = value; }
    }
    private bool spiderOntoPatient = false;
    public bool SpiderOntoPatient
    {
        get { return spiderOntoPatient; }
        set { spiderOntoPatient = value; }
    }
    //Machine Learning
    private bool machineAlarmActive = false;
    public bool MachineAlarmActive
    {
        get { return machineAlarmActive; }
        set { machineAlarmActive = value; }
    }

    //save a value for each time interval
    private float timeElapsed = 0f; 
    private float timeInterval = 1f; //every time interval a value is saved


    private void Awake()
    {
        //keep object during whole run
        DontDestroyOnLoad(this.gameObject);

        SetupEventLists(); //add all lists to eventList for later use in EvaluationScenario
    }


    private void Update()
    {
        //update lists in each time interval
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeInterval) 
        {
            //update lists
            feedbackValues.Add((float)PhotonNetwork.CurrentRoom.CustomProperties["FeedbackValue"]);
            //update values for the current scenario
            if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
            {
                UpdateArachnophobiaValues();
            }
            else if((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
            {
                UpdateMachineOperatingValues();
            }

            timeElapsed = 0;
        }
    }


    //used in Awake() to add all lists to the eventList list
    private void SetupEventLists()
    {
        eventLists.Add(spiderSpawnedList);
        eventLists.Add(spiderLookingList);
        eventLists.Add(spiderMovingList);
        eventLists.Add(spiderOntoPatientList);
        eventLists.Add(spiderDeadList);
        eventLists.Add(machineAlarmList);
    }


    //method used in Update() to update arachnophobia scenario values
    private void UpdateArachnophobiaValues()
    {
        //update lists
        spiderSpawnedList.Add(SpiderSpawned);
        spiderLookingList.Add(SpiderLooking);
        spiderOntoPatientList.Add(SpiderOntoPatient);
        spiderDeadList.Add(SpiderDead);

        //if spider movement is true update that value
        if(SpiderMovingToPatient == true)
            spiderMovingList.Add(SpiderMovingToPatient);
        else if(SpiderMovingToPos == true)
            spiderMovingList.Add(SpiderMovingToPos);
        else //else just update one false value
            spiderMovingList.Add(SpiderMovingToPos);
    }


    //method used in Update() to update machine scenario values
    private void UpdateMachineOperatingValues()
    {
        //update list
        machineAlarmList.Add(MachineAlarmActive);
    }


    //method used by SupervisorUIManager to reset bools after some time
    public IEnumerator ResetBoolAfterTime(bool currentBool)
    {
        yield return new WaitForSecondsRealtime(timeInterval + 1f); //wait for time to always catch the true values
        currentBool = false;
    }
}
