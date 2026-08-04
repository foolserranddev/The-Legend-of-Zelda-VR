using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizzrobe : MovingShooter
{
    [Header("Wizzrobe Specific")]
    public bool isBlue = false;
    public AudioClip MagicalRod;
    public AudioClip Blocking;
    public float blueShootRate = 0.5f;
    public float bluePhaseMin = 5;
    public float bluePhaseMax = 10;
    public float phaseDuration = 0.5f;
    public float blueSpeedMultiplier = 3;
    public Renderer selfImage;
    public float blinkInTime = 1f;
    public float freezeAfterShot = 1.5f;

    private Animator animator;
    private float projectileTime;
    private float mobileTime;
    private float solidTime;
    private bool shooting;
    private bool blinking;
    private Collider col;
    private AudioSource wizzAudio;
    private LayerMask wizzLayerMask;
    private float tempSpeed;
    private float phaseTime;
    private float phaseEnd;
    private bool phasing;
    //private bool stopWhileLoop = false;
    private bool phasingThroughObstacle = false;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        wizzAudio = GetComponent<AudioSource>();
        wizzLayerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Walls"));
        if (isBlue) animator.speed = animator.speed * 3;
        tempSpeed = speed;
        if (!isBlue)
        {
            col.enabled = false;
            selfImage.enabled = false;
        }
    }

    public override void Update()
    {
        base.Update();
        if (stunned || ClockStun) return;
        if (isBlue)
        {
            if (phasingThroughObstacle)
            {
                selfImage.enabled = !selfImage.enabled;
                return;
            }
            if (shooting && Time.time > ShootTime)
            {
                animator.SetTrigger("Fire");
                base.Shoot();
                wizzAudio.clip = MagicalRod;
                wizzAudio.Play();
                ShootTime = Time.time + blueShootRate;
            }
            if (phasing)
            {
                if (Time.time > phaseEnd)
                {
                    phasing = false;
                    selfImage.enabled = true;
                    col.enabled = true;
                    phaseTime = Time.time + Random.Range(bluePhaseMin, bluePhaseMax);
                    moveTime = Time.time;
                    speed = tempSpeed;
                }
                else
                {
                    selfImage.enabled = !selfImage.enabled;
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 0.5f, transform.forward, out hit, 14, wizzLayerMask))
                {
                    if (hit.collider.tag == "Player")
                    {
                        if (!shooting) ShootTime = Time.time;
                        shooting = true;
                        isMobile = false;
                    }
                    else
                    {
                        shooting = false;
                        isMobile = true;
                    }
                }
                else
                {
                    shooting = false;
                    isMobile = true;
                    if (Time.time > phaseTime)
                    {
                        phasing = true;
                        selfImage.enabled = false;
                        col.enabled = false;
                        phaseEnd = Time.time + phaseDuration;
                        moveTime = Time.time;
                        speed = speed * blueSpeedMultiplier;
                    }
                }
            }
        }
        else // Is Orange
        {
            if (phasingThroughObstacle) return;
            if (blinking) // Blinking occurs as it reappears before shooting, but it begins after "Shoot" function
            {
                rigidBody.velocity = Vector3.zero;
                if (Time.time > solidTime)
                {
                    selfImage.enabled = true;
                    blinking = false;
                    animator.SetTrigger("Fire");
                }
                else
                {
                    selfImage.enabled = !selfImage.enabled;
                }
            }
            else if (Time.time > projectileTime && shooting)
            {
                base.Shoot(); // Calling the BASE shoot rather than the extended shoot which is called by the base class
                shooting = false;
                wizzAudio.clip = MagicalRod;
                wizzAudio.Play();
            }
            else if (!isMobile && Time.time > mobileTime)
            {
                isMobile = true;
                col.enabled = false;
                selfImage.enabled = false;
            }
        }
    }

    // Called by Base Class. Does not occur if Player not in same quadrant.
    public override void Shoot()
    {
        shooting = true;
        if (phasingThroughObstacle) return;
        DetermineBetterOrientation();
        isMobile = false;
        blinking = true;
        col.enabled = true;
        rigidBody.velocity = Vector3.zero;
        solidTime = Time.time + blinkInTime;
        projectileTime = solidTime + 0.9f;
        mobileTime = projectileTime + freezeAfterShot;
        ShootTime = mobileTime + Random.Range(projectileMinRateOfFire, projectileMaxRateOfFire);
    }

    private void DetermineBetterOrientation()
    {
        Vector3 lookat = Camera.main.transform.position;
        lookat.y = transform.position.y;
        transform.LookAt(lookat);
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), transform.forward * 16, out hit, 16, layerMask) && hit.transform.tag == "Player") return; // If player in front, no logic needed.
        //while (!stopWhileLoop && Physics.SphereCast(transform.position + new Vector3(0, 1f, 0), 1, transform.forward * 4, out hit, 4, layerMask) && hit.transform.tag == "Wall")
        //{
        //    transform.Rotate(new Vector3(0, 90, 0));
        //}
    }

    public override void OnTriggerStay(Collider col)
    {
        if (col.tag == "Wall" || col.tag == "Water" || col.tag == "Ocean")
        {
            phasingThroughObstacle = true;
            if (isBlue && speed == tempSpeed)
            {
                speed = speed * blueSpeedMultiplier;
                rigidBody.velocity *= blueSpeedMultiplier;
            }
            isMobile = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Wall" || col.tag == "Water" || col.tag == "Ocean")
        {
            phasingThroughObstacle = false;
            selfImage.enabled = true;
            if (isBlue && speed != tempSpeed)
            {
                speed = tempSpeed;
                rigidBody.velocity /= blueSpeedMultiplier;
            }
            if (shooting) Shoot();
        }
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Wall" || col.tag == "Water" || col.tag == "Ocean")
        {
            OnTriggerStay(col);
        }
        else
        {
            base.OnTriggerEnter(col);
        }
    }

    public override void HandleContact(Collider col)
    {
        if (col.GetComponent<WandBlast>() == null)
        {
            base.HandleContact(col);
        }
        else
        {
            wizzAudio.clip = Blocking;
            wizzAudio.Play();
        }
    }
}
