using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gel : Enemy {

    public float minMoveTime = 1f;
    public float maxMoveTime = 2f;
    public float jiggleSpeed = 30;
    public float jiggleHeightMultiplier = 0.05f;
    private float moveTime;

    // Use this for initialization
    public override void Update()
    {
        base.Update();
        if (stunned || ClockStun) return;
        Vector3 newScale = transform.localScale;
        newScale.y = newScale.x + Mathf.Sin(Time.time * jiggleSpeed) * jiggleHeightMultiplier;
        transform.localScale = newScale;
        if (Time.time > moveTime) Jump();
    }

    private void Jump()
    {
        float randX = Random.Range(transform.localPosition.x < 2 ? 0 : -1, transform.localPosition.x > 14 ? 0 : 2);
        //Random.Range((Mathf.Max((transform.localPosition.x - MinX), 0) / (MaxX - MinX)) * -15f, (Mathf.Max((MaxX - transform.localPosition.x), 0) / (MaxX - MinX)) * 15f);
        float randZ = Random.Range(transform.localPosition.z < 2 ? 0 : -1, transform.localPosition.z > 14 ? 0 : 2);
        //Random.Range((Mathf.Max((transform.localPosition.z - MinZ), 0) / (MaxZ - MinZ)) * -15f, (Mathf.Max((MaxZ - transform.localPosition.z), 0) / (MaxZ - MinZ)) * 15f);
        transform.position += new Vector3(0, 0.01f, 0);
        rigidBody.velocity = new Vector3(randX * 10, Random.Range(4, 6), randZ * 10);
        moveTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
        Vector3 v = Camera.main.transform.position;
        v.y = transform.position.y;
        transform.LookAt(v);
    }

}
