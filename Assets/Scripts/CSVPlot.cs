using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using LumenWorks.Framework.IO.Csv;
using UnityEngine;

public class CSVPlot : Plot
{
    public static String PathToCsvFile { get; set; }

    public static int MAX_INTIAL_POINTS = 20000;

    private Thread renderingThread;

    public override void Start()
    {
        base.Start();
        ReadParticlesFromCSVFile(PathToCsvFile);
    }

    public override void Update()
    {
        if (renderingThread != null && !renderingThread.IsAlive)
        {
            Draw();
            renderingThread = null;
        }
    }

    // Helper function for setting the particles based on values retrieved from the CSV file.
    private void ReadParticlesFromCSVFile(String pathToCSVFile)
    {
        ThreadStart threadedParticleWriter = new ThreadStart(() => WriteParticlesToParticleList(
            pathToCSVFile));
        renderingThread = new Thread(threadedParticleWriter);
        renderingThread.Start();
    }

    private void WriteParticlesToParticleList(String pathToCSVFile)
    {
        LinkedList<float[]> particlesList = new LinkedList<float[]>();
        using (CsvReader csv = new CsvReader(new StreamReader(pathToCSVFile), true))
        {
            int fieldCount = csv.FieldCount;
            EnsureFieldIndicesAreValid(csv.FieldCount);

            // Determine whether the column titles are actually titles by checking
            // to see if they can be parsed as floats.
            ColumnTitles = csv.GetFieldHeaders();
            try
            {
                for (int i = 0; i < ColumnTitles.Length; i++)
                {
                    float.Parse(ColumnTitles[i]);
                }

                // The remaining code in this try block will only execute if the
                // "titles" were successfully parsed as floats (meaning they aren't
                // really titles at all).
                float[] dataPoint = new float[fieldCount];
                for (int i = 0; i < ColumnTitles.Length; i++)
                {
                    dataPoint[i] = float.Parse(ColumnTitles[i]);
                    ColumnTitles[i] = "Feature " + i.ToString();
                }
                particlesList.AddLast(dataPoint);
            }
            catch (FormatException)
            {
                // Do nothing
            }
            ColumnTitlesAreReady = true;

            // Read each entry from the CSV file and convert it into a data point.
            while (csv.ReadNextRecord())
            {
                float[] dataPoint = new float[fieldCount];
                for (int i = 0; i < dataPoint.Length; i++)
                {
                    try
                    {
                        dataPoint[i] = float.Parse(csv[i]);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                particlesList.AddLast(dataPoint);
            }
        }

        // If there are too many points, throttle the amount of points that are initially displayed.
        if (particlesList.Count > MAX_INTIAL_POINTS)
        {
            Debug.Log("throttling");
            Data.SampleRatePercent = (float) MAX_INTIAL_POINTS / (float) particlesList.Count * 100f;
        }

        float[][] dataPoints = new float[particlesList.Count][];
        particlesList.CopyTo(dataPoints, 0);
        Data.SetData(dataPoints);
    }

    void EnsureFieldIndicesAreValid(int numFeatures)
    {
        if (Data.XSpatialFieldIndex < 0 || Data.XSpatialFieldIndex >= numFeatures)
        {
            Data.XSpatialFieldIndex = 0;
        }
        if (Data.YSpatialFieldIndex < 0 || Data.YSpatialFieldIndex >= numFeatures)
        {
            Data.YSpatialFieldIndex = 0;
        }
        if (Data.ZSpatialFieldIndex < 0 || Data.YSpatialFieldIndex >= numFeatures)
        {
            Data.ZSpatialFieldIndex = 0;
        }
        if (Data.SizeFieldIndex < 0 || Data.SizeFieldIndex >= numFeatures)
        {
            Data.SizeFieldIndex = 0;
        }
        if (Data.ColorFieldIndex < 0 || Data.ColorFieldIndex >= numFeatures)
        {
            Data.ColorFieldIndex = 0;
        }
    }

    // Helper function to convert a triplet of CSV entries to a Vector3.
    private Vector3 CsvEntryToVector3(String x, String y, String z)
    {
        return new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    // Helper function to convert a triplet of CSV entries to a Color.
    private Color CsvEntryToColor(String x, String y, String z)
    {
        return new Color(float.Parse(x), float.Parse(y), float.Parse(z));
    }
}
