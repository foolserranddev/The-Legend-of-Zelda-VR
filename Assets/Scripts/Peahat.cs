using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peahat : Enemy
{
    public GameObject propellor;
    public float rotateSpeed = 2000;
    public float riseSpeed = 1;
    public float speed = 8;
    public float maxHeight = 1.2f;
    public float fallDrag = 30f;

    private bool grounded = true;
    private bool rising = false;
    private bool falling;
    private Vector3 rot;
    private float switchTime;
    private float startHeight;
    private float zeroFallTime;
    private const float FallDuration = 1.5f;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        rot = new Vector3(0, 0, 1);
        switchTime = Time.time + 1;
        startHeight = transform.position.y;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (stunned || ClockStun) return;
        float percentUp = Mathf.Max((transform.position.y - startHeight), 0) / maxHeight;
        if (rising)
        {
            if (rigidBody.useGravity)
            {
                rigidBody.useGravity = false;
                rigidBody.drag = 0;
                rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
            propellor.transform.Rotate(rot * Mathf.Min(rotateSpeed * percentUp * Time.deltaTime, 1000));
            Vector3 pos = transform.position;
            pos.y = Mathf.Min((startHeight + maxHeight + 0.01f), pos.y + riseSpeed * Time.deltaTime);
            transform.position = pos;
            
            if (pos.y >= (startHeight + maxHeight))
            {
                rising = false;
                switchTime = Time.time + Random.Range(6f, 8f);
            }
        }
        else if (falling)
        {
            float timeLeft = Mathf.Max(0, (switchTime - Time.time) / FallDuration);
            propellor.transform.Rotate(rot * Mathf.Max(rotateSpeed * timeLeft * Time.deltaTime, 0));
            if (rigidBody.useGravity)
            {
                rigidBody.drag = 0f;
                rigidBody.useGravity = false;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                // Descend the authored max height over the authored fall
                // window. Collision still lets terrain and bushes stop us.
                rigidBody.velocity = Vector3.down * (maxHeight / FallDuration);
            }
            else if (Time.time >= switchTime)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                falling = false;
                invulnerable = false;
                isStunnable = true;
                grounded = true;
                switchTime = Time.time + Random.Range(2f, 3f);
            }
        }
        else
        {
            if (Time.time >= switchTime)
            {
                rising = grounded;
                falling = !rising;
                invulnerable = true;
                isStunnable = false;
                grounded = false;
                rigidBody.velocity = Vector3.zero;
                switchTime = Time.time + FallDuration; // Not used for rising.
                if (falling)
                {
                    rigidBody.drag = 0f;
                    rigidBody.useGravity = false;
                    rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                    rigidBody.velocity = Vector3.down * (maxHeight / FallDuration);
                }
            }
            else if (!grounded)
            {
                propellor.transform.Rotate(rot * rotateSpeed * Time.deltaTime);
                float x = transform.localPosition.x;
                float z = transform.localPosition.z;
                bool outOfBounds = ((x < MinX || x > MaxX) && MaxX > MinX) || ((z < MinZ || z > MaxZ) && MaxZ > MinZ);
                if (outOfBounds || Vector3.Distance(rigidBody.velocity, Vector3.zero) < 1)
                {
                    newDirection();
                }
            }
        }
    }

    private void newDirection()
    {
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        rigidBody.velocity = new Vector3(randX, 0, randZ);
    }

}
