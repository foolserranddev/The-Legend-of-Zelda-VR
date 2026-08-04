using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAnimation : MonoBehaviour {

    public float DestroyTime;

    private float endTime;

	// Use this for initialization
	void Start () {
        endTime = Time.time + DestroyTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(this.gameObject);
        }
    }
}
