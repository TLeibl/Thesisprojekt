using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;

public class UIManager : MonoBehaviour
{
    //UI elements
    [SerializeField] private TextMeshProUGUI controlWalk = null;
    [SerializeField] private TextMeshProUGUI controlGrab = null;
    [SerializeField] private TextMeshProUGUI controlFeedback = null;
    [SerializeField] private Image feedbackBar = null;

    //feedback
    private float lastTriggerValue = 0.0f; //the last used trigger value
    private int valueDivider = 50; //value used for feedback value calculation


    //set texts in UI and start feedback value
    private void Awake()
    {
        //Wall UI
        controlWalk.text = "Walk - Left Thumbstick";
        controlGrab.text = "Grab - Left/Right Grip";
        //Player UI
        controlFeedback.text = "Feedback\nRight Trigger";
    }

    //update feedback bar
    private void Update()
    {
        //update FeedbackBar value
        feedbackBar.fillAmount = GameManager.FeedbackValue;
    }

    //method used by VRCharController to calculate the current feedback value in dependence of the controller trigger value
    public float CalculateCurrentFeedbackValue(float currentFeedbackValue, float triggerValue)
    {
        Debug.Log("triggerValue: " + triggerValue + ", lastTriggerValue: " + lastTriggerValue);

        //increase/decrease feedback value the more trigger is pressed or released
        if (triggerValue > lastTriggerValue)
            currentFeedbackValue = currentFeedbackValue + triggerValue / valueDivider;
        else if(triggerValue < lastTriggerValue)
            currentFeedbackValue =  currentFeedbackValue - triggerValue / valueDivider;
        //check if feedback value is between 0 and 100
        if (currentFeedbackValue > 100)
            currentFeedbackValue = 100;
        else if (currentFeedbackValue < 0)
            currentFeedbackValue = 0;

        Debug.Log("CURRENTFEEDBACKVALUE: " + currentFeedbackValue);

        //save trigger value as last trigger value
        lastTriggerValue = triggerValue;

        return currentFeedbackValue;
    }
}
