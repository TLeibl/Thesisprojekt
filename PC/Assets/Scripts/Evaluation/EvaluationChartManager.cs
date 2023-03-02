using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using AwesomeCharts;

//Manager script to build line chart with the evaluation values.
public class EvaluationChartManager : MonoBehaviour
{
    private LineChart chart = null; //the line chart
    private EvaluationValueManager valueManager = null; //value manager

    //Hashtable for individual event values used as Y value in the chart
    Hashtable individualYValues = new Hashtable(){
        {"SpiderSpawned", 100},
        {"SpiderLooking", 20},
        {"SpiderMoving", 40},
        {"SpiderOntoPatient", 60},
        {"SpiderDead", 80},
        {"MachineAlarm", 40}
    };


    // Find EvaluationValueManager and set up line chart
    private void Awake()
    {
        //set components
        chart = this.gameObject.GetComponent<LineChart>();
        valueManager = GameObject.Find("EvaluationManager").GetComponent<EvaluationValueManager>();
    }

    private void Start()
    {
        if (chart != null)
        {
            //Grid Config - set horizontal lines count to number of values in list so every value change is visible in the chart
            chart.GridConfig.HorizontalLinesCount = valueManager.feedbackValues.Count;

            //create and fill chart lines
            CreateAndFillFeedbackLine();
            CreateAndFillEventLines();
        }
    }


    //method used to create a line for the feedback values and fill it with the feedback values collected by the EvaluationValueManager
    private void CreateAndFillFeedbackLine()
    {
        // Create data set for entries 
        LineDataSet feedbackLine = new LineDataSet();

        // Configure line 
        feedbackLine.Title = "Feedback";
        feedbackLine.LineColor = Color.red; //red
        feedbackLine.LineThickness = 4;
        feedbackLine.UseBezier = true;

        //LineChart - Data - fill data in DataSets 
        // Add entries to data set 
        for (var i = 0; i < valueManager.feedbackValues.Count; i++)
        {
            feedbackLine.AddEntry(new LineEntry(i, valueManager.feedbackValues[i]));
            Debug.Log("FEEDBACKVALUES VALUE: " + valueManager.feedbackValues[i]);
        }

        // Add data set to chart data 
        chart.GetChartData().DataSets.Add(feedbackLine);

        // Refresh chart after data change 
        chart.SetDirty();

    }


