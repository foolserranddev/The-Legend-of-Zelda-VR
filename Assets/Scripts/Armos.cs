using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armos : MovingShooter
{
    public bool RevealSecret = false;

    private Vector3 startPos;
    private Vector3 startRot;
    private bool initialized = false;
    private Renderer rend;
    private Material mat;
    private float RevealTime;
    private bool triggered = false;
    private DoorReveal doorReveal;
    private bool revealed = false;
    private Material livingMat;

    // Use this for initialization
	public override void Start ()
    {
        livingMat = HitMaterials[0];
        base.Start();
        allowBlink = false;
        rend = GetComponent<Renderer>();
        mat = rend.material;
        if (RevealSecret) doorReveal = GetComponent<DoorReveal>();
    }

    public override void Update()
    {
        base.Update();
        if (isMobile && (stunned || ClockStun)) return;
        if (triggered && !isMobile && Time.time > moveTime)
        {
            isMobile = true;
            rend.material = HitMaterials[0];
        }
        if (RevealSecret && triggered && Time.time > RevealTime)
        {
            doorReveal.Reveal(false);
            triggered = false;
            revealed = true;
        }
        //if (isMobile && Player.player.isUnderground)
        //{
        //    gameObject.SetActive(false);
        //    gameObject.SetActive(true);
        //}
        if (isMobile && rend.sharedMaterial != livingMat)
        {
            Debug.Log("Armos Updating Material");
            rend.material = livingMat;
        }
    }

    public override void OnEnable()
    {
        allowBlink = false;
        if (!initialized)
        {
            startPos = transform.position;
            startRot = transform.localEulerAngles;
            initialized = true;
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            isMobile = false;
            transform.position = startPos;
            transform.localEulerAngles = startRot;
            rend.material = mat;
            if (RevealSecret) doorReveal.Unreveal();
            base.OnEnable();
        }
    }

    public override void Die(string s)
    {
        tag = "Armos";
        if (RevealSecret && !revealed)
        {
            doorReveal.Reveal(false);
            revealed = true;
        }
        base.Die(s);
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (isMobile)
        {
            base.OnTriggerEnter(col);
        }
        else
        {
            if (!triggered && (col.tag == "Player" || col.tag == "Sword" || col.tag == "LeftHand" || col.tag == "RightHand" || col.tag == "Shield"))
            {
                tag = "Enemy";
                hitDelay = Time.time + 0.5f;
                moveTime = Time.time + 0.75f;
                RevealTime = Time.time + 1f;
                triggered = true;
                speed = Random.Range(0, 2) == 0 ? 3 : 6;
                rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                gameObject.layer = LayerMask.NameToLayer("Enemies");
                allowBlink = true;
            }
        }

    }

    public override void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    public override void OnTriggerStay(Collider col)
    {
        if (isMobile)
        {
            base.OnTriggerStay(col);
        }
    }
}
