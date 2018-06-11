using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class brings up the menu and locks movement of the plot
public class SettingsMenu : MonoBehaviour {

    public GameObject cSVPlotSettings;
    public Camera mCamera;
    public GameObject reticle;

    private OrbitCamera orbitCamera;

    // Use this for initialization
	void Start () {
        orbitCamera = mCamera.GetComponentInParent<OrbitCamera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.JoystickButton3)) {
            if (!cSVPlotSettings.activeSelf) {
                cSVPlotSettings.SetActive(true);
                reticle.SetActive(true);
                cSVPlotSettings.transform.position = mCamera.transform.position + (mCamera.transform.forward * 10);
                cSVPlotSettings.transform.LookAt(mCamera.transform.position, mCamera.transform.up);
                cSVPlotSettings.transform.Rotate(new Vector3(0,180,0));
                orbitCamera.enabled = false;
            } else {
                reticle.SetActive(false);
                cSVPlotSettings.SetActive(false);
                orbitCamera.enabled = true;
            }
        }
	}
}