    //method used to create a line for every event list and fill it with the values collected by the EvaluationValueManager
    private void CreateAndFillEventLines()
    {
        //-------------------------------------ARACHNOPHOBIA--------------------------------------------------
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            //SpiderSpawnedLine
            // Create data set for entries 
            LineDataSet spiderSpawnedLine = new LineDataSet();
            // Configure line 
            spiderSpawnedLine.Title = "Spawned";
            spiderSpawnedLine.LineColor = Color.white; 
            spiderSpawnedLine.LineThickness = 4;
            spiderSpawnedLine.UseBezier = true;
            // Add entries to data set 
            for (var i = 0; i < valueManager.spiderSpawnedList.Count; i++)
            {
                int currentYValue = GetEventValue(valueManager.spiderSpawnedList[i], "SpiderSpawned");
                spiderSpawnedLine.AddEntry(new LineEntry(i, currentYValue));
            }
            // Add data set to chart data 
            chart.GetChartData().DataSets.Add(spiderSpawnedLine);

            // Refresh chart after data change 
            chart.SetDirty();


            //SpiderLookingLine
            // Create data set for entries 
            LineDataSet spiderLookingLine = new LineDataSet();
            // Configure line 
            spiderLookingLine.Title = "LookAt";
            spiderLookingLine.LineColor = Color.grey; 
            spiderLookingLine.LineThickness = 4;
            spiderLookingLine.UseBezier = true;
            // Add entries to data set 
            for (var i = 0; i < valueManager.spiderLookingList.Count; i++)
            {
                int currentYValue = GetEventValue(valueManager.spiderLookingList[i], "SpiderLooking");
                spiderLookingLine.AddEntry(new LineEntry(i, currentYValue));
            }
            // Add data set to chart data 
            chart.GetChartData().DataSets.Add(spiderLookingLine);

            // Refresh chart after data change 
            chart.SetDirty();


            //SpiderMovingLine
            // Create data set for entries 
            LineDataSet spiderMovingLine = new LineDataSet();
            // Configure line 
            spiderMovingLine.Title = "Moving";
            spiderMovingLine.LineColor = Color.green;
            spiderMovingLine.LineThickness = 4;
            spiderMovingLine.UseBezier = true;
            // Add entries to data set 
            for (var i = 0; i < valueManager.spiderMovingList.Count; i++)
            {
                int currentYValue = GetEventValue(valueManager.spiderMovingList[i], "SpiderMoving");
                spiderMovingLine.AddEntry(new LineEntry(i, currentYValue));
            }
            // Add data set to chart data 
            chart.GetChartData().DataSets.Add(spiderMovingLine);

            // Refresh chart after data change 
            chart.SetDirty();


            //SpiderOntoPatientLine
            // Create data set for entries 
            LineDataSet spiderOntoPatientLine = new LineDataSet();
            // Configure line 
            spiderOntoPatientLine.Title = "OntoPatient";
            spiderOntoPatientLine.LineColor = Color.blue;
            spiderOntoPatientLine.LineThickness = 4;
            spiderOntoPatientLine.UseBezier = true;
            // Add entries to data set 
            for (var i = 0; i < valueManager.spiderOntoPatientList.Count; i++)
            {
                int currentYValue = GetEventValue(valueManager.spiderOntoPatientList[i], "SpiderOntoPatient");
                spiderOntoPatientLine.AddEntry(new LineEntry(i, currentYValue));
            }
            // Add data set to chart data 
            chart.GetChartData().DataSets.Add(spiderOntoPatientLine);

            // Refresh chart after data change 
            chart.SetDirty();

            //TODO add if needed
            /*
            //SpiderDeadLine
            // Create data set for entries 
            LineDataSet spiderDeadLine = new LineDataSet();
            // Configure line 
            spiderDeadLine.LineColor = Color.black;
            spiderDeadLine.LineThickness = 4;
            spiderDeadLine.UseBezier = true;
            // Add entries to data set 
            for (var i = 0; i < valueManager.spiderDeadList.Count; i++)
            {
                int currentYValue = GetEventValue(valueManager.spiderDeadList[i], "SpiderDead");
                spiderDeadLine.AddEntry(new LineEntry(i, currentYValue));
            }
            // Add data set to chart data 
            chart.GetChartData().DataSets.Add(spiderDeadLine);
            // Refresh chart after data change 
            chart.SetDirty();
            */
        }

        //----------------------------------------MACHINE LEARNING--------------------------------------------------
        else if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
        {
            //MachineAlarmLine
            // Create data set for entries 
            LineDataSet machineAlarmLine = new LineDataSet();
            // Configure line 
            machineAlarmLine.Title = "Alarm active";
            machineAlarmLine.LineColor = Color.green;
            machineAlarmLine.LineThickness = 4;
            machineAlarmLine.UseBezier = true;
            // Add entries to data set 
            for (var i = 0; i < valueManager.machineAlarmList.Count; i++)
            {
                int currentYValue = GetEventValue(valueManager.machineAlarmList[i], "MachineAlarm");
                machineAlarmLine.AddEntry(new LineEntry(i, currentYValue));
            }
            // Add data set to chart data 
            chart.GetChartData().DataSets.Add(machineAlarmLine);

            // Refresh chart after data change 
            chart.SetDirty();
        }
    }


    //method to get the individual Y value belonging to the current bool value
    private int GetEventValue(bool eventHappened, string eventName)
    {
        int individualYValue = 0;

        //if bool true - get affiliated Y value
        if (eventHappened)
        {
            individualYValue = (int)individualYValues[eventName];
        }

        return individualYValue;
    }
}
