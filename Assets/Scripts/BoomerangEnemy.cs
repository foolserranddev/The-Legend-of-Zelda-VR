using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangEnemy : Projectile {

    public float maxDistance = 5f;
    public float rotateSpeed = 1000;
    public Vector3 rot = new Vector3(0, 1, 0);

    private bool returning;
    private float startY;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        startY = transform.position.y;
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        if (shooter != null && !shooter.gameObject.activeSelf) Destroy(gameObject);
        if (returning)
        {
            if (Vector3.Distance(transform.position, shooter.transform.position) < 1.2f)
            {
                Destroy(gameObject);
            }
            else
            {
                Vector3 v = shooter.transform.position;
                v.y = startY;
                rigidBody.velocity = (v - transform.position).normalized * ShootSpeed;
            }
        }
		else if (shooter != null && Vector3.Distance(transform.position, shooter.transform.position) > maxDistance)
        {
            returning = true;
        }
        transform.Rotate(rot * rotateSpeed * Time.deltaTime);
    }

    public override void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == shooter)
        {
            Destroy(gameObject);
        }
        else if (col.collider.tag == "Wall")
        {
            returning = true;
            GetComponent<Collider>().isTrigger = true;
        }
    }
}
