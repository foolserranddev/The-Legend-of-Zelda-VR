using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vire : MovingShooter
{
    [Header("Vire Specific")]
    public Keese keesePrefab;
    public float explosionRadius = 5;

    [HideInInspector]
    public bool stayDead = false;
    private float timeToDie;
    private string colliderTag;
    private float drag;
    private float angularDrag;
    private Keese[] spawn = new Keese[2];

    public override void Start()
    {
        base.Start();
        drag = rigidBody.drag;
        angularDrag = rigidBody.angularDrag;
    }
    // Use this for initialization
    public override void Update()
    {
        if (!dead)
        {
            base.Update();
        }
        else if (dead && Time.time > timeToDie)
        {
            spawn[0] = Instantiate(keesePrefab, transform.position + new Vector3(0, 1f, 0), keesePrefab.transform.rotation, transform.parent);
            spawn[1] = Instantiate(keesePrefab, transform.position + new Vector3(0, 1.2f, 0), keesePrefab.transform.rotation, transform.parent);
            //spawn[0].GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            //spawn[1].GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            spawn[0].GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) * explosionRadius;
            spawn[1].GetComponent<Rigidbody>().velocity = -spawn[0].GetComponent<Rigidbody>().velocity;
            GetComponent<Rigidbody>().drag = drag;
            GetComponent<Rigidbody>().angularDrag = angularDrag;
            base.Die(colliderTag);
        }
    }

    public override void Die(string cTag)
    {
        dead = true;
        if (currentHealth < -0.6f)
        {
            stayDead = true;
            base.Die(cTag);
        }
        else
        {
            colliderTag = cTag;
            timeToDie = Time.time + 0.1f;
            Vector3 vVire = transform.position;
            Vector3 vPlayer = Camera.main.transform.position;
            vPlayer.y = vVire.y;
            Vector3 direction = (vVire - vPlayer).normalized * explosionRadius;
            rigidBody.velocity = direction;
            rigidBody.drag = 0;
            rigidBody.angularDrag = 0;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        stayDead = false;
        for (int i = 0; i < spawn.Length; i++)
        {
            if (spawn[i] != null)
            {
                DestroyImmediate(spawn[i].gameObject);
            }
            spawn[i] = null;
        }
    }
}
