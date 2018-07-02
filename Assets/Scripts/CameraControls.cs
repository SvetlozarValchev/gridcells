using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {
    Vector3 cameraPosition;
    Vector3 mouseOrigin;

    bool isPanning = false;
    float panSpeed = 20f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            cameraPosition = transform.position;
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Camera.main.transform.position = cameraPosition + -pos * panSpeed;
        }
    }
}
