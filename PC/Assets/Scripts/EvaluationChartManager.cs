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

    private LineChart chart = null; //the line chart

    // Find EvaluationValueManager and set up line chart
    private void Awake()
    {
        //find EvaluationValueManager
        valueManager = GameObject.Find("EvaluationManager").GetComponent<EvaluationValueManager>();

        //set correct line chart for current scenario
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            chart = GameObject.Find("ArachnophobiaChart").GetComponentInChildren<LineChart>();
        }
        else if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
        {
            chart = GameObject.Find("MachineLearningChart").GetComponentInChildren<LineChart>();
        }

        if(chart != null)
            chart.gameObject.SetActive(true); //make chart visible
    }

    private void FillFeedbackLine()
    {
        //TODO
        //Grid Config - set horizontal lines count to number of values in list
        chart.GridConfig.HorizontalLinesCount = valueManager.feedbackValues.Count;


        //LineChart - Data - fill data in DataSets 

    }

    private void FillEventLine()
    {
        //TODO
        //Grid Config - adapt horizontal lines count to highest number of entries in a list
        int numberOfItems = 0;
        foreach(List<bool> list in valueManager.eventLists)
        {
            if (list.Count > numberOfItems)
                numberOfItems = list.Count;
        }
        chart.GridConfig.HorizontalLinesCount = numberOfItems;

        //LineChart - Data - fill data in DataSets --> if true own value for each event, if false 0

        bool eventHappened = false;

        GetEventValue(eventHappened);

    }


    private int GetEventValue(bool eventHappened)
    {
        //TODO
        int individualValue = 0;

        //get individual value from e.g. hashmap
        //Spawned = 100, LookAt = 20, Moving = 40, OntoPatient = 60

        return individualValue;
    }
}
