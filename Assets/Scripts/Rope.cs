using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MovingShooter
{
    private LayerMask lMask;

    public override void Start()
    {
        base.Start();
        lMask = LayerMask.GetMask(new string[] { "Player", "Wall" });
    }
    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (stunned || ClockStun) return;

        float maxDistance = -transform.localPosition.z + 0.5f;
        Vector3 north = new Vector3(0, 0, -1);
        if (checkAttack(north, Mathf.Abs(maxDistance))) return;

        maxDistance = 15.5f - transform.localPosition.z;
        Vector3 south = new Vector3(0, 0, 1);
        if (checkAttack(south, maxDistance)) return;

        maxDistance = 15.5f - transform.localPosition.x;
        Vector3 west = new Vector3(1, 0, 0);
        if (checkAttack(west, maxDistance)) return;

        maxDistance = -transform.localPosition.x + 0.5f;
        Vector3 east = new Vector3(-1, 0, 0);
        checkAttack(east, Mathf.Abs(maxDistance));
    }

    private bool checkAttack(Vector3 dir, float maxDistance)
    {
        bool attacking = false;

        RaycastHit hit;
//        Debug.DrawRay(transform.position + new Vector3(0, 0.6f, 0), dir * maxDistance);
        if (Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 1f, dir, out hit, maxDistance, lMask))
        {
//            Debug.Log(hit.transform.tag);
            if (hit.transform.tag == "Player")
            {
                attacking = true;
                Vector3 vel = Vector3.Normalize(Camera.main.transform.position - transform.position) * speed * 2.5f;
                rigidBody.velocity = vel; //* (speed / Vector3.Distance(vel, Vector3.zero));
                moveTime = Time.time + 3;
                transform.LookAt(rigidBody.velocity + transform.position);
            }
        }
        return attacking;
    }

    //public override void OnTriggerEnter(Collider col)
    //{
    //    if (attacking)
    //    {
    //        attacking = false;
    //    }
    //}

}
