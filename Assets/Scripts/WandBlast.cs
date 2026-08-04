using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandBlast : Projectile
{
    public GameObject [] pieces;
    public float[] rotationSpeeds;
    public int rotateAxis;

    private GameObject fire;

    public void Awake()
    {
        fire = impactAnimation;
        impactAnimation = null;
    }
    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
        if (impactAnimation == null && ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Book].val) impactAnimation = fire;
        Vector3 rot = Vector3.zero;
        rot[rotateAxis] = 1;

        for (int i = 0; i < pieces.Length; i++)
        {
            try
            {
                pieces[i].transform.Rotate(rot * rotationSpeeds[i] * Time.deltaTime);
                transform.LookAt(rigidBody.velocity + transform.position);
            }
            catch
            {

            }
        }
    }
}
