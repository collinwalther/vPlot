using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTest : MonoBehaviour {
   

    // Use this for initialization
    void Start () {
        string[] a = Input.GetJoystickNames();
        print("I have started");
        for (int i = 0; i < a.Length; i++) {
            print("Joystick: " + a[i]);
        }
        print("Listed all joysticks");
	}
	
	// Update is called once per frame
	void Update () {
        float value;
        if (Input.GetKeyDown(KeyCode.JoystickButton0)) {
            value = Input.GetAxis("Horizontal");
            print("JoystickButton0 was pressed: " + value.ToString());
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1)) {
            value = Input.GetAxis("Horizontal");
            print("JoystickButton1 was pressed: " + value.ToString());
        }
	}
}
