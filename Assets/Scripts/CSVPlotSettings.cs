using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CSVPlotSettings : PlotSettings
{
    protected override void GetReferencesToComponents()
    {
        base.GetReferencesToComponents();
        GameObject plotObject = GameObject.Find("CSVPlot");
        plot = plotObject.GetComponent<Plot>();
    }
}
