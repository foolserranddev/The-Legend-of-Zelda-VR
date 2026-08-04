using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digdogger : MovingShooter
{
    [Header("Digdogger Specific")]

    public float rotationSpeed = 1;
    public Digdogger parent = null;

	// Use this for initialization
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();
        //rigidBody.angularVelocity = new Vector3(0, 0, rotationSpeed);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
	}

    public void OnDisable()
    {
        if (parent != null)
        {
            Destroy(gameObject);
        }
    }

    public override void Die(string colliderTag)
    {
        if (parent == null)
        {
            base.Die(colliderTag);
        }
        else
        {
            Enemy [] enemies = transform.parent.GetComponentsInChildren<Enemy>();
            foreach (Enemy e in enemies)
            {
                if (e != this && e.gameObject.activeSelf)
                {
                    base.Die(colliderTag);
                    return;
                }
            }
            parent.transform.position = transform.position;
            parent.Die(colliderTag);
            base.Die(colliderTag);
        }
    }
}
