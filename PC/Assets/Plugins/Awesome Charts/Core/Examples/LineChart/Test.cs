using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeCharts;

public class Test : MonoBehaviour
{
    private LineChart chart = null;


    private void Awake()
    {
        chart = this.gameObject.GetComponent<LineChart>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create data set for entries 
        LineDataSet set = new LineDataSet();
        // Add entries to data set 
        set.AddEntry(new LineEntry(0, 100f));
        set.AddEntry(new LineEntry(100, 50));
        set.AddEntry(new LineEntry(150, 70));
        set.AddEntry(new LineEntry(180, 130));
        // Configure line 
        set.LineColor = Color.red;
        set.LineThickness = 4;
        set.UseBezier = true;
        // Add data set to chart data 
        chart.GetChartData().DataSets.Add(set);
        // Refresh chart after data change 
        chart.SetDirty();
    }

    
}
