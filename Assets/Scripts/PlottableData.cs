using UnityEngine;
using System;
using System.Collections.Generic;

public class PlottableData : MonoBehaviour
{
    // Settables by the user.
    public int XSpatialFieldIndex { get; set; }
    public int YSpatialFieldIndex { get; set; }
    public int ZSpatialFieldIndex { get; set; }
    public bool IsArrowEnabled { get; set; }
    public float ArrowPointiness { get; set; }
    public int XArrowFieldIndex { get; set; }
    public int YArrowFieldIndex { get; set; }
    public int ZArrowFieldIndex { get; set; }
    public bool IsColorEnabled { get; set; }
    public int ColorFieldIndex { get; set; }
    public bool IsSizeEnabled { get; set; }
    public float ParticleSize { get; set; }
    public int SizeFieldIndex { get; set; }
    public Color MinColor { get; set; }
    public Color MaxColor { get; set; }
    public float MaxSize { get; set; }
    public float MinSize { get; set; }
    public float SampleRatePercent { get; set; }
    public bool IsColorLinearlyInterpolated { get; set; }
    public bool IsSizeLinearlyInterpolated { get; set; }
    public float XDrillMin { get; set; }
    public float XDrillMax { get; set; }
    public float YDrillMin { get; set; }
    public float YDrillMax { get; set; }
    public float ZDrillMin { get; set; }
    public float ZDrillMax { get; set; }
    public int NumFeatures { get; set; }
    public GameObject arrowPrefab;

    private LinkedList<float[]> Data;
    private float[] MaxDataValues { get; set; }
    private float[] MinDataValues { get; set; }
    private SortedList<float, int>[] ColumnRanks { get; set; }
    private delegate Vector3 PositionGettingFunction(float[] dataPoint);
    private LinkedList<GameObject> PreviousArrows { get; set; }

    /// <summary>
    ///     Default constructor.
    /// </summary>
    public PlottableData()
    {
        // Set sensible defaults for some of the properties.
        IsColorEnabled = true;
        IsSizeEnabled = true;
        ParticleSize = .01f;
        MinColor = Color.green;
        MaxColor = Color.red;
        MaxSize = .01f;
        MinSize = .001f;
        SampleRatePercent = 100f;
        IsColorLinearlyInterpolated = true;
        IsSizeLinearlyInterpolated = true;
        XDrillMin = 0;
        XDrillMax = 1;
        YDrillMin = 0;
        YDrillMax = 1;
        ZDrillMin = 0;
        ZDrillMax = 1;
        ArrowPointiness = 2;
    }

    /// <summary>
    ///     Constructs a PlottableData object from the given data.
    /// </summary>
    /// <param name="data">
    ///     The data from which to construct the PlottableData object.
    /// </param>
    public PlottableData(float[][] data) : this()
    {
        SetData(data);
    }

    /// <summary>
    ///     Sets the actual data of this PlottableData object.
    /// </summary>
    /// <param name="data">
    ///     The data.
    /// </param>
    public void SetData(float[][] data)
    {
        try
        {
            NumFeatures = data[0].Length;
        }
        catch (Exception)
        {
            NumFeatures = 0;
        }
        if (NumFeatures == 0)
        {
            throw new ArgumentException("data's second dimension must be greater than 0.");
        }
        ColumnRanks = new SortedList<float, int>[data[0].Length];
        this.Data = new LinkedList<float[]>(data);
        if (data.Length < 1)
        {
            return;
        }

        // Set the min and max values for the data, so we don't have to iterate through the data
        // to get these values every time we need them.
        this.MaxDataValues = (float[]) data[0].Clone();
        this.MinDataValues = (float[]) data[0].Clone();
        foreach (float[] point in this.Data)
        {
            for (int j = 0; j < NumFeatures; j++)
            {
                if (point[j] < this.MinDataValues[j])
                {
                    this.MinDataValues[j] = point[j];
                }
                if (point[j] > this.MaxDataValues[j])
                {
                    this.MaxDataValues[j] = point[j];
                }
            }
        }

        for (int i = 0; i < NumFeatures; i++)
        {
            SetColumnRank(i);
        }
    }

    public void AddData(float[] point)
    {
        if (this.Data == null)
        {
            SetData(new float[][] { point });
            return;
        }

        for (int i = 0; i < NumFeatures; i++)
        {
            UpdateColumnRank(point, i);
            if (point[i] < this.MinDataValues[i])
            {
                this.MinDataValues[i] = point[i];
            }
            if (point[i] > this.MaxDataValues[i])
            {
                this.MaxDataValues[i] = point[i];
            }
        }

        this.Data.AddLast(point);
    }

    private void UpdateColumnRank(float[] point, int col)
    {
        float newVal = point[col];
        SortedList<float, int> ranks = ColumnRanks[col];
        if (!ranks.ContainsKey(newVal))
        {
            ranks.Add(newVal, 0);
        }
    }

