using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aquamentus : MovingShooter
{
    public override void Shoot()
    {
        Vector3 startPos = transform.position + (transform.forward * projectileForwardOffset) + (transform.up * projectileUpOffset) + (transform.right * projectileRightOffset);
        Projectile p = Instantiate(ProjectilePrefab, startPos + new Vector3(0, 0, -1f), ProjectilePrefab.transform.rotation).GetComponent<Projectile>();
        Projectile p1 = Instantiate(ProjectilePrefab, startPos, ProjectilePrefab.transform.rotation).GetComponent<Projectile>();
        Projectile p2 = Instantiate(ProjectilePrefab, startPos + new Vector3(0, 0, 1f), ProjectilePrefab.transform.rotation).GetComponent<Projectile>();
        p2.Shoot(Camera.main.transform.position - new Vector3(0, 0.5f, -2) - startPos - new Vector3(0, 0, -1f), gameObject);
        p1.Shoot(Camera.main.transform.position - new Vector3(0, 0.5f, 0) - startPos, gameObject);
        p.Shoot(Camera.main.transform.position - new Vector3(0, 0.5f, 2) - startPos - new Vector3(0, 0, 1f), gameObject);
    }
}
