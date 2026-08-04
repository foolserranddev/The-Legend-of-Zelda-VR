using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manhandla : MovingShooter
{
    [Header("Manhandla Specific")]
    public Manhandla[] claws;
    public bool isBody = false;
    private Animation anim;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        anim = GetComponent<Animation>();
    }

    //// Update is called once per frame
    //void Update () {

    //}

    public void ClawDied(string colliderTag)
    {
        bool allDead = true;
        foreach (Manhandla m in claws)
        {
            if (!m.dead) allDead = false;
        }
        if (allDead)
        {
            Die(colliderTag);
        }
        else
        {
            speed += 1.5f;
            rigidBody.velocity = rigidBody.velocity.normalized * speed;
            anim["Manhandla"].speed += 1;
        }
    }

    public override void Die(string colliderTag)
    {
        dead = true;
        if (claws.Length == 1) GetComponentsInParent<Manhandla>()[1].ClawDied(colliderTag);
        base.Die(colliderTag);
    }

    public override void HandleContact(Collider col)
    {
        // If invulnerable, then it's the center body which makes a dying sound when hit but does nothing else.
        base.HandleContact(col);
//        Debug.Log("Manhandla Handla Contact. Health = " + currentHealth + " isBody = " + isBody);
        if (isBody) currentHealth = MaxHealth;
    }
}
