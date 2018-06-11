using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayClickActionDrop : MonoBehaviour {

    public VRStandardAssets.Utils.VRInteractiveItem vRInteractiveItem; //reference to the the interactive item
    public Dropdown dropDown;


    void Start()
    {
        vRInteractiveItem.OnClick += DropSelect;
        vRInteractiveItem.OnOver += ReticleOver;
    }

    void ReticleOver()
    {
        print("I am Over a DropDown");
    }


    //TODO: Fix this
    void DropSelect()
    {
        if (dropDown.GetComponent<Canvas>())
        {
            dropDown.Hide();
        }
        else
        {
            dropDown.Show();
        }
        
    }
}


