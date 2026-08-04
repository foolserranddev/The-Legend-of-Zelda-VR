using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatGoblin : MonoBehaviour {

    public DestroyStuff destroyer;
    public TrapDoor trapDoor;

    private AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Meat")
        {
            Debug.Log("Meat Triggered");
            GetComponent<Collider>().enabled = false;
            audioSource.Play();
            col.transform.parent = transform;
            col.GetComponent<Rigidbody>().velocity = (transform.position + Vector3.up - col.transform.position).normalized * 3;
            ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Meat].val = false;
            Player.player.ChangeSecondary(true);
            StatusWindow.statusWindow.Refresh();
            trapDoor.TriggerOpen();
            destroyer.GetReadyToDestroy();
        }
    }
}
