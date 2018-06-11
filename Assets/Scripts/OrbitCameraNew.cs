using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitCameraNew : MonoBehaviour
{

	private Transform cameraTransform;										// transform of the camera subobject for vertical rotation
	[SerializeField] private GameObject target = null;						// object to rotate around
	[SerializeField] [Range(10f, 1000f)] private float cameraSpeed = 100;	// rotation speed multiplier

	void Start()
	{
		// get the child transform
		cameraTransform = gameObject.transform.GetChild(0);
		Debug.Log(cameraTransform.name);

		// make the camera face the target object
		if (target != null)
		{
			cameraTransform.LookAt(target.transform);
		}
	}

	void Update()
	{
		float horiz = Input.GetAxisRaw("Horizontal");
		float vert = Input.GetAxisRaw("Vertical");
		//transform.RotateAround(	target.transform.position,
		//						new Vector3(0, horiz, 0),
		//						Time.deltaTime * cameraSpeed );
		//cameraTransform.RotateAround(	target.transform.position,
		//								new Vector3(vert, 0, 0),
		//								Time.deltaTime * cameraSpeed );
		transform.RotateAround(target.transform.position,
						transform.up,
						Time.deltaTime * cameraSpeed * horiz);
		cameraTransform.RotateAround(target.transform.position,
										cameraTransform.right,
										Time.deltaTime * cameraSpeed * vert);
	}
}
