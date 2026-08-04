using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patra : MovingShooter
{
    [Header("Patra Specific")]
    public GameObject PatraBabiesMainRoot;
    public GameObject PatraBabyRoot;
    public Patra FatherPatra;
    private Patra [] BabyPatras;
    public bool expand = true;
    public float SpinSpeed = 5f;
    public float maxDistance = 4f;
    public float minDistance = 2f;
    public float expandRate = 5;
    public float expandFrequency = 6f;
    private int expandDirection = 0;
    private int expandCount = 0;
    private float expandTime;
    private string location;
    private float rootRotationTime = 0.5f;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        location = StandardStuff.getQuadrant(transform.position);
        if (PatraBabyRoot != null)
        {
            BabyPatras = PatraBabyRoot.GetComponentsInChildren<Patra>();
            expandTime = Time.time + expandFrequency;
        }
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();
		if (PatraBabyRoot != null)
        {
            if (FatherPatra.dead)
            {
                Die("NA");
                return;
            }
            PatraBabyRoot.transform.Rotate(Vector3.up * SpinSpeed * Time.deltaTime);

            if (expand)
            {
                if (expandDirection == 0 && Time.time > expandTime && location == Player.player.quadrant)
                {
                    expandDirection = 1;
                }
                if (expandDirection != 0)
                {
                    if (location != Player.player.quadrant)
                    {
                        expandDirection = -1;
                        expandCount = 2;
                    }
                    foreach (Patra p in BabyPatras)
                    {
                        Vector3 v = FatherPatra.transform.position - p.transform.position;
                        v *= (1 + ((expandRate * Time.deltaTime) * expandDirection));
                        p.transform.position = FatherPatra.transform.position + v;
                    }
                    if (expandDirection == 1 && Vector3.Distance(FatherPatra.transform.position, BabyPatras[0].transform.position) >= maxDistance ||
                        expandDirection == -1 && Vector3.Distance(FatherPatra.transform.position, BabyPatras[0].transform.position) <= minDistance)
                    {
                        if (expandDirection == -1) expandCount++;
                        if (expandCount == 3)
                        {
                            expandCount = 0;
                            expandDirection = 0;
                            expandTime = Time.time + expandFrequency;
                        }
                        else
                        {
                            expandDirection *= -1;
                        }
                    }
                }
            }
            // Special Rotation Instead
            else if (Time.time > expandTime)
            {
                if (expandDirection == 0) expandDirection = 1;
                rootRotationTime += Time.deltaTime * expandDirection * 1.5f;
                if (rootRotationTime <= 0)
                {
                    expandDirection = 1;
                    rootRotationTime = 0;
                }
                else if (rootRotationTime > 1)
                {
                    expandDirection = -1;
                    rootRotationTime = 1;
                }
//                rootRotationTime = Mathf.Abs(rootRotationTime % 2 - 1);
                if (Time.time > expandTime + 4 && Mathf.Abs(rootRotationTime - 0.5f) < 0.01f)
                {
                    rootRotationTime = 0.5f;
                    expandTime = Time.time + expandFrequency;
                }
                float z = Mathf.Lerp(-60, 60, rootRotationTime);
                PatraBabiesMainRoot.transform.localEulerAngles = new Vector3(0, 0, z);
            }
            if (FatherPatra.invulnerable)
            {
                bool stayInvulnerable = false;
                foreach (Patra p in BabyPatras)
                {
                    if (!p.dead) stayInvulnerable = true;
                }
                if (!stayInvulnerable) FatherPatra.invulnerable = false;
            }
        }
    }
}
