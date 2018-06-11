using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayClickActionDropOp : MonoBehaviour
{

    public VRStandardAssets.Utils.VRInteractiveItem vRInteractiveItem; //reference to the the interactive item
    public Dropdown dropDown;
    //public Dropdown.OptionData dropDownOption;
    public Text dropDownOption;


    void Start()
    {
        vRInteractiveItem.OnClick += DropSelectOption;
        vRInteractiveItem.OnOver += ReticleOver;
    }

    void ReticleOver()
    {
        print("Well I am over a DropDown Menu item");
    }

    void DropSelectOption()
    {
        string valueStr = dropDownOption.text;
        List<Dropdown.OptionData> options = dropDown.options;
        for (int i = 0; i < options.Count; i++) {
            if (options[i].text == valueStr) {
                dropDown.value = i;
                dropDown.Hide();
                break;
            } 
        }
    }
}