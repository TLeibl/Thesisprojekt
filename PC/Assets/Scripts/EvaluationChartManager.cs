using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using AwesomeCharts;

//Manager script to build line chart with the evaluation values.
public class EvaluationChartManager : MonoBehaviour
{
    private EvaluationValueManager valueManager = null; //value manager

    [SerializeField] private LineChart chart = null; //the line chart

    // Find EvaluationValueManager and set up line chart
    private void Start()
    {
        //find EvaluationValueManager
        valueManager = GameObject.Find("EvaluationManager").GetComponent<EvaluationValueManager>();
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
