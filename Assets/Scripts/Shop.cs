using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

    public GameObject[] ObjectsToDisappear;

    public void OnEnable ()
    {
        Reappear();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Disappear()
    {
        foreach (GameObject g in ObjectsToDisappear)
        {
            g.SetActive(false);
        }
    }
    
    public void Reappear()
    {
        foreach (GameObject g in ObjectsToDisappear)
        {
            g.SetActive(true);
        }
    }
}
