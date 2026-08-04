using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizzrobeBlast : Projectile
{
    public GameObject [] pieces;
    public float[] rotationSpeeds;
    public int rotateAxis;

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        Vector3 rot = Vector3.zero;
        rot[rotateAxis] = 1;

        for (int i = 0; i < pieces.Length; i++)
        {
            try
            {
                pieces[i].transform.Rotate(rot * rotationSpeeds[i] * Time.deltaTime);
            }
            catch
            {

            }
        }
    }
}
