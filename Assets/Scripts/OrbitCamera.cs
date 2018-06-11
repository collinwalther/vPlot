using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
	[SerializeField] private GameObject target = null;
	[SerializeField] [Range(1f, 100f)] private float cameraSpeed = 50f;
	[SerializeField] [Range(1f, 100f)] private float radialSpeed = 1f;
	[SerializeField] private float radialMinDistance = 0.1f;

	void Start()
	{
		// make the camera face the target object
		if (target != null)
		{
			transform.LookAt(target.transform);
		}
	}
	
	void Update()
	{
		// make the camera rotate around the target object at a speed
		// corresponding to the controller input
		moveTangentially();
		moveRadially();		

	}

	/// <summary>
	/// Revolve the camera about the target based on user input.
	/// </summary>
	/// <remarks>
	/// To be used only in Update()
	/// </remarks>
	void moveTangentially()
	{
		float horiz = Input.GetAxis("Horizontal");
		float vert = Input.GetAxis("Vertical");

		transform.RotateAround( target.transform.position,
								Vector3.up,
								Time.deltaTime * cameraSpeed * horiz );

		transform.RotateAround( target.transform.position,
								transform.right,
								Time.deltaTime * cameraSpeed * vert );
	}

	/// <summary>
	/// Move the camera towards or away from the target based on user input.
	/// </summary>
	/// <remarks>
	/// To be used only in Update()
	/// </remarks>
	void moveRadially()
	{
        float axisIn = Input.GetAxis("ZoomIn");
        float axisOut = Input.GetAxis("ZoomOut");

        // Note:	it is necessary to keep the camera and target transforms from occupying the same space,
        //			or else direction is lost and the camera cannot be moved backwards.

        //print("radial axis value: " + axis);

		// if the camera is too close, only permit backwards motion
		if ((axisIn - axisOut) > 0 && Vector3.Distance(transform.position, target.transform.position) < radialMinDistance)
			return;
        if (axisIn - axisOut > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                                                  target.transform.position,
                                                  Time.deltaTime * radialSpeed * axisIn);
        }
        else if (axisOut - axisIn > 0)
        {
            axisOut = -axisOut;
            transform.position = Vector3.MoveTowards(transform.position,
                                                  target.transform.position,
                                                  Time.deltaTime * radialSpeed * axisOut);
        }
	}
}
