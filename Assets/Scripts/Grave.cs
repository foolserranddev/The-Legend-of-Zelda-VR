using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour {

    public Ghini ghiniMaster;
    private float touchTime;

	// Use this for initialization
	void Start () {
        touchTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void trigger(Collider col)
    {
        if (col.tag == "Player" && Time.time > touchTime)
        {
            touchTime = Time.time + 2;
            ghiniMaster.AddGhost(transform.position);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        trigger(col);
    }

    private void OnTriggerStay(Collider col)
    {
        trigger(col);
    }

    private void OnCollisionEnter(Collision collision)
    {
        trigger(collision.collider);
    }
}
