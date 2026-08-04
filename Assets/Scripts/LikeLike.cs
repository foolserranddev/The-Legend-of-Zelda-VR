using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeLike : MovingShooter {

    public float jigglySpeed = 10f;
    public float jigglyScaleMultiplier = 0.1f;
    public float rotateSpeed = 10;
    private bool captured;
    private float y;
    private float ShieldSwallowTime;
    private Collider myCol;
    private float standardHeight;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        y = transform.position.y;
        myCol = GetComponent<Collider>();
        standardHeight = transform.localScale.y;
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (captured)
        {
            if (Player.player.Dead)
            {
                captured = false;
                myCol.isTrigger = false;
                return;
            }
            Vector3 newPos = Camera.main.transform.position;
            newPos.y = y;
            transform.position = newPos;
            if (Time.time > ShieldSwallowTime && Player.player.HasBigShield.val)
            {
                Player.player.RemoveShield();
            }
        }
        Vector3 newScale = transform.localScale;
        newScale.y = standardHeight + Mathf.Sin(Time.time * jigglySpeed) * jigglyScaleMultiplier;
        transform.localScale = newScale;
        transform.Rotate(transform.up * rotateSpeed * Time.deltaTime);
    }

    public void OnDisable()
    {
        if (captured) Player.player.Mobilize();
    }

    public override void OnTriggerEnter(Collider col)
    {
        HandleContact(col);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        HandleContact(collision.collider);
    }

    public override void HandleContact(Collider col)
    {
        base.HandleContact(col);
        if (!knockingBack && col.tag == "Player" && Player.player.isMobile && !Player.player.Dead && Time.time > hitDelay)
        {
            captured = true;
            myCol.isTrigger = true;
            isMobile = false;
            Player.player.Immobilize();
            ShieldSwallowTime = Time.time + 2f;
        }
    }
}
