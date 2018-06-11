using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerPlotSettings : PlotSettings
{
    private InputField port;
    private InputField ip;

    public override void Start()
    {
        base.Start();
        port.text = ServerPlot.port.ToString();
        ip.text = ((ServerPlot) plot).IP;
    }

    protected override void GetReferencesToComponents()
    {
        base.GetReferencesToComponents();
        GameObject plotObject = GameObject.Find("ServerPlot");
        plot = plotObject.GetComponent<Plot>();
        port = GameObject.Find("Port").GetComponentInChildren<InputField>();
        ip = GameObject.Find("IP").GetComponentInChildren<InputField>();
    }

    protected override void UpdateActiveOptions()
    {
        base.UpdateActiveOptions();
        port.text = ServerPlot.port.ToString();
        ip.text = ((ServerPlot) plot).IP;
    }

    public override void Update()
    {
        base.Update();
        if (((ServerPlot) plot).IP != null)
        {
            ip.text = ((ServerPlot) plot).IP;
        }
    }
}
