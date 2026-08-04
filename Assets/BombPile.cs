using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPile : MonoBehaviour {

    public Bomb bombPrefab;
    public Bomb attachedBomb;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider col)
    {
        if (attachedBomb == null && (col.tag == "LeftHand" || col.tag == "RightHand"))
        {
            attachedBomb = Instantiate(bombPrefab, col.transform);
            attachedBomb.InitialSetup(col.GetComponent<HandController>(), null);
            attachedBomb.SetAsEraseBomb();
        }
    }

    void OnDestroy()
    {
        DestroyImmediate(attachedBomb.gameObject);
    }
}
