using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class PlotSettings : MonoBehaviour
{
    protected Plot plot;
    private Dropdown xDrop;
    private Dropdown yDrop;
    private Dropdown zDrop;
    private Dropdown xArrowDrop;
    private Dropdown yArrowDrop;
    private Dropdown zArrowDrop;
    private Dropdown sizeDrop;
    private Dropdown colorDrop;
    private Dropdown minColorDrop;
    private Dropdown maxColorDrop;
    private Toggle sizeToggle;
    private Toggle sizeInterpolationToggle;
    private Toggle colorToggle;
    private Toggle colorInterpolationToggle;
    private Slider minPointSlider;
    private Slider maxPointSlider;
    private Slider sampleRateSlider;
    private Slider minXDrillSlider;
    private Slider maxXDrillSlider;
    private Slider minYDrillSlider;
    private Slider maxYDrillSlider;
    private Slider minZDrillSlider;
    private Slider maxZDrillSlider;

    protected bool InitialSettingsPopulated { get; set; }

    public virtual void Start()
    {
        InitialSettingsPopulated = false;
        GetReferencesToComponents();
        AttachEventHandlers();
        SetSliderValues();
    }

    protected virtual void GetReferencesToComponents()
    {
        xDrop = GameObject.Find("XColumnNumber").GetComponentInChildren<Dropdown>();
        yDrop = GameObject.Find("YColumnNumber").GetComponentInChildren<Dropdown>();
        zDrop = GameObject.Find("ZColumnNumber").GetComponentInChildren<Dropdown>();
        xArrowDrop = GameObject.Find("XArrowColumnNumber").GetComponentInChildren<Dropdown>();
        yArrowDrop = GameObject.Find("YArrowColumnNumber").GetComponentInChildren<Dropdown>();
        zArrowDrop = GameObject.Find("ZArrowColumnNumber").GetComponentInChildren<Dropdown>();
        sizeDrop = GameObject.Find("SizeColumnNumber").GetComponentInChildren<Dropdown>();
        colorDrop = GameObject.Find("ColorColumnNumber").GetComponentInChildren<Dropdown>();
        minColorDrop = GameObject.Find("MinColor").GetComponentInChildren<Dropdown>();
        maxColorDrop = GameObject.Find("MaxColor").GetComponentInChildren<Dropdown>();
        sizeToggle = GameObject.Find("SizeEnabled").GetComponentInChildren<Toggle>();
        sizeInterpolationToggle = GameObject.Find("SizeLinearlyInterpolated").GetComponentInChildren<Toggle>();
        colorToggle = GameObject.Find("ColorEnabled").GetComponentInChildren<Toggle>();
        colorInterpolationToggle = GameObject.Find("ColorLinearlyInterpolated").GetComponentInChildren<Toggle>();
        minPointSlider = GameObject.Find("MinParticleSize").GetComponentInChildren<Slider>();
        maxPointSlider = GameObject.Find("MaxParticleSize").GetComponentInChildren<Slider>();
        sampleRateSlider = GameObject.Find("SampleRate").GetComponentInChildren<Slider>();
        minXDrillSlider = GameObject.Find("MinXDrill").GetComponentInChildren<Slider>();
        maxXDrillSlider = GameObject.Find("MaxXDrill").GetComponentInChildren<Slider>();
        minYDrillSlider = GameObject.Find("MinYDrill").GetComponentInChildren<Slider>();
        maxYDrillSlider = GameObject.Find("MaxYDrill").GetComponentInChildren<Slider>();
        minZDrillSlider = GameObject.Find("MinZDrill").GetComponentInChildren<Slider>();
        maxZDrillSlider = GameObject.Find("MaxZDrill").GetComponentInChildren<Slider>();
    }

    protected virtual void AttachEventHandlers()
    {
        xDrop.onValueChanged.AddListener(plot.HandleXColumnNumberChange);
        yDrop.onValueChanged.AddListener(plot.HandleYColumnNumberChange);
        zDrop.onValueChanged.AddListener(plot.HandleZColumnNumberChange);
        xArrowDrop.onValueChanged.AddListener(plot.HandleXArrowColumnChanged);
        yArrowDrop.onValueChanged.AddListener(plot.HandleYArrowColumnChanged);
        zArrowDrop.onValueChanged.AddListener(plot.HandleZArrowColumnChanged);
        sizeDrop.onValueChanged.AddListener(plot.HandleSizeColumnNumberChange);
        colorDrop.onValueChanged.AddListener(plot.HandleColorColumnNumberChange);
        minColorDrop.onValueChanged.AddListener(plot.HandleMinColorChange);
        maxColorDrop.onValueChanged.AddListener(plot.HandleMaxColorChange);
        sizeToggle.onValueChanged.AddListener(plot.HandleSizeEnabledChange);
        sizeInterpolationToggle.onValueChanged.AddListener(plot.HandleSizeLinearlyInterpolatedChange);
        colorToggle.onValueChanged.AddListener(plot.HandleColorEnabledChange);
        colorInterpolationToggle.onValueChanged.AddListener(plot.HandleColorLinearlyInterpolatedChange);
        minPointSlider.onValueChanged.AddListener(plot.HandleMinParticleSizeChange);
        maxPointSlider.onValueChanged.AddListener(plot.HandleMaxParticleSizeChange);
        sampleRateSlider.onValueChanged.AddListener(plot.HandleSampleProbabilityPercentChanged);
        minXDrillSlider.onValueChanged.AddListener(plot.HandleMinXDrillDownChanged);
        maxXDrillSlider.onValueChanged.AddListener(plot.HandleMaxXDrillDownChanged);
        minYDrillSlider.onValueChanged.AddListener(plot.HandleMinYDrillDownChanged);
        maxYDrillSlider.onValueChanged.AddListener(plot.HandleMaxYDrillDownChanged);
        minZDrillSlider.onValueChanged.AddListener(plot.HandleMinZDrillDownChanged);
        maxZDrillSlider.onValueChanged.AddListener(plot.HandleMaxZDrillDownChanged);
    }

    protected virtual void SetSliderValues()
    {
        minPointSlider.minValue = 0f;
        minPointSlider.maxValue = .05f;
        maxPointSlider.minValue = 0f;
        maxPointSlider.maxValue = .05f;
        sampleRateSlider.maxValue = 100f;
    }

    public virtual void Update()
    {
        if (!InitialSettingsPopulated && plot.ColumnTitlesAreReady)
        {
            string[] columnTitles = plot.ColumnTitles;
            PopulateOptions(columnTitles);
        }
        if (InitialSettingsPopulated)
        {
            UpdateActiveOptions();
        }
    }

    protected virtual void PopulateOptions(string[] columnTitles)
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        if (columnTitles == null)
        {
            Dropdown.OptionData option = new Dropdown.OptionData("No options available");
            options.Add(option);
        }
        else
        {
            foreach (string title in columnTitles)
            {
                Dropdown.OptionData option = new Dropdown.OptionData(title);
                options.Add(option);
            }

            Dropdown.OptionData emptyOption = new Dropdown.OptionData("None");
            options.Add(emptyOption);
        }

        xDrop.options = options;
        yDrop.options = options;
        zDrop.options = options;
        xArrowDrop.options = options;
        yArrowDrop.options = options;
        zArrowDrop.options = options;
        sizeDrop.options = options;
        colorDrop.options = options;

        InitialSettingsPopulated = true;
    }

    protected virtual void UpdateActiveOptions()
    {
        xDrop.value = plot.Data.XSpatialFieldIndex;
        yDrop.value = plot.Data.YSpatialFieldIndex;
        zDrop.value = plot.Data.ZSpatialFieldIndex;
        xArrowDrop.value = plot.Data.XArrowFieldIndex;
        yArrowDrop.value = plot.Data.YArrowFieldIndex;
        zArrowDrop.value = plot.Data.ZArrowFieldIndex;
        sizeDrop.value = plot.Data.SizeFieldIndex;
        colorDrop.value = plot.Data.ColorFieldIndex;
        minColorDrop.value = Plot.ColorToDropdownValue(plot.Data.MinColor);
        maxColorDrop.value = Plot.ColorToDropdownValue(plot.Data.MaxColor);
        sizeToggle.isOn = plot.Data.IsSizeEnabled;
        sizeInterpolationToggle.isOn = plot.Data.IsSizeLinearlyInterpolated;
        colorToggle.isOn = plot.Data.IsColorEnabled;
        colorInterpolationToggle.isOn = plot.Data.IsColorLinearlyInterpolated;
        minPointSlider.value = plot.Data.MinSize;
        maxPointSlider.value = plot.Data.MaxSize;
        sampleRateSlider.value = plot.Data.SampleRatePercent;
        minXDrillSlider.value = plot.Data.XDrillMin;
        maxXDrillSlider.value = plot.Data.XDrillMax;
        minYDrillSlider.value = plot.Data.YDrillMin;
        maxYDrillSlider.value = plot.Data.YDrillMax;
        minZDrillSlider.value = plot.Data.ZDrillMin;
        maxZDrillSlider.value = plot.Data.ZDrillMax;
    }
}
