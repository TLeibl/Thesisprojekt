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
            //value can be between 0 and 100
            if(feedbackValue > 100)
            {
                feedbackValue = 100;
            }
        }
    }
}
