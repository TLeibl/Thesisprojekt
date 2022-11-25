using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI controlWalk = null;
    [SerializeField] private TextMeshProUGUI controlFeedback = null;
    [SerializeField] private TextMeshProUGUI controlGrab = null;

    //set texts in UI and start feedback value
    private void Awake()
    {
        controlWalk.text = "Walk - Left Thumbstick";
        controlFeedback.text = "Feedback\nRight Trigger";
        controlGrab.text = "Grab - Left/Right Grip";
    }

    //update feedback value in UI
    private void Update()
    {
        //update Feedback value
        //TODO
        //_____ = GameManager.FeedbackValue;
    }
}
