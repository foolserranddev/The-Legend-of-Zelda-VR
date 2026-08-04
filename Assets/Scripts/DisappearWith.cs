using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearWith : MonoBehaviour {

    public GameObject dissappearWith;

	// Use this for initialization
	void Start ()
    {
		if (dissappearWith.GetComponent<Collectible>().obtained())
        {
            Destroy(dissappearWith);
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
