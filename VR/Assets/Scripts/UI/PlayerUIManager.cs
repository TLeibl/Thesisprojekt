using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerUIManager : MonoBehaviour
{
    //UI elements
    [SerializeField] private TextMeshProUGUI controlWalk = null;
    [SerializeField] private TextMeshProUGUI controlGrab = null;
    [SerializeField] private TextMeshProUGUI controlFeedback = null;
    [SerializeField] private Image feedbackBar = null;

    //feedback
    private float lastTriggerValue = 0.0f; //the last used trigger value
    private float calculationValue = 0.01f; //value used for feedback value calculation


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
        if(PhotonNetwork.CurrentRoom != null)
            feedbackBar.fillAmount = (float)PhotonNetwork.CurrentRoom.CustomProperties["FeedbackValue"];
    }

    //method used by VRCharController to calculate the current feedback value in dependence of the controller trigger value
    public float CalculateCurrentFeedbackValue(float currentFeedbackValue, float triggerValue)
    {
        //increase/decrease feedback value the more trigger is pressed or released
        //if value has only slightly changed: leave it constant - return from method
        if (Mathf.Approximately(triggerValue, lastTriggerValue))
            return currentFeedbackValue;
        else if (triggerValue > lastTriggerValue) //trigger is pressed more
            if (triggerValue > 0.8f) //trigger is pressed strongly - change value more rapid
                currentFeedbackValue += calculationValue * 2;
            else currentFeedbackValue += calculationValue; //else normal change
        else if(triggerValue < lastTriggerValue)
            if (triggerValue < 0.2f) //trigger is let go nearly completely - change value more rapid
                currentFeedbackValue -= calculationValue * 2;
            else currentFeedbackValue -= calculationValue; //else normal change

        //check if feedback value is between 0 and 100
        if (currentFeedbackValue > 100)
            currentFeedbackValue = 100;
        else if (currentFeedbackValue < 0)
            currentFeedbackValue = 0;

        //save trigger value as last trigger value
        lastTriggerValue = triggerValue;

        return currentFeedbackValue;
    }
}
