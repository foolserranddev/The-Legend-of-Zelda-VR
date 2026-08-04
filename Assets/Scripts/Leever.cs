using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leever : Enemy {

    public float rotateSpeed = 1;
    public float riseSpeed = 1;
    public float speed;
    public bool MoveTowardPlayer = false;
    public bool verbose = false;

    private bool underground = false;
    private bool rising = false;
    private bool falling = false;
    private Vector3 rot;
    private float switchTime;
    private float switchDirection;
    private float startHeight;
    private SphereCollider [] sphereColliders;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        rot = new Vector3(0, 1, 0);
        switchDirection = Time.time;
        startHeight = transform.position.y;
        sphereColliders = GetComponents<SphereCollider>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        switchTime = Time.time + Random.Range(0f, 1f);
    }
    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (stunned || ClockStun) return;
        if (rising)
        {
            if (myQuadrant != Player.player.quadrant)
            {
                rising = false;
                falling = true;
                switchTime = Time.time + Random.Range(0f, 2f);
                return;
            }
            transform.Rotate(rot * Time.deltaTime * 1000);
            Vector3 pos = transform.position;
            pos.y = Mathf.Min(startHeight, pos.y + riseSpeed * Time.deltaTime);
            transform.position = pos;
            if (pos.y >= startHeight)
            {
                rising = false;
                sphereColliders[1].enabled = false;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            }
            else if (pos.y >= startHeight - 0.4f)
            {
                invulnerable = false;
                underground = false;
                transform.tag = "Enemy";
            }
        }
        else if (falling)
        {
            transform.Rotate(rot * Time.deltaTime * 1000);
            Vector3 pos = transform.position;
            pos.y = Mathf.Max(startHeight - 1.05f, pos.y - riseSpeed * Time.deltaTime);
            transform.position = pos;
            if (pos.y <= startHeight - 1f)
            {
                falling = false;
                sphereColliders[0].enabled = false;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            }
            else if (pos.y <= startHeight - 0.4f)
            {
                invulnerable = true;
                underground = true;
                transform.tag = "InvisibleCollider";
            }
        }
        else
        {
            transform.Rotate(rot * Time.deltaTime * 400);
            if (Time.time >= switchTime)
            {
                switchTime = Time.time + Random.Range(3f, 5f);
                falling = !underground;
                rising = !falling;
                rigidBody.velocity = Vector3.zero;
                sphereColliders[0].enabled = true;
                sphereColliders[1].enabled = true;
                rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            }
            float x = transform.localPosition.x;
            float z = transform.localPosition.z;
            bool outOfBounds = ((x < MinX || x > MaxX) && MaxX > MinX) || ((z < MinZ || z > MaxZ) && MaxZ > MinZ);
            if ((Time.time > switchDirection) || outOfBounds || Vector3.Distance(rigidBody.velocity, Vector3.zero) < 1)
            {
                if (!outOfBounds) switchDirection = Time.time + 4;
                newDirection(outOfBounds);
            }
        }
    }

    private void newDirection(bool outOfBounds)
    {
        if (!outOfBounds && !underground && MoveTowardPlayer && myQuadrant == Player.player.quadrant)
        {
            Vector3 look = Camera.main.transform.position;
            look.y = transform.position.y;
            transform.LookAt(look);
            rigidBody.velocity = transform.forward * speed;
        }
        else
        {
            float x = transform.localPosition.x;
            float z = transform.localPosition.z;
            float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
            float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
            rigidBody.velocity = new Vector3(randX, 0, randZ);
        }
    }


}
