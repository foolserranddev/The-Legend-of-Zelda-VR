using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MovingShooter
{
    private AudioSource hitAudioSource;
    private float hurtTime;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        hitAudioSource = GetComponent<AudioSource>();
    }

    //   // Update is called once per frame
    //   public override void Update ()
    //   {
    //       base.Update();
    //   }

    private void checkHurt(Collider col)
    {
        if (col.tag == "Player")
        {
            ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Sword].GetComponent<Sword>().disableSword();
            if (Time.time > hurtTime + 1)
            {
                hitAudioSource.Play();
                hurtTime = Time.time;
            }
        }
    }
    public override void OnTriggerEnter(Collider col)
    {
        checkHurt(col);
    }
    public override void OnCollisionEnter(Collision collision)
    {
        checkHurt(collision.collider);
    }
}
