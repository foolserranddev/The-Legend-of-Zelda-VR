using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordImpact : MonoBehaviour {

    public GameObject tl;
    public GameObject tr;
    public GameObject bl;
    public GameObject br;
    public bool billboard = true;

    public float speed;
    public float endTime;

	// Use this for initialization
	void Start ()
    {
        endTime += Time.time;
        if (billboard) transform.LookAt(Camera.main.transform.position);
    }
	
	// Update is called once per frame
	void Update ()
    {
        tl.transform.localPosition += new Vector3(0.1f, 0.1f, 0) * Time.deltaTime * speed;
        tr.transform.localPosition += new Vector3(-0.1f, 0.1f, 0) * Time.deltaTime * speed;
        bl.transform.localPosition += new Vector3(0.1f, -0.1f, 0) * Time.deltaTime * speed;
        br.transform.localPosition += new Vector3(-0.1f, -0.1f, 0) * Time.deltaTime * speed;

        if (Time.time > endTime) Destroy(gameObject);
    }
}
