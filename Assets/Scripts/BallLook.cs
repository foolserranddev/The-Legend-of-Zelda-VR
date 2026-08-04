using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLook : MonoBehaviour {

    public bool up;
    public bool right;
    public bool forward;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 v = transform.position;
        if (up)
        {
            v += Vector3.up;
        }
        else if (right)
        {
            v += Vector3.right;
        }
        else if (forward)
        {
            v += Vector3.forward;
        }
        transform.LookAt(v);
	}
}
