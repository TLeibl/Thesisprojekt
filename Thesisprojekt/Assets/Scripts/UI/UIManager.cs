using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI controlWalk = null;
    [SerializeField] private TextMeshProUGUI controlGrab = null;
    [SerializeField] private TextMeshProUGUI controlFeedback = null;
    [SerializeField] private Image feedbackBar = null;

    //set texts in UI and start feedback value
    private void Awake()
    {
        //Wall UI
        controlWalk.text = "Walk - Left Thumbstick";
        controlGrab.text = "Grab - Left/Right Grip";
        //Player UI
        controlFeedback.text = "Feedback\nRight Trigger";
    }

    //update feedback value in UI
    private void Update()
    {
        //update FeedbackBar value
        feedbackBar.fillAmount = GameManager.FeedbackValue;
    }
}
