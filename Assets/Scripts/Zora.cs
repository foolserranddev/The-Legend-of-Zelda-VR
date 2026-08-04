using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zora : Enemy
{

    public GameObject ProjectilePrefab;
    public float speed;

    private float ShootTime = 0;
    private float moveTime = 0;
    private float heightUp;
    private float heightDown;
    private bool UnderBridge = false;
    private bool shooting = false;

    private void Awake()
    {
        heightUp = transform.localPosition.y + 1;
        heightDown = transform.localPosition.y;
    }
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (stunned || ClockStun) return;
        if (Time.time > ShootTime && !UnderBridge)
        {
            Shoot();
            moveTime = Time.time + 2;
            ShootTime = Time.time + 4;
        }
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        bool outOfBounds = ((x < MinX || x > MaxX) && MaxX > MinX) || ((z < MinZ || z > MaxZ) && MaxZ > MinZ);
        if ((Time.time > moveTime) || outOfBounds || (Vector3.Distance(rigidBody.velocity, Vector3.zero) < 1 && !shooting))
        {
            if (!outOfBounds) moveTime = Time.time + 4;
            newDirection(outOfBounds);
        }
        else if (!shooting)
        {
            transform.LookAt(rigidBody.velocity + transform.position);
        }
    }

    private void newDirection(bool outOfBounds)
    {
        shooting = false;
        transform.localPosition = new Vector3(transform.localPosition.x, heightDown, transform.localPosition.z);
        currentHealth = MaxHealth;
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        rigidBody.velocity = new Vector3(randX, 0, randZ);
        transform.LookAt(rigidBody.velocity + transform.position);
        invulnerable = true;
    }

    public void Shoot()
    {
        rigidBody.velocity = Vector3.zero;
        invulnerable = false;
        shooting = true;
        transform.localPosition = new Vector3(transform.localPosition.x, heightUp, transform.localPosition.z);
        Vector3 v = Camera.main.transform.position;
        v.y = transform.position.y;
        transform.LookAt(v);
        Invoke("delayFire", 0.5f);
    }

    private void delayFire()
    {
        Vector3 v = Camera.main.transform.position - transform.position;
        v.x = 0f;
        v.z = 0f;
        transform.LookAt(Camera.main.transform.position - v);
        Projectile p = Instantiate(ProjectilePrefab, transform.position + new Vector3(0, 1.5f, 0f), transform.rotation).GetComponent<Projectile>();
        p.transform.SetParent(transform.parent);
        p.Shoot(transform.forward, gameObject);
    }

    public override void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);
        if (col.tag == "Bridge")
        {
            UnderBridge = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Bridge")
        {
            UnderBridge = false;
        }
    }
}