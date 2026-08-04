using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DryWater : FluteTrigger
{
    public GameObject Water;
    //public Material newMaterial;
    private TextureSwapper textureSwapper;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        textureSwapper = GetComponent<TextureSwapper>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Just Triggered - Beginning Activation
        if (responding && !activated)
        {
            activated = true;
            textureSwapper.Trigger();
            Water.SetActive(false);
        }
        // Response in Progress
        else if (responding)
        {
            if (textureSwapper.stop)
            {
                GetComponent<DoorReveal>().Reveal(false);
                responding = false;
            }
        }
        // Player Left Area, Close it up.
        else if (activated && Player.player?.quadrant != quadrant)
        {
            activated = false;
            Water.SetActive(true);
            responding = false;
        }
	}
}
