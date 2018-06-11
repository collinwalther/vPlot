using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ServerPlot : Plot
{
    /// <summary>
    ///     A DataStore object that is responsible for delivering the
    ///     data retrieved from an external source to this plot.
    /// </summary>
    /// <value>
    ///     The datastore.
    /// </value>
    protected DataStore Datastore { get; set; }

    public string IP { get; set; }

    /// <summary>
    ///     The port dedicated for listening for data.  The default
    ///     is 27343, but this property may be changed before
    ///     instantiation of any ServerPlot if you want to use a
    ///     different port.
    /// </summary>
    public static ushort port = 27343;

    public override void Start()
    {
        base.Start();
        Datastore = new DataStore();
        RunThreadedServer();
    }

    /// <summary>
    ///     Starts the server that listens for incoming data
    ///     in a new thread, so as not to block the thread
    ///     that plots the data.
    /// </summary>
    void RunThreadedServer()
    {
        Server server = new Server(port, Datastore);
        IP = server.GetIPAddress();
        Thread serverThread = new Thread(server.Run);
        serverThread.Start();
    }

    public override void Update()
    {
        if (UpdateData() > 0)
        {
            Draw();
        }
    }

    /// <summary>
    ///     Retrieves as many data points as are ready from the Datastore
    ///     property and puts them into the Data property.
    /// </summary>
    /// <returns>
    ///     The number of data points retrieved from the Datastore.
    /// </returns>
    protected long UpdateData()
    {
        // Get the points and figure out how many there are.
        float[][] newPoints = Datastore.Dequeue();
        if (newPoints == null)
        {
            return 0;
        }
        long length = newPoints.Length;
        if (length < 1)
        {
            return 0;
        }

        // Update the points in Data.
        Data.AddData(newPoints);

        // If the field names haven't been set yet, do so now.
        ColumnTitles = Datastore.Labels;
        if (ColumnTitles != null && !ColumnTitlesAreReady)
        {
            ColumnTitlesAreReady = true;
        }

        return length;
    }
}
