using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy {

    public int yThrowMin = 40;
    public int yThrowmax = 50;
    public int horizontalThrowStrength = 15;
    public int jumpDelay = 3;
    public int minSequentialJumps = 2;
    public int maxSequentialJumps = 4;
    private bool midJump = false;
    private float delayStart;
    private float noJumpTime;
    private bool noJump;
    private int numJumps = 0;
    private int jumpsInSequence;


    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (stunned || ClockStun) return;
        if (!midJump)
        {
            float randX = Random.Range((Mathf.Max((transform.localPosition.x - MinX),0) / (MaxX - MinX)) * -horizontalThrowStrength, (Mathf.Max((MaxX - transform.localPosition.x),0) / (MaxX - MinX)) * horizontalThrowStrength);
            float randZ = Random.Range((Mathf.Max((transform.localPosition.z - MinZ),0) / (MaxZ - MinZ)) * -horizontalThrowStrength, (Mathf.Max((MaxZ - transform.localPosition.z),0) / (MaxZ - MinZ)) * horizontalThrowStrength);
            transform.position += new Vector3(0, 0.01f, 0);
            rigidBody.velocity = new Vector3(randX, Random.Range(yThrowMin, yThrowmax), randZ);
            Vector3 v = Camera.main.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(Camera.main.transform.position - v);
            numJumps += 1;
            if (numJumps == 1) jumpsInSequence = Random.Range(minSequentialJumps, maxSequentialJumps);
            midJump = true;
        }
        else if (rigidBody.velocity == Vector3.zero)
        {
            if (numJumps == jumpsInSequence && !noJump)
            {
                delayStart = Time.time;
                noJump = true;
                noJumpTime = jumpDelay;
            }
            else if (noJump)
            {
                if (Time.time - delayStart > noJumpTime)
                {
                    noJump = false;
                    numJumps = 0;
                }
            }
            else
            {
                midJump = false;
            }
        }
    }

    public override void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);
    }

}
