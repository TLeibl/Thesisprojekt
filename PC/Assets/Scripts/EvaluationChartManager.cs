using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Manager script to build line chart with the evaluation values.
public class EvaluationChartManager : MonoBehaviour
{
    //TODO ANPASSEN!

    List<GameObject> lineList = new List<GameObject>();

    private DD_DataDiagram m_DataDiagram; //the diagram

    private EvaluationValueManager valueManager = null;

    // Find EvaluationValueManager and set up line chart
    private void Start()
    {
        //find EvaluationValueManager
        valueManager = GameObject.Find("EvaluationManager").GetComponent<EvaluationValueManager>();

        //get DataDiagram object and script
        GameObject dd = GameObject.Find("DataDiagram");
        if (null == dd)
        {
            Debug.LogWarning("Can't find DataDiagram!");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        //Add lines for feedback and other values
        AddLine(Color.red); //feedback
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
        {
            AddLine(Color.green); //spiderSpawned
            AddLine(Color.blue); //spiderLooking
            AddLine(Color.cyan); //spiderMovingToPos
            AddLine(Color.gray); //spiderMovingToPatient
            AddLine(Color.yellow); //spiderOntoPatient
            AddLine(Color.black); //spiderDied
        }
    }

    //Add a line to the diagram.
    private void AddLine(Color color)
    {
        if (null == m_DataDiagram)
            return;

        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);

        SetLineValues(line);
    }

    //Add all evaluation values to the specified line
    //TODO (Werte von EvaluationValueManager)
    public void SetLineValues(GameObject line)
    {
        if (null == m_DataDiagram)
            return;

        foreach (GameObject l in lineList)
        {
            m_DataDiagram.InputPoint(l, new Vector2(1, Random.value * 4f));
        }
    }
}
