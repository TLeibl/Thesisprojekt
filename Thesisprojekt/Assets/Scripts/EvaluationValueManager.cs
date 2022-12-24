using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manager that saves the current value of the feedback and occurring events for each time interval for
//later evaluation.
public class EvaluationValueManager : MonoBehaviour
{
    //value lists (list for feedback values and every event occuring)
    private List<float> feedbackValues = new List<float>();
    private List<bool> spiderSpawned = new List<bool>();
    private List<bool> spiderLooking = new List<bool>();
    private List<bool> spiderMovingToPos = new List<bool>();
    private List<bool> spiderMovingToPatient = new List<bool>();
    private List<bool> spiderOnPatient = new List<bool>();

    //save a value for each time interval
    private float timeElapsed = 0f; 
    private float timeInterval = 3f; //every time interval a value is saved


    private void Update()
    {
        //update lists in each time interval
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeInterval) 
        {
            //update lists
            //feedbackValues.Add(currentFeedbackValue);
            //spiderSpawned.Add(bool);
            //spiderLooking.Add(bool);
            //spiderMovingToPos.Add(bool);
            //spiderMovingToPatient.Add(bool);
            //spiderOnPatient.Add(bool);

            timeElapsed = 0;
        }
    }
}
