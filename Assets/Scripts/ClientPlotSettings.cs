using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClientPlotSettings : PlotSettings
{
    private InputField port;
    private InputField ip;

    protected override void GetReferencesToComponents()
    {
        base.GetReferencesToComponents();
        GameObject plotObject = GameObject.Find("ClientPlot");
        plot = plotObject.GetComponent<Plot>();
        port = GameObject.Find("Port").GetComponentInChildren<InputField>();
        ip = GameObject.Find("IP").GetComponentInChildren<InputField>();
    }

    protected override void AttachEventHandlers()
    {
        base.AttachEventHandlers();
        ClientPlot cplot = (ClientPlot) plot;
        port.onEndEdit.AddListener(cplot.HandlePortChanged);
        ip.onEndEdit.AddListener(cplot.HandleIPChanged);
    }
}
