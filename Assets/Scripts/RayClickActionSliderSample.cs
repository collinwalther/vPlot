using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RayClickActionSliderSample : MonoBehaviour
{

    public VRStandardAssets.Utils.VRInteractiveItem vRInteractiveItem; //reference to the the interactive item
    //public Slider mSlider;
    public Slider mSlider;
    //public VRStandardAssets.Utils.Reticle mReticle;

    bool activeSlider;

    // Use this for initialization
    void Start()
    {
        vRInteractiveItem.OnClick += SliderClick;
        //Don't know if I need this next line
        EventSystem.current.sendNavigationEvents = false;
    }

    void SliderClick()
    {
        activeSlider = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Fire1"))
        {
            activeSlider = false;
        }
        if (activeSlider)
        {
            mSlider.normalizedValue = mSlider.normalizedValue + (Input.GetAxis("Horizontal") * 0.05F);
            Debug.LogError(mSlider.name + " " + mSlider.normalizedValue + " " + mSlider.value);
        }
    }
}
