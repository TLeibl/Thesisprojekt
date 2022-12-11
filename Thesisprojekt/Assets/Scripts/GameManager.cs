using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //the feedback value that can be set by the VR user and seen by both users
    private static float feedbackValue = 0;

    public static float FeedbackValue
    {
        get => feedbackValue;
        set
        {
            feedbackValue = value;
            //value can be between 0 and 100
            if(feedbackValue > 100)
            {
                feedbackValue = 100;
            }
        }
    }


    //all possible scenarios that can be chosen by the supervisor
    public enum Scenario
    {
        Arachnophobia,
        MachineOperating
    }


    private static Scenario chosenScenario; //the scenario the supervisor has chosen


    //the in the ChooseScenario scene chosen scenario 
    public static Scenario ChosenScenario
    {
        get => chosenScenario;
        set
        {
            chosenScenario = value;
        }
    }


    private void Awake()
    {
        //keep values during whole run
        DontDestroyOnLoad(this.gameObject);
    }
}
