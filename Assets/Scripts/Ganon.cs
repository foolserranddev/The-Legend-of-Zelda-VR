using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ganon : MovingShooter
{
    [Header("Ganon Specific")]
    public bool isCharacter = false;
    public bool reverse = false;
    public float MAX_DISTANCE = 3;
    public float MIN_DISTANCE = 1;
    public float EXPANSION_RATE = 1;
    public float ROTATE_RATE = 1;
    public float FOLLOW_RATE = 1;
    public bool isPaused;

    private float expansionNegator = 1;
    private float rotateNegator = 1;
    private Ganon GanonObject;
    private float currentDistance;
    private bool goneTooFar = false;
    private Ganon GanonFather;
    private SkinnedMeshRenderer meshRenderer;
    private float unpauseTime;
    private float blinkTime;
    private Animator animator;
    private Billboard billboard;
    private bool playedCry;

    private bool initialized = false;

    // Use this for initialization
    public override void Start ()
    {
		if (!isCharacter)
        {
            GanonObject = GetComponentsInChildren<Ganon>()[1];
            rigidBody = GetComponent<Rigidbody>();
        }
        else
        {
            base.Start();
            GanonFather = transform.parent.GetComponent<Ganon>();
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (meshRenderer == null) Debug.Log("Renderer Null");
            animator = GetComponent<Animator>();
            billboard = GetComponent<Billboard>();
        }
	}
    public override void OnEnable()
    {
        base.OnEnable();
        playedCry = false;
    }

    // Update is called once per frame
    public override void Update ()
    {
        if (!initialized) return;
        if (!playedCry && !isCharacter && Player.player.quadrant == myQuadrant)
        {
            CryAudio.Play();
            playedCry = true;
        }
        if (isCharacter)
        {
            if (isPaused)
            {
                if (Time.time > unpauseTime)
                {
                    isPaused = false;
                    GanonFather.isPaused = false;
                    meshRenderer.enabled = false;
                    meshRenderer.material = HitMaterials[0];
                    animator.SetBool("Randomizing", true);
                    if (currentHealth <= 0) currentHealth = MaxHealth;
                    billboard.enabled = true;
                    tag = "Untagged";
                    GanonFather.transform.localPosition = new Vector3(Random.Range(2, 14), 0, Random.Range(2, 14));
                }
                else if (Time.time > blinkTime)
                {
                    meshRenderer.enabled = !meshRenderer.enabled;
                }
            }
            else
            {
                base.Update();
            }
        }
        else if (!isPaused)
        {
            if (reverse)
            {
                rotateNegator *= -1;
                reverse = false;
            }
            transform.Rotate(Vector3.up, ROTATE_RATE * Time.deltaTime * rotateNegator);

            currentDistance = currentDistance + (EXPANSION_RATE * Time.deltaTime * expansionNegator);
            if (currentDistance > MAX_DISTANCE)
            {
                currentDistance = MAX_DISTANCE;
                expansionNegator *= -1;
            }
            else if (currentDistance < MIN_DISTANCE)
            {
                currentDistance = MIN_DISTANCE;
                expansionNegator *= -1;
            }

            Vector3 FinalLocalPosition = transform.localPosition + (transform.forward * currentDistance);
            float newX = Mathf.Max(Mathf.Min(FinalLocalPosition.x, GanonObject.MaxX), GanonObject.MinX);
            float newZ = Mathf.Max(Mathf.Min(FinalLocalPosition.z, GanonObject.MaxZ), GanonObject.MinZ);
            float tooFar = Mathf.Sqrt(Mathf.Pow(FinalLocalPosition.x - newX, 2) + Mathf.Pow(FinalLocalPosition.z - newZ, 2));
            if (tooFar > 0)
            {
                if (!goneTooFar) reverse = true;
                goneTooFar = true;
            }
            else
            {
                goneTooFar = false;
            }
            Vector3 GanonPosition = new Vector3(0, GanonObject.transform.localPosition.y, currentDistance - (tooFar * 1.1f));
            GanonObject.transform.localPosition = GanonPosition;

            rigidBody.velocity = (Camera.main.transform.position - transform.position).normalized * FOLLOW_RATE;
        }
    }

    public override void HandleContact(Collider col)
    {
        if (!isPaused && col.tag == "Sword")
        {
            Sword s = col.GetComponent<Sword>();
            if (s.IsSwinging())
            {
                currentHealth -= s.damageDealt();
                Debug.Log("Current Health = " + currentHealth + " after " + s.damageDealt() + " damage.");
                if (currentHealth <= 0)
                {
                    blinkTime = Time.time + 6;
                    unpauseTime = Time.time + 8;
                    meshRenderer.material = HitMaterials[1];
                }
                else
                {
                    unpauseTime = Time.time + 2;
                    blinkTime = unpauseTime;
                }
                isPaused = true;
                animator.SetBool("Randomizing", false);
                stunTime = Time.time + 1f;
                GanonFather.Pause();
                meshRenderer.enabled = true;
                billboard.enabled = false;
                tag = "Enemy";
                audioSource.Play();
            }
        }
        else if (isPaused && currentHealth <= 0 && col.tag == "Arrow" && ObjectList.objectList.itemLevels[(int)ObjectList.prefabObjects.BowAndArrow].val == 1)
        {
            Vector3 finalPosition = transform.position;
            GanonFather.transform.position = transform.position;
            transform.position = finalPosition;
            GanonFather.Pause();
            hitDelay = Time.time + 3;
            timeDelta = 0;
            isPaused = false;
            ProjectilePrefab = null;
            Invoke("DelayedDeath", 2);
        }
        else if (isPaused && Time.time > stunTime && col.tag == "Player")
        {
            base.HandleContact(col);
        }
    }

    public void DelayedDeath()
    {
        Player.player.StopMusic();
        GanonFather.Die("Arrow");
    }

    public void Pause()
    {
        isPaused = true;
        rigidBody.velocity = Vector3.zero;
    }

    public void BeginFight()
    {
        initialized = true;
        // If the parent Ganon, set the Child Ganon
        if (GanonObject != null) GanonObject.BeginFight();
        // Else the Child Ganon, Get Moving and Disappear
        else
        {
            animator.SetBool("Randomizing", true);
            meshRenderer.enabled = false;
        }
    }

}
