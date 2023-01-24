using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Manager script to build line chart with the evaluation values.
public class EvaluationChartManager : MonoBehaviour
{
    //TODO ANPASSEN!

    List<GameObject> lineList = new List<GameObject>();

    private EvaluationValueManager valueManager = null;

    // Find EvaluationValueManager and set up line chart
    private void Start()
    {
        //find EvaluationValueManager
        valueManager = GameObject.Find("EvaluationManager").GetComponent<EvaluationValueManager>();

        //get line chart object and script
       
    }

    private void FillFeedbackLine()
    {
        //TODO Line: Feedback in LineChart-Skript - Data - DataSets füllen
    }

    private void FillEventLine()
    {
        //TODO Line: Events (wie Bool darstellen?) in LineChart-Skript - Data - DataSets füllen
    }
}
