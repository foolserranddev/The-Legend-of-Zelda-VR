using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour {

    public Vector3 rot = new Vector3(1f, 0.5f, 1f);
    public float timeToDisappear = 4f;
    public float bounce = 2;
    public float bounceRate = 0.5f;

    private float nextBounceTime;
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        nextBounceTime = Time.time;
        timeToDisappear += Time.time;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(rot * Time.deltaTime * 1000);
        Vector3 p = transform.localPosition;
        bool OutOfBounds = p.z > 16 || p.z < 0; //p.x > 16 || p.x < 0 ||
        if (Time.time > timeToDisappear || OutOfBounds) Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (Time.time > nextBounceTime)
        {
            rb.velocity += new Vector3(0, bounce, 0);
            nextBounceTime = Time.time + bounceRate;
        }
    }
}
