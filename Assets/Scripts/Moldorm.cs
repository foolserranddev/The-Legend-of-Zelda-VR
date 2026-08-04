using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moldorm : Enemy 
{
    public int NumBallsToMake = 5;
    public bool startingBall = false;
    public Moldorm moldormPrefab;

    private Moldorm parentBall;
    private Moldorm childBall;

    private bool initialized = false;

    public float speed = 3;
    public float minSpeed = 4;
    public float minMoveTime = 2f;
    public float maxMoveTime = 4f;
    public float maxPeakHeightFromStart = 0.5f;
    private float moveTime;
    private bool contactingParent = true;
    private float maxY;
    private float minY;

    public override void Start()
    {
        base.Start();
        maxY = transform.position.y + maxPeakHeightFromStart;
        minY = transform.position.y - maxPeakHeightFromStart;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        initialized = false;
    }

    public void OnDisable()
    {
        if (!startingBall) Destroy(gameObject);
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (startingBall && !initialized)
        {
            setParentBall(null);
            initialized = true;
        }
        else if (startingBall)
        {
            float x = transform.localPosition.x;
            float z = transform.localPosition.z;
            float speed = Vector3.Distance(rigidBody.velocity, Vector3.zero);
            if (Time.time > moveTime || x < MinX || x > MaxX || z < MinZ || z > MaxZ || speed < minSpeed)
            {
                newDirection();
            }
            else
            {
                Vector3 vel = rigidBody.velocity;
                vel.y = transform.position.y > maxY ? -(Mathf.Abs(Mathf.Sin(Time.time * 5) / 0.5f)) :
                        transform.position.y < minY ? Mathf.Abs(Mathf.Sin(Time.time * 5) / 0.5f) :
                        Mathf.Sin(Time.time * 5) / 0.5f;
                rigidBody.velocity = vel;
                transform.LookAt(rigidBody.velocity + transform.position);
            }
        }
        else if (!contactingParent)
        {
            rigidBody.velocity = Vector3.Normalize(parentBall.transform.position - transform.position) * speed;
        }
    }

    private void newDirection()
    {
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        Vector3 vel = new Vector3(randX, Mathf.Sin(Time.time * 5) / 5, randZ);
        rigidBody.velocity = vel * (speed / Vector3.Distance(vel, Vector3.zero));
        moveTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
    }

    public void setParentBall(Moldorm m)
    {
        parentBall = m;
        if (NumBallsToMake > 0)
        {
            childBall = Instantiate(moldormPrefab, transform.position, moldormPrefab.transform.rotation);
            childBall.NumBallsToMake = NumBallsToMake - 1;
            childBall.startingBall = false;
            childBall.transform.SetParent(transform.parent);
            childBall.setParentBall(this);
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (!startingBall && col.gameObject == parentBall.gameObject)
        {
            contactingParent = false;
        }
    }

    public override void HandleContact(Collider col)
    {
        if (col == null) return;
        if (!startingBall && col.gameObject == parentBall.gameObject)
        {
            contactingParent = true;
            rigidBody.velocity = Vector3.zero;
            return;
        }
        if (invulnerable) return;
        if (Time.time > hitDelay)
        {
            float damage = 0;
            if ((col.tag == "Sword" && col.GetComponent<Sword>().IsSwinging()) || col.tag == "BlinkySword" || col.tag == "Arrow") damage = col.GetComponent<InteractableObject>().damageDealt();
            else if (col.tag == "Bomb") damage = 4;
            else if (col.tag == "Fire") damage = 1;

            if (damage > 0)
            {
                currentHealth -= damage;
                hitDelay = Time.time + 0.5f;
                timeDelta = 0;
                PlayHitSound();
                if (!dead && currentHealth <= 0)
                {
                    deathOccurred(this, col);
                    // if not a lone ball, then make the death prefab at location of death.
                    // Otherwise, the Die function will make prefab.
                    if (childBall != null || parentBall != null)
                    {
                        Vector3 pos = transform.position;
                        pos.y += DeathPrefabHeightOffset;
                        Instantiate(DeathPrefab, pos, transform.rotation);
                    }
                }
            }
        }
    }

    public void PlayHitSound()
    {
        if (parentBall == null)
        {
            audioSource.clip = HitSound;
            audioSource.Play();
        }
        else
        {
            parentBall.PlayHitSound();
        }
    }

    public Moldorm deathOccurred(Moldorm m, Collider col)
    {
        // If Alerted From a Child Ball
        if (m == childBall || m == null)
        {
            // Reset Health and Alert Next Parent
            childBall = m;
            if (currentHealth > 0) currentHealth = MaxHealth;
            if (parentBall != null) parentBall.deathOccurred(this, col);
        }
        // If alerted by parent or self
        else
        {
            // If there's no child, this object will die
            if (childBall == null)
            {
                // If this is the head, time to die for real
                if (startingBall)
                {
                    Die(col.tag);
                }
                // If not head, then just disappear and alert parent
                else
                {
                    Destroy(gameObject);
                    return null;
                }
            }
            // If there's a child, let it die instead
            else
            {
                // Send kill down the children line and reset health
                childBall = childBall.deathOccurred(this, col);
                if (currentHealth > 0) currentHealth = MaxHealth;
            }
        }
        return this;
    }
}
