using UnityEngine;

// This script should be applied to a particle system.
public abstract class Plot : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject[] lastArrows;

    /// <summary>
    ///     Contains the actual data that the plot will display.
    /// </summary>
    public PlottableData Data { get; set; }

    /// <summary>
    ///     Determines whether the names identifying each feature of the data
    ///     are available and ready to be read.
    /// </summary>
    public bool ColumnTitlesAreReady { get; set; }

    /// <summary>
    ///     The names identifying each feature of data.  May be null; be sure
    ///     to check ColumnTitlesAreReady before using this property to
    ///     ensure that you are getting valid data.
    /// </summary>
    public string[] ColumnTitles { get; set; }

    /// <summary>
    ///     Basic initializations when instantiating a plot object.  This
    ///     function only sets the Data property; any inheriting class should
    ///     properly override this function.
    /// </summary>
    public virtual void Start()
    {
        Data = gameObject.AddComponent<PlottableData>() as PlottableData;
        ColumnTitlesAreReady = false;
        InitializeDefaultDataProperties();
    }

    /// <summary>
    ///     Actions to execute every frame.  May be implemented by
    ///     child classes.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    ///     Draws the particles stored in this object.
    /// </summary>
    public virtual void Draw()
    {
        if (!Data.IsArrowEnabled)
        {
            if (lastArrows != null)
            {
                foreach (GameObject arrow in lastArrows)
                {
                    GameObject.Destroy(arrow);
                }
            }
            ParticleSystem.Particle[] particles = Data.GetParticles();
            if (particles == null)
            {
                return;
            }
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].position = new Vector3(particles[i].position.x - .5f,
                    particles[i].position.y - .5f,
                    particles[i].position.z - .5f);
            }
            GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
        }
        else
        {
            GameObject[] arrows = Data.GetArrows();
            if (arrows == null)
            {
                Debug.Log("Couldn't get arrows");
                return;
            }

            GetComponent<ParticleSystem>().SetParticles(null, 0);
            foreach (GameObject arrow in arrows)
            {
                arrow.transform.position = new Vector3(arrow.transform.position.x - .5f,
                    arrow.transform.position.y - .5f,
                    arrow.transform.position.z - .5f);
                arrow.SetActive(true);
            }

            lastArrows = arrows;
        }
    }

    /// <summary>
    ///     Initializes settings that control how data is initially visualized.
    ///     These settings are just sensible defaults, and will likely be changed
    ///     by the user over the course of the program's operation.
    /// </summary>
    protected virtual void InitializeDefaultDataProperties()
    {
        Data.SampleRatePercent = 100f;
        Data.XSpatialFieldIndex = 0;
        Data.YSpatialFieldIndex = 1;
        Data.ZSpatialFieldIndex = 2;
        Data.IsSizeEnabled = false;
        Data.SizeFieldIndex = 3;
        Data.ParticleSize = .01f;
        Data.MinSize = .001f;
        Data.MaxSize = .01f;
        Data.IsSizeLinearlyInterpolated = true;
        Data.IsColorEnabled = true;
        Data.ColorFieldIndex = 4;
        Data.MinColor = Color.green;
        Data.MaxColor = Color.red;
        Data.IsColorLinearlyInterpolated = true;
        Data.arrowPrefab = arrowPrefab;
        Data.IsArrowEnabled = false;
    }

    /// <summary>
    ///     Inverse function of Plot.ColorToDropdownValue.
    ///     A helper function that converts a value retrieved from a
    ///     dropdown menu (a positive integer, where the value corresponds
    ///     to its ordinal position in the dropdown) to a color.  Used
    ///     when communicating colors via the UI.
    /// </summary>
    /// <returns>
    ///     The color that corresponds to the value given as a parameter.
    /// </returns>
    /// <param name="val">
    ///     The value retrieved from the dropdown.  Should be on the
    ///     interval [0, 7] inclusive.
    /// </param>
    public static Color DropdownValueToColor(int val)
    {
        switch (val)
        {
            case 0: return Color.black;
            case 1: return Color.blue;
            case 2: return Color.yellow;
            case 3: return Color.green;
            case 4: return Color.red;
            case 5: return Color.magenta;
            case 6: return Color.cyan;
            case 7: return Color.white;
            default: return Color.white;
        }
    }

    /// <summary>
    ///     Inverse function of Plot.DropdownColorToValue.  A helper
    ///     function that converts a color to the corresponding value
    ///     in a dropdown menu in the settings UI.
    /// </summary>
    /// <returns>
    ///     The dropdown value that corresponds to the color given as a
    ///     parameter.
    /// </returns>
    /// <param name="val">
    ///     A color that may be selected via the dropdown.  May be one of
    ///     the unity-standard colors black, blue, yellow, green, red,
    ///     magenta, cyan, or white.
    /// </param>
    public static int ColorToDropdownValue(Color val)
    {
        if (val == Color.black)
        {
            return 0;
        }
        else if (val == Color.blue)
        {
            return 1;
        }
        else if (val == Color.yellow)
        {
            return 2;
        }
        else if (val == Color.green)
        {
            return 3;
        }
        else if (val == Color.red)
        {
            return 4;
        }
        else if (val == Color.magenta)
        {
            return 5;
        }
        else if (val == Color.cyan)
        {
            return 6;
        }
        else if (val == Color.white)
        {
            return 7;
        }
        else
        {
            return 7;
        }
    }

    /// <summary>
    ///     Event handler for when the number of the field x
    ///     corresponds to changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleXColumnNumberChange(int col)
    {
        Data.XSpatialFieldIndex = col;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the number of the field y
    ///     corresponds to changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleYColumnNumberChange(int col)
    {
        Data.YSpatialFieldIndex = col;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the number of the field z
    ///     corresponds to changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleZColumnNumberChange(int col)
    {
        Data.ZSpatialFieldIndex = col;
        Draw();
    }

    /// <summary>
    ///     Event handler for when size being enabled is toggled.
    /// </summary>
    /// <param name="col">
    ///     Whether it's enabled or not.
    /// </param>
    public void HandleSizeEnabledChange(bool enabled)
    {
        Data.IsSizeEnabled = enabled;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the number of the field size
    ///     corresponds to changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleSizeColumnNumberChange(int col)
    {
        Data.SizeFieldIndex = col;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the minimum size of the
    ///     particles changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleMinParticleSizeChange(float val)
    {
        Data.MinSize = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the maximum size of the
    ///     particles changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleMaxParticleSizeChange(float val)
    {
        Data.MaxSize = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when linear size interpolation
    ///     is toggled.
    /// </summary>
    /// <param name="col">
    ///     Whether it's enabled or not.
    /// </param>
    public void HandleSizeLinearlyInterpolatedChange(bool enabled)
    {
        Data.IsSizeLinearlyInterpolated = enabled;
        Draw();
    }

    /// <summary>
    ///     Event handler for when color being enabled is toggled.
    /// </summary>
    /// <param name="col">
    ///     Whether it's enabled or not.
    /// </param>
    public void HandleColorEnabledChange(bool enabled)
    {
        Data.IsColorEnabled = enabled;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the number of the field color
    ///     corresponds to changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleColorColumnNumberChange(int col)
    {
        Data.ColorFieldIndex = col;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the minimum particle color
    ///     changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleMinColorChange(int val)
    {
        Data.MinColor = DropdownValueToColor(val);
        Draw();
    }

    /// <summary>
    ///     Event handler for when the maximum particle color
    ///     changes.
    /// </summary>
    /// <param name="col">
    ///     The new field.
    /// </param>
    public void HandleMaxColorChange(int val)
    {
        Data.MaxColor = DropdownValueToColor(val);
        Draw();
    }

    /// <summary>
    ///     Event handler for when linear color interpolation
    ///     is toggled.
    /// </summary>
    /// <param name="enabled">
    ///     Whether it's enabled or not.
    /// </param>
    public void HandleColorLinearlyInterpolatedChange(bool enabled)
    {
        Data.IsColorLinearlyInterpolated = enabled;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the sample rate percent is changed.
    /// </summary>
    /// <param name="val">
    ///     The new sample rate percent.
    /// </param>
    public void HandleSampleProbabilityPercentChanged(float val)
    {
        Data.SampleRatePercent = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the minimum X Drill Down is changed.
    /// </summary>
    /// <param name="val">
    ///     The new minimum X Drill Down value.
    /// </param>
    public void HandleMinXDrillDownChanged(float val)
    {
        Data.XDrillMin = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the maximum X Drill Down is changed.
    /// </summary>
    /// <param name="val">
    ///     The new maximum X Drill Down value.
    /// </param>
    public void HandleMaxXDrillDownChanged(float val)
    {
        Data.XDrillMax = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the minimum Y Drill Down is changed.
    /// </summary>
    /// <param name="val">
    ///     The new minimum Y Drill Down value.
    /// </param>
    public void HandleMinYDrillDownChanged(float val)
    {
        Data.YDrillMin = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the maximum Y Drill Down is changed.
    /// </summary>
    /// <param name="val">
    ///     The new maximum Y Drill Down value.
    /// </param>
    public void HandleMaxYDrillDownChanged(float val)
    {
        Data.YDrillMax = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the minimum Z Drill Down is changed.
    /// </summary>
    /// <param name="val">
    ///     The new minimum Z Drill Down value.
    /// </param>
    public void HandleMinZDrillDownChanged(float val)
    {
        Data.ZDrillMin = val;
        Draw();
    }

    /// <summary>
    ///     Event handler for when the maximum Z Drill Down is changed.
    /// </summary>
    /// <param name="val">
    ///     The new maximum Z Drill Down value.
    /// </param>
    public void HandleMaxZDrillDownChanged(float val)
    {
        Data.ZDrillMax = val;
        Draw();
    }

    public void HandleArrowsEnabledChange(bool enabled)
    {
        Data.IsArrowEnabled = enabled;
        Draw();
    }

    public void HandleXArrowColumnChanged(int col)
    {
        Data.XArrowFieldIndex = col;
        Draw();
    }

    public void HandleYArrowColumnChanged(int col)
    {
        Data.YArrowFieldIndex = col;
        Draw();
    }

    public void HandleZArrowColumnChanged(int col)
    {
        Data.ZArrowFieldIndex = col;
        Draw();
    }
}
