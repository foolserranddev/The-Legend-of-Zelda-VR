using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gohma : MovingShooter
{
    [Header("Gohma Specific")]
    public float MaxEyeRotation = 85;
    public float MinEyeRotation = 0;
    public float EyeRotationSpeed = 5;
    public GameObject Eyelid;

    private bool EyeOpen = true;
    private int eyeDirection = 0;
    private float eyeToggle;
    private float eyeRotation;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();
        if (eyeDirection != 0)
        {
            Eyelid.transform.Rotate(new Vector3(0, Time.deltaTime * EyeRotationSpeed * eyeDirection));
            float rotY = Eyelid.transform.localEulerAngles.y;
            if (rotY < MinEyeRotation)
            {
                eyeDirection = 0;
                eyeToggle = Time.time + 5f;
//                Eyelid.transform.Rotate(new Vector3(0, rotY - MaxEyeRotation, 0));
            }
            else if (Eyelid.transform.localEulerAngles.y > MaxEyeRotation)
            {
                eyeDirection = 0;
                EyeOpen = true;
                eyeToggle = Time.time + 2f;
//                Eyelid.transform.Rotate(new Vector3(0, MinEyeRotation - rotY, 0));
            }
        }
        else if (Time.time > eyeToggle)
        {
            if (EyeOpen)
            {
                eyeDirection = -1;
                EyeOpen = false;
            }
            else
            {
                eyeDirection = 1;
            }
        }
	}

    public override void OnTriggerStay(Collider col)
    {
        //base.OnTriggerStay(col);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        eyeToggle = Time.time;
    }

    public override void HandleContact(Collider col)
    {
        if (col.tag == "Arrow" && EyeOpen && Mathf.Abs((col.transform.position - transform.position).x) < 0.5f)
        {
            invulnerable = false;
        }
        base.HandleContact(col);
        invulnerable = true;
        eyeToggle = 0;
    }
}
