using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBillboard : MonoBehaviour
{
    public float FadeInDuration = 2;
    public float SolidDuration = 2;
    public float FadeOutDuration = 100;
    public float FlightDistance = 30;
    public float FlightSpeed = 5;

    public MeshRenderer[] meshRenderers;

    private float SolidTime;
    private float FadeOutTime;
    private float DisappearTime;
    private float lerpTime;
    // Use this for initialization
    void Start ()
    {
        if (FadeInDuration > 0) SolidTime = Time.time + FadeInDuration;
        if (FadeOutTime > 0) FadeOutTime = SolidTime + SolidDuration;
        DisappearTime = FadeOutTime + FadeOutDuration;
        lerpTime = 0;

    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 translateTarget = (Camera.main.transform.forward * FlightDistance) + Camera.main.transform.position;
        Vector3 translateTargetDirection = translateTarget - transform.position;
        Vector3 translateWithSpeed = translateTargetDirection.normalized * FlightSpeed/1000 * Vector3.Distance(transform.position, translateTarget);
        transform.position += translateWithSpeed;

        if (DisappearTime != 0 && Time.time > DisappearTime)
        {
            Destroy(gameObject);
        }
        else if (FadeOutTime != 0 && Time.time > FadeOutTime)
        {
            // Lerp Fade Out
        }
        else if (SolidTime != 0 && Time.time > SolidTime)
        {
            // Do Nothing
        }
        else
        {
            // Lerp Fade in
        }
	}
}
