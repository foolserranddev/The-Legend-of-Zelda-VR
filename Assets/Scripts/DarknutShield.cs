using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknutShield : Enemy
{
    // Use this for initialization
    public override void Start()
    {
        audioSource = GetComponent<AudioSource>();
        damageDealt = GetComponentInParent<Darknut>().damageDealt;
        invulnerable = true;
        isStunnable = false;
    }


    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Arrow" || collision.collider.tag == "BlinkySword") audioSource.Play();
    }

    public override void HandleContact(Collider col)
    {
        
    }
}
