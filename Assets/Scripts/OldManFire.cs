using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManFire : Enemy
{
    public MovingShooter FireLeft;
    public MovingShooter FireRight;

    private SpriteRenderer spriteRenderer;
    private MeshRenderer[] meshRenderers;
    private string myLocation = "";

    public override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        myLocation = StandardStuff.getQuadrant(transform.position);
    }

    public override void Update()
    {
        base.Update();
        timeDelta += Time.deltaTime;
        if (hitDelay > Time.time)
        {
            if (timeDelta >= 0.05f)
            {
                if (spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
                foreach (MeshRenderer mr in meshRenderers)
                {
                    mr.enabled = !mr.enabled;
                }
                timeDelta = 0;
            }
        }
        else
        {
            if (spriteRenderer != null && !spriteRenderer.enabled) spriteRenderer.enabled = true;
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.enabled = true;
            }
        }
        if (!myLocation.Equals(Player.player.quadrant))
        {
            DisableFire();
        }
    }

    void OnDisable()
    {
        DisableFire();
    }

    private void DisableFire()
    {
        FireLeft.projectileMinRateOfFire = 0;
        FireLeft.projectileMaxRateOfFire = 0;
        FireRight.projectileMinRateOfFire = 0;
        FireRight.projectileMaxRateOfFire = 0;
    }

    public override void HandleContact(Collider col)
    {
        base.HandleContact(col);
        if (currentHealth != MaxHealth)
        {
            currentHealth = MaxHealth;
            FireLeft.projectileMinRateOfFire = 1;
            FireLeft.projectileMaxRateOfFire = 2;
            FireRight.projectileMinRateOfFire = 1;
            FireRight.projectileMaxRateOfFire = 2;
        }

    }


}
