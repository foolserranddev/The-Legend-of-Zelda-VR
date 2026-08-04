using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keese : Enemy {

    public float speed = 3;
    public float minSpeed = 0;
    public float minMoveTime = 2f;
    public float maxMoveTime = 4f;
    public float maxPeakHeightFromStart = 0.5f;
    private float moveTime;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (stunned || ClockStun) return;
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        if ((Time.time > moveTime || x < MinX || x > MaxX || z < MinZ || z > MaxZ) || rigidBody.velocity.magnitude < minSpeed)
        {
            newDirection();
        }
        else
        {
            //Vector3 vel = rigidBody.velocity;
            //vel.y = transform.position.y > maxY ? -(Mathf.Abs(Mathf.Sin(Time.time * 5) / 0.5f)) :
            //        transform.position.y < minY ? Mathf.Abs(Mathf.Sin(Time.time * 5) / 0.5f) :
            //        Mathf.Sin((Time.time + randomOffset) * 5) / 0.5f;
            //rigidBody.velocity = vel;
            Vector3 look = rigidBody.velocity + transform.position;
            look.y = transform.position.y;
            transform.LookAt(look);
        }

    }

    private void newDirection()
    {
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        Vector3 vel = new Vector3(randX, Mathf.Sin(Time.time*5)/5, randZ);
        rigidBody.velocity = vel * (speed / Vector3.Distance(vel, Vector3.zero));
        moveTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
    }
}
