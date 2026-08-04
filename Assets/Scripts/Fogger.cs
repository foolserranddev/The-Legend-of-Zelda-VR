using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fogger : MonoBehaviour {

    public float fogRate = 0.01f;
    public float fogMax = 0.2f;
    public string Quadrant;
    public GameObject Floor;
    public GameObject WhiteWalls;
    public GameObject Teleporter;

    private bool fogging = false;
    private float startFog;

    private const float FLOAT_PLAY = 0.001f;
	// Use this for initialization
	void Start () {
        startFog = RenderSettings.fogDensity;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (fogging && RenderSettings.fogDensity < fogMax - FLOAT_PLAY)
        {
            RenderSettings.fogDensity = Mathf.Min(RenderSettings.fogDensity + fogRate * Time.deltaTime, fogMax);
        }
        else if (fogging && !Floor.activeSelf) // Must be done incrementing fog, so turn on all the white stuff.
        {
            Floor.SetActive(true);
            WhiteWalls.SetActive(true);
        }
        else if (!fogging && RenderSettings.fogDensity > startFog + FLOAT_PLAY)
        {
            RenderSettings.fogDensity = Mathf.Max(RenderSettings.fogDensity - fogRate * Time.deltaTime, startFog);
        }
        if (fogging && !Player.player.quadrant.Equals(Quadrant))
        {
            Disable();
        }
    }

    private void Disable()
    {
        fogging = false;
        Floor.SetActive(false);
        WhiteWalls.SetActive(false);
        Teleporter.SetActive(false);
        if (Player.player.quadrant.Equals("A8")) RenderSettings.fogDensity = startFog;
    }

    public void OnDisable()
    {
        Disable();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            fogging = true;
            //RenderSettings.fog = true;
            Teleporter.SetActive(true);
        }
    }

    //private void OnTriggerExit(Collider col)
    //{
    //    if (col.tag == "Player")
    //    {
    //        fogging = false;
    //    }
    //}
}