    public void AddData(float[][] points)
    {
        foreach (float[] point in points)
        {
            AddData(point);
        }
    }

    /// <summary>
    ///     A utility function for determining the sorted order of each column, and saving these orders to a dictionary
    ///     so that the rank of a value can be later determined in constant time.
    /// </summary>
    private void SetColumnRank(int column)
    {
        if (Data == null || Data.Count < 1 || !IsValidIndex(column) || ColumnRanks[column] != null)
        {
            return;
        }
        ColumnRanks[column] = new SortedList<float, int>();

        foreach (float[] point in this.Data)
        {
            if (!ColumnRanks[column].ContainsKey(point[column]))
            {
                ColumnRanks[column].Add(point[column], 0);
            }
        }
    }

    /// <summary>
    ///     Generates the particles according to the current settings of the PlottableData object.
    ///     Before calling this method, you may want to define what various fields of the given data
    ///     mean by setting XSpatialFieldIndex, ColorFieldIndex, etc.
    /// </summary>
    /// <returns>
    ///     An array of particles that may be passed to a particle system and displayed.  Returns
    ///     null if data has not been set.
    /// </returns>
    public ParticleSystem.Particle[] GetParticles()
    {
        if (Data == null)
        {
            return null;
        }

        if (!IsColorLinearlyInterpolated && IsValidIndex(ColorFieldIndex))
        {
            SetColumnRank(ColorFieldIndex);
        }
        if (!IsSizeLinearlyInterpolated && IsValidIndex(SizeFieldIndex))
        {
            SetColumnRank(SizeFieldIndex);
        }

        LinkedList<ParticleSystem.Particle> particlesList = new LinkedList<ParticleSystem.Particle>();
        System.Random random = new System.Random(80085);
        PositionGettingFunction GetPosition = GetPositionGettingFunction();

        foreach (float[] point in this.Data)
        {
            if (random.NextDouble() > (SampleRatePercent / 100f) + .00001)
            {
                continue;
            }

            if (!DrillDownValid(GetPosition(point)))
            {
                continue;
            }

            ParticleSystem.Particle particle = new ParticleSystem.Particle
            {
                position = GetPosition(point),
                startColor = GetColor(point),
                startSize = GetSize(point)
            };

            particlesList.AddLast(particle);
        }
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particlesList.Count];
        particlesList.CopyTo(particles, 0);
        return particles;
    }

    public GameObject[] GetArrows()
    {
        if (Data == null || arrowPrefab == null)
        {
            return null;
        }

        if (PreviousArrows != null)
        {
            foreach (GameObject arrow in PreviousArrows)
            {
                Destroy(arrow);
            }
        }

        if (!IsColorLinearlyInterpolated && IsValidIndex(ColorFieldIndex))
        {
            SetColumnRank(ColorFieldIndex);
        }
        if (!IsSizeLinearlyInterpolated && IsValidIndex(SizeFieldIndex))
        {
            SetColumnRank(SizeFieldIndex);
        }

        LinkedList<GameObject> arrowsList = new LinkedList<GameObject>();
        System.Random random = new System.Random(80085);
        PositionGettingFunction GetPosition = GetPositionGettingFunction();

        foreach (float[] point in this.Data)
        {
            if (random.NextDouble() > (SampleRatePercent / 100f) + .00001)
            {
                continue;
            }

            if (!DrillDownValid(GetPosition(point)))
            {
                continue;
            }

            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrow.SetActive(false);
            arrow.GetComponentInChildren<MeshRenderer>().material.color = GetColor(point);
            arrow.transform.position = GetPosition(point);
            arrow.transform.localScale = GetScale(point);
            arrow.transform.rotation = GetRotation(point);

            arrowsList.AddLast(arrow);
        }

        PreviousArrows = arrowsList;
        GameObject[] arrows = new GameObject[arrowsList.Count];
        arrowsList.CopyTo(arrows, 0);
        return arrows;
    }


    /// <summary>
    ///     A helper function that determines whether a point should be plotted,
    ///     based on whether the current DrillDown settings allow for the point
    ///     to be viewed.
    /// </summary>
    /// <returns>
    ///     <c>true</c>, if point should be plotted given current 
    ///     DrillDown settings, <c>false</c> otherwise.
    /// </returns>
    /// <param name="pos">
    ///     The spatial position of the point to test.
    /// </param>
    protected bool DrillDownValid(Vector3 pos)
    {
        if (pos.x <= XDrillMax
            && pos.x >= XDrillMin
            && pos.y <= YDrillMax
            && pos.y >= YDrillMin
            && pos.z <= ZDrillMax
            && pos.z >= ZDrillMin)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Determines what function to use for getting the position of a particle, based on which 
    ///     XYZ coordinates are actually valid at the time of calling this function.
    /// </summary>
    /// <returns>
    ///     A PositionGettingFunction that gets the most accurate position possible, given the current
    ///     set of XYZ coordinate indices.
    /// </returns>
    private PositionGettingFunction GetPositionGettingFunction()
    {
        int sum = 0;
        if (IsValidIndex(XSpatialFieldIndex))
        {
            sum += 1;
        }
        if (IsValidIndex(YSpatialFieldIndex))
        {
            sum += 2;
        }
        if (IsValidIndex(ZSpatialFieldIndex))
        {
            sum += 4;
        }

        switch (sum)
        {
            case 0: return GetPosition0;
            case 1: return GetPosition1;
            case 2: return GetPosition2;
            case 3: return GetPosition3;
            case 4: return GetPosition4;
            case 5: return GetPosition5;
            case 6: return GetPosition6;
            case 7: return GetPosition7;
            default:
                // This will never happen
                return GetPosition0;
        }
    }

    /// <summary>
    ///     Position function when none of the XYZ spatial coordinates are valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     MinLocation.
    /// </returns>
    private Vector3 GetPosition0(float[] dataPoint)
    {
        return Vector3.zero;
    }

    /// <summary>
    ///     Position function when only the X spatial coordinate is valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition1(float[] dataPoint)
    {
        return new Vector3(
            GetLinearInterpolationPercent(dataPoint[XSpatialFieldIndex],
                                          XSpatialFieldIndex),
            0,
            0);
    }

    /// <summary>
    ///     Position function when only the Y spatial coordinate is valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition2(float[] dataPoint)
    {
        return new Vector3(
            0,
                       GetLinearInterpolationPercent(dataPoint[YSpatialFieldIndex],
                                                     YSpatialFieldIndex),
            0);
    }

    /// <summary>
    ///     Position function when only the X and Y spatial coordinates are valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition3(float[] dataPoint)
    {
        return new Vector3(
            GetLinearInterpolationPercent(dataPoint[XSpatialFieldIndex],
                                          XSpatialFieldIndex),
            GetLinearInterpolationPercent(dataPoint[YSpatialFieldIndex],
                                          YSpatialFieldIndex),
            0);
    }

    /// <summary>
    ///     Position function when only the Z spatial cooridnate is valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition4(float[] dataPoint)
    {
        return new Vector3(
            0,
            0,
            GetLinearInterpolationPercent(dataPoint[ZSpatialFieldIndex],
                                          ZSpatialFieldIndex));
    }

    /// <summary>
    ///     Position function when only the X and Z spatial cooridnates are valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition5(float[] dataPoint)
    {
        return new Vector3(
            GetLinearInterpolationPercent(dataPoint[XSpatialFieldIndex],
                                          XSpatialFieldIndex),
            0,
            GetLinearInterpolationPercent(dataPoint[ZSpatialFieldIndex],
                                          ZSpatialFieldIndex));
    }

    /// <summary>
    ///     Position function when only the Y and Z spatial cooridnates are valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition6(float[] dataPoint)
    {
        return new Vector3(
            0,
            GetLinearInterpolationPercent(dataPoint[YSpatialFieldIndex],
                                          YSpatialFieldIndex),
            GetLinearInterpolationPercent(dataPoint[ZSpatialFieldIndex],
                                          ZSpatialFieldIndex));
    }

    /// <summary>
    ///     Position function when all of the spatial cooridnates are valid.
    /// </summary>
    /// <param name="dataPoint">
    ///     The point to get the position of.
    /// </param>
    /// <returns>
    ///     A Vector3, properly scaled to fit within the bounds of MinLocation and MaxLocation.
    /// </returns>
    private Vector3 GetPosition7(float[] dataPoint)
    {
        return new Vector3(
            GetLinearInterpolationPercent(dataPoint[XSpatialFieldIndex],
                                          XSpatialFieldIndex),
            GetLinearInterpolationPercent(dataPoint[YSpatialFieldIndex],
                                          YSpatialFieldIndex),
            GetLinearInterpolationPercent(dataPoint[ZSpatialFieldIndex],
                                          ZSpatialFieldIndex));
    }

    /// <summary>
    ///     A helper function for determining the color to assign to a data point, based on interpolating
    ///     between the min and max possible values for the color.
    /// </summary>
    /// <param name="dataPoint">
    ///     The data point that we want the color of.
    /// </param>
    /// <returns>
    ///     The color of the data point.
    /// </returns>
    private Color GetColor(float[] dataPoint)
    {
        if (IsColorEnabled && IsValidIndex(ColorFieldIndex))
        {
            float percent;
            if (IsColorLinearlyInterpolated)
            {
                percent = GetLinearInterpolationPercent(dataPoint[ColorFieldIndex], ColorFieldIndex);
            }
            else
            {
                percent = GetNonlinearInterpolationPercent(dataPoint[ColorFieldIndex], ColorFieldIndex);
            }
            return Color.Lerp(MinColor, MaxColor, percent);
        }
        return Color.white;
    }

    private Quaternion GetRotation(float[] dataPoint)
    {
        Vector3 direction = new Vector3();
        if (IsValidIndex(XArrowFieldIndex))
        {
            direction.x = dataPoint[XArrowFieldIndex];
        }
        if (IsValidIndex(YArrowFieldIndex))
        {
            direction.y = dataPoint[YArrowFieldIndex];
        }
        if (IsValidIndex(ZArrowFieldIndex))
        {
            direction.z = dataPoint[ZArrowFieldIndex];
        }

        Quaternion answer = new Quaternion();
        answer.SetFromToRotation(new Vector3(0, 1, 0), direction);
        return answer;
    }

    private Vector3 GetScale(float[] dataPoint)
    {
        if (IsSizeEnabled && IsValidIndex(SizeFieldIndex))
        {
            float percent;
            if (IsSizeLinearlyInterpolated)
            {
                percent = GetLinearInterpolationPercent(dataPoint[SizeFieldIndex], SizeFieldIndex);
            }
            else
            {
                percent = GetNonlinearInterpolationPercent(dataPoint[SizeFieldIndex], SizeFieldIndex);
            }
            float sizeFactor = (percent * (MaxSize - MinSize)) + MinSize;
            return new Vector3(sizeFactor / ArrowPointiness,
                               sizeFactor * ArrowPointiness,
                               sizeFactor / ArrowPointiness);
        }
        return new Vector3(ParticleSize / ArrowPointiness,
                           ParticleSize * ArrowPointiness,
                           ParticleSize / ArrowPointiness);
    }

    /// <summary>
    ///     A helper function for determining the size of a data point, based on interpolation between
    ///     the min and max possible values for the color.
    /// </summary>
    /// <param name="dataPoint">
    ///     The data point we want the size of.
    /// </param>
    /// <returns>
    ///     The size of the data point.
    /// </returns>
    private float GetSize(float[] dataPoint)
    {
        if (IsSizeEnabled && IsValidIndex(SizeFieldIndex))
        {
            float percent;
            if (IsSizeLinearlyInterpolated)
            {
                percent = GetLinearInterpolationPercent(dataPoint[SizeFieldIndex], SizeFieldIndex);
            }
            else
            {
                percent = GetNonlinearInterpolationPercent(dataPoint[SizeFieldIndex], SizeFieldIndex);
            }
            return (percent * (MaxSize - MinSize)) + MinSize;
        }
        return ParticleSize;
    }

    /// <summary>
    ///     A helper function that determines whether an integer may be used to index the 2nd dimension
    ///     of the Data array.
    /// </summary>
    /// <param name="index">
    ///     The index in the 2nd dimension of the Data array.
    /// </param>
    /// <returns>
    ///     True if the index may be used to index the array, and false otherwise.
    /// </returns>
    public bool IsValidIndex(int index)
    {
        if (Data == null || NumFeatures < 1 || index < 0 || index >= NumFeatures)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    ///     A helper function that gives percent used to interpolate between
    ///     minimum and max values of a column.  This function uses linear
    ///     interpolation, so the percent returned depends entirely on the 
    ///     value given to this function, and not other points' values.
    /// </summary>
    /// <param name="val">
    ///     The value that we want the interpolation percent for.
    /// </param>
    /// <param name="column">
    ///     The column in the data that the value belongs to.
    /// </param>
    /// <returns>
    ///     A float between 0 and 1 inclusive that denotes a percent.
    /// </returns>
    private float GetLinearInterpolationPercent(float val, int column)
    {
        float max = MaxDataValues[column];
        float min = MinDataValues[column];
        return (val - min) / (max - min);
    }

    /// <summary>
    ///     A helper function that gives percent used to interpolate between 
    ///     minimum and max values of a column.  This function uses non-linear
    ///     interpolation, so the percent returned depends on how many values
    ///     in the column are greater or less than the given value.
    /// </summary>
    /// <param name="val">
    ///     The value that we want the interpolation percent for.
    /// </param>
    /// <param name="column">
    ///     The column in the data that the value belongs to.
    /// </param>
    /// <returns>
    ///     A float between 0 and 1 inclusive that denotes a percent.
    /// </returns>
    private float GetNonlinearInterpolationPercent(float val, int column)
    {
        SetColumnRank(column);
        int rank = ColumnRanks[column].IndexOfKey(val);
        float answer = (float) rank / (float) (ColumnRanks[column].Count);
        //if (answer > .98 || answer < .02)
        //{
        //    Debug.Log("rank: " + rank);
        //    Debug.Log("NonLinearInterpolationPercent: " + answer);
        //}
        return answer;
    }
}
