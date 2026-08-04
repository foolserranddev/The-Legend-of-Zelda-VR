using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darknut : MovingShooter
{
    private AudioSource audioShield;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        audioShield = GetComponent<AudioSource>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Bomb" || col.tag == "Boomerang")
        {
            base.HandleContact(col);
        }
        else if ((col.tag == "Sword" && col.GetComponent<Sword>().IsSwinging()) || (col.tag == "BlinkySword"))
        {
            Vector3 playerDiff = col.transform.position - transform.position;
            float inFront = Vector3.Dot(playerDiff, transform.forward);
            //Debug.Log("Forward = " + transform.forward);
            if (inFront > 0 || col.GetComponent<WandBlast>() != null)
            {
                //Debug.Log("in front of me (true) = " + inFront);
                audioShield.Play();
            }
            else
            {
                //Debug.Log("in front of me (false) = " + inFront);
                base.HandleContact(col);
            }
        }
    }

    public override void HandleContact(Collider col)
    {
        //Debug.Log("Darknut handling contact for some strange reason.");
    }

    public override void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Darknut Collision with " + collision.collider.tag);
        if (collision.collider.tag == "Arrow") audioShield.Play();
        else OnTriggerEnter(collision.collider);
    }

    //public override void OnTriggerStay(Collider col)
    //{
    //    OnTriggerEnter(col);
    //}
}
