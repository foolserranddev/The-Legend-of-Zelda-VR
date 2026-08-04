using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghini : Enemy {

    public float speed = 8;
    public bool isMaster = false;
    public GameObject GhiniPrefab;

    private Ghini[] slaves = new Ghini[10];
    private float moveTime;
    private int numSlaves = 0;
    private float startTime;
    private float realDamage;

    public override void Start()
    {
        base.Start();
        if (isMaster)
        {
            startTime = Time.time;
            GetComponent<SphereCollider>().isTrigger = false;
        }
        else
        {
            startTime = Time.time + 1;
            invulnerable = true;
            realDamage = damageDealt;
            damageDealt = 0;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (stunned || ClockStun) return;
        if (damageDealt == 0 && Time.time > startTime) damageDealt = realDamage;
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        if (Time.time > moveTime || x < MinX || x > MaxX || z < MinZ || z > MaxZ || Vector3.Distance(rigidBody.velocity, Vector3.zero) < 1)
        {
            newDirection();
        }
        else
        {
            transform.LookAt(rigidBody.velocity + transform.position);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (isMaster)
        {
            for (int i = 0; i < 10; i++)
            {
                if (slaves[i] != null)
                {
                    DestroyImmediate(slaves[i].gameObject);
                }
            }
            numSlaves = 0;
        }
    }

    public void OnDisable()
    {
        if (!isMaster)
        {
            Destroy(gameObject);
        }
    }

    private void newDirection()
    {
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        Vector3 v;
        if (isMaster) v = Random.Range(0, 2) == 0 ? new Vector3(0, 0, randZ) : new Vector3(randX, 0, 0);
        else v = new Vector3(randX, 0, randZ);
        rigidBody.velocity = v; 
        moveTime = Time.time + Random.Range(1f, 3f);
        
        transform.LookAt(rigidBody.velocity + transform.position);
    }

    public void AddGhost(Vector3 location)
    {
        if (numSlaves < 10)
        {
            location.y = transform.position.y;
            slaves[numSlaves] = Instantiate(GhiniPrefab, location, GhiniPrefab.transform.rotation).GetComponent<Ghini>();
            slaves[numSlaves].MinX = MinX;
            slaves[numSlaves].MinZ = MinZ;
            slaves[numSlaves].MaxX = MaxX;
            slaves[numSlaves].MaxZ = MaxZ;
            slaves[numSlaves].transform.SetParent(transform.parent);
            numSlaves++;
        }
    }

    public override void Die(string colliderTag)
    {
        if (isMaster)
        {
            for (int i = 0; i < numSlaves; i++)
            {
                slaves[i].Die(colliderTag);
                Destroy(slaves[i]);
            }
            numSlaves = 0;
        }
        base.Die(colliderTag);
    }
}
