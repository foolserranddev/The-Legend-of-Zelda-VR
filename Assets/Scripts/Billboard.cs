using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public bool RotateOnX = false;
    public bool RotateOnY = true;
    public bool RotateOnZ = false;
    public Vector3 rotationOffset = Vector3.zero;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Camera.main == null) return;
        Vector3 v = Camera.main.transform.position - transform.position;
        if (!RotateOnX) v.x = 0f;
        if (!RotateOnY) v.y = 0f;
        if (!RotateOnZ) v.z = 0f;
        transform.LookAt(Camera.main.transform.position - v);
        transform.Rotate(rotationOffset);
    }
}
