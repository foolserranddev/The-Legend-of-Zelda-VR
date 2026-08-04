using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : InteractableObject {

    public float ShootSpeed;
    public float damagePerHit;
    public float AllowableDistance;
    public bool CanSmallShieldBlock;
    public bool CanLargeShieldBlock;
    public bool goesThroughObstacles = false;
    public GameObject impactAnimation;
    public float impactObjectHeightOffset = 0;

    private Vector3 startPosition;
    private float timeoutDelay = 0.2f;
    private float timeoutTime = 0f;
    protected GameObject shooter;
    protected Rigidbody rigidBody;

    private const float BOUNCE_UP = 10;

    public override void Start()
    {
        base.Start();
        if (tag == "Enemy Projectile")
        {
            if (CanSmallShieldBlock)
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy Projectile");
            }
            else if (CanLargeShieldBlock)
            {
                gameObject.layer = LayerMask.NameToLayer("BigShieldProjectile");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Unblockable Projectile");
            }
            GetComponent<Collider>().isTrigger = false;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player Projectile");
            GetComponent<Collider>().isTrigger = true;
        }
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        
        if (timeoutTime == 0 && Vector3.Distance(startPosition, transform.localPosition) > AllowableDistance)
        {
            timeoutTime = Time.time + timeoutDelay;
            if (tag != "BlinkySword") GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Collider>().enabled = false;
        }
        if (timeoutTime != 0 && Time.time > timeoutTime)
        {
            DestroyImmediate(gameObject);
        }
    }

    public virtual void Shoot(Vector3 direction, GameObject s)
    {
        shooter = s;
        startPosition = transform.localPosition;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.velocity = direction.normalized * ShootSpeed;
        rigidBody.mass = 0;
    }

    private void handleCollision(Collider col)
    {
        if (col.tag == "Collectible" && (tag == "Boomerang" || tag == "Arrow"))
        {
            Player.player.HandleCollisions(col);
            return;
        }
        else if (col.tag == "Wall" || (col.tag == "Enemy" && tag != "Enemy Projectile"))
        {
            if (impactAnimation != null) Instantiate(impactAnimation, transform.position + new Vector3(0, impactObjectHeightOffset, 0), transform.rotation);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (col.tag == "Ground")
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (gameObject.layer != LayerMask.NameToLayer("Player Projectile") && col.tag != "Player")
        {
            GetComponent<Collider>().enabled = false;
            rigidBody.useGravity = true;
            timeoutTime = Time.time + timeoutDelay;
            rigidBody.velocity = (rigidBody.velocity * -1) + new Vector3(0, BOUNCE_UP, 0);
        }
        else if (tag == "Enemy Projectile" && col.tag == "Player")
        {
            timeoutTime = Time.time + timeoutDelay;
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        handleCollision(collision.collider);
    }

    public virtual void OnTriggerEnter(Collider col)
    {
        handleCollision(col);
        //bool ignoreHit = (col.tag == "Sword" || col.tag == "LeftHand" || col.tag == "RightHand" || col.tag == "Player" || col.tag == "Shield" || col.tag == "Enemy Projectile" || col.tag == "Water" || col.tag == "Ocean" || col.tag == "InvisibleCollider" || col.tag == "Untagged");
        //ignoreHit = ignoreHit || (goesThroughObstacles && col.gameObject.layer == LayerMask.NameToLayer("Trees"));
        //if (col.gameObject != shooter && !ignoreHit && !(col.tag == "Enemy" && tag == "Enemy Projectile")) Destroy(gameObject);
    }

    public override float damageDealt()
    {
        return damagePerHit;
    }
}
