using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingShooter : Enemy
{
    public bool hitKnockback = false;
    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public bool shootTowardPlayer = false;
    public int DamageDealtOverride = 0;
    public bool DisableShieldBlocking = false;
    public int projectileMinRateOfFire = 3;
    public int projectileMaxRateOfFire = 4;
    public float projectileForwardOffset = 0;
    public float projectileUpOffset = 0;
    public float projectileRightOffset = 0;
    public Vector3 ProjectileRotation;
    [Header("Movement")]
    public bool isMobile = true;
    public float speed = 3;
    public float minSpeed = 0;
    public bool lockXMovement = false;
    public bool lockZMovement = false;
    public bool rotate = true;
    public float minMoveTime = 2f;
    public float maxMoveTime = 4f;
    [Header("Pathfinding Tweaks")]
    public float SpherecastRadius = 0.5f;
    public float RotatePercent = 1f;
    public float MinDistanceToMove = 1f;

    protected float ShootTime;
    protected float moveTime;
    protected bool knockingBack;

    private float knockbackTime;
    private Vector3 previousVelocity;
    private LayerMask layerMask;
    private bool attemptSpeedup = false;
    private bool playerColliding = false;
    private float nextDirectionChangeTime;
    private float stalledSince = -1f;

    public void Awake()
    {
        layerMask = (1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Trees")) | (1 << LayerMask.NameToLayer("Water"));
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (stunned || ClockStun) return;
		if (projectileMaxRateOfFire > 0 && Time.time > ShootTime)
        {
            ShootTime = Time.time + Random.Range(projectileMinRateOfFire, projectileMaxRateOfFire);
            // I want to keep the randomness of when to shoot but only shoot if player is in the screen for dungeons
            if (Player.player.quadrant.Equals(myQuadrant) || !Player.player.InDungeon) Shoot();
        }
        if (knockingBack && Time.time > knockbackTime)
        {
            knockingBack = false;
            rigidBody.velocity = previousVelocity;
        }
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        if (rotate && !playerColliding)
        {
            transform.LookAt(rigidBody.velocity + transform.position);
        }
        if (isMobile && !playerColliding)
        {
            float currentSpeed = rigidBody.velocity.magnitude;
            bool outOfBounds = x < MinX || x > MaxX || z < MinZ || z > MaxZ;
            if (currentSpeed < minSpeed)
            {
                if (stalledSince < 0f) stalledSince = Time.time;
                // A momentary contact should not become a turn. Restore the
                // configured speed along the existing heading first.
                if (rigidBody.velocity.sqrMagnitude > 0.0001f)
                    rigidBody.velocity = rigidBody.velocity.normalized * this.speed;
            }
            else
            {
                stalledSince = -1f;
            }

            bool genuinelyStalled = stalledSince >= 0f && Time.time - stalledSince >= 0.4f;
            bool needsDirection = (!knockingBack && Time.time > moveTime) || outOfBounds || genuinelyStalled;
            if (needsDirection && Time.time >= nextDirectionChangeTime)
            {
                NewDirection();
                stalledSince = -1f;
            }
        }
    }

    private void NewDirection()
    {
        if (stunned) return;
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = lockXMovement ? 0 : Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = lockZMovement ? 0 : Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        Vector3 vel = new Vector3(randX, 0, randZ);
        vel = ImproveDirection(vel);
        rigidBody.velocity = vel * (speed / Vector3.Distance(vel, Vector3.zero));
        moveTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
        // A blocked rigidbody can report low speed for several render frames.
        // Prevent a new random turn on every frame while PhysX resolves it.
        nextDirectionChangeTime = Time.time + 0.35f;
        if (rotate) transform.LookAt(rigidBody.velocity + transform.position);
    }

    public Vector3 ImproveDirection(Vector3 vel)
    {
        RaycastHit hit;
        int tries = 0;
        float MaxDistance = 0;
        int neg = Random.Range(0, 1) == 0 ? -1 : 1;
        vel = (Quaternion.Euler(Vector3.up * -15 * neg) * vel).normalized * speed;
        Vector3 MaxDistanceVel = Vector3.zero;
        while (Physics.SphereCast(transform.position, SpherecastRadius, vel, out hit, MinDistanceToMove, layerMask) && tries < 360 / Mathf.Max(1f, RotatePercent))
        {
            if (hit.distance > MaxDistance)
            {
                MaxDistance = hit.distance;
                MaxDistanceVel = vel;
            }
            vel = (Quaternion.Euler(Vector3.up * RotatePercent * neg) * vel).normalized * speed;
            if (hit.distance > MaxDistance)
            {
                MaxDistance = hit.distance;
                MaxDistanceVel = vel;
            }
            tries++;
        }
        if (tries == 360 / Mathf.Max(1f, RotatePercent))
        {
            //Debug.Log(StandardStuff.getName(transform) + " Could not find an adequate moving place");
            vel = MaxDistanceVel;
        }
        return vel;
    }

    public virtual void Shoot()
    {
        if (ProjectilePrefab == null) return;
        Vector3 startPos = transform.position + (transform.forward * projectileForwardOffset) + (transform.up * projectileUpOffset) + (transform.right * projectileRightOffset);
        Projectile p = Instantiate(ProjectilePrefab, startPos, ProjectilePrefab.transform.rotation).GetComponent<Projectile>();
        if (DisableShieldBlocking)
        {
            p.CanLargeShieldBlock = false;
            p.CanSmallShieldBlock = false;
            p.gameObject.layer = LayerMask.NameToLayer("Unblockable Projectile");
        }
        if (DamageDealtOverride > 0)
        {
            p.damagePerHit = DamageDealtOverride;
        }

        if (shootTowardPlayer)
        {
            Vector3 dir = Camera.main.transform.position - transform.position;
            if (dir.y > 0) dir.y = 0; // aim down if necessary (Gleeok flying head), but not up
            p.Shoot(dir, this.gameObject);
        }
        else
        {
            Vector3 rot = p.transform.localEulerAngles;
            p.transform.SetParent(transform.parent);
            rot = transform.localEulerAngles + ProjectileRotation;
            p.transform.localEulerAngles = rot;
            p.Shoot(transform.forward, gameObject);
        }
    }

    public override void HandleContact(Collider col)
    {
        base.HandleContact(col);
        if (hitKnockback && ((col.tag == "Sword" && col.GetComponent<Sword>().IsSwinging()) || col.tag == "Bomb" || col.tag == "Fire" || col.tag == "Arrow"))
        {
            Vector3 EnemyToCol = col.transform.position - transform.position;
            if (Vector3.Angle(EnemyToCol, rigidBody.velocity) < 45)
            {
                knockingBack = true;
                knockbackTime = Time.time + 0.1f;
                previousVelocity = rigidBody.velocity;
                rigidBody.velocity = -EnemyToCol.normalized * 40;
            }
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            playerColliding = true;
        }
        base.OnCollisionEnter(collision);
    }

    public virtual void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            playerColliding = false;
        }
    }

    public override void Die(string colliderTag)
    {
        base.Die(colliderTag);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        playerColliding = false;
        nextDirectionChangeTime = Time.time;
        stalledSince = -1f;
    }
}
