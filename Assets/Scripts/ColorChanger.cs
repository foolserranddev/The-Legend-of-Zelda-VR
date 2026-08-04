using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material[] MaterialsToChange;
    public Color[] destinationColors;
    public float[] timeToDestination;
    private Material[] newMats;

	// Use this for initialization
	void Start ()
    {
        newMats = new Material[MaterialsToChange.Length];
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < MaterialsToChange.Length; i++)
        {
            newMats[i] = new Material(MaterialsToChange[i]);
            foreach (Renderer r in rs)
            {
                if (r.sharedMaterial == MaterialsToChange[i])
                {
                    r.sharedMaterial = newMats[i];
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		for (int i = 0; i < newMats.Length; i++)
        {
            newMats[i].color = Color.Lerp(MaterialsToChange[i].color, destinationColors[i], Mathf.PingPong(Time.time, timeToDestination[i])/ timeToDestination[i]);
        }
	}
}
