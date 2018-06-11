using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayClickActionCheck : MonoBehaviour
{

    public VRStandardAssets.Utils.VRInteractiveItem vRInteractiveItem; //reference to the the interactive item
    public Toggle mToggle;

    // Use this for initialization
    void Start()
    {
        vRInteractiveItem.OnClick += ToggleSwitch;
        vRInteractiveItem.OnOver += OverTest;
    }

    // Update is called once per frame
    void ToggleSwitch()
    {
        if (mToggle.isOn == true)
        {
            mToggle.isOn = false;
        }
        else
        {
            mToggle.isOn = true;
        }
    }

    void OverTest() {
        //print("This boy went over");
    }
}
