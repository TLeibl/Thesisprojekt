using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static int feedbackValue = 0;

    public static int FeedbackValue
    {
        get => feedbackValue;
        set
        {
            feedbackValue = value;
        }
    }

    public void UpdateFeedbackValue(int newFeedbackValue)
    {
        feedbackValue = newFeedbackValue;
    }
}
