using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftRide : MonoBehaviour
{
    public GameObject raft;
    public GameObject destination;
    public float speed;
    public float arrivalDistanceThreshold = 0.1f;
    public RaftRide returnRide;
    public float RaftHeight = 0.3f;

    private bool riding = false;
    private Vector3 startPos;
    private Vector3 direction;
    private Rigidbody raftRigidBody;
    private AudioSource audioSource;
    private Collider myCollider;

	// Use this for initialization
	void Start ()
    {
        startPos = raft.transform.position;
        audioSource = GetComponent<AudioSource>();
        raftRigidBody = raft.GetComponent<Rigidbody>();
        direction = destination.transform.position - raft.transform.position;
        direction *= speed;
        myCollider = GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (riding)
        {
            Vector3 pPos = Player.player.transform.position;
            Vector3 rPos = raft.transform.position;
            Vector3 dPos = destination.transform.position;
            float distance = Vector3.Distance(rPos, dPos);

            if (distance < arrivalDistanceThreshold)
            {
                riding = false;
                raft.SetActive(false);
                raft.transform.position = startPos;
                raftRigidBody.velocity = Vector3.zero;
                pPos.x = dPos.x;
                pPos.z = dPos.z;
                Player.player.transform.position = pPos - Player.player.playerPlayspaceOffset;
                Player.player.Mobilize();
                returnRide.gameObject.SetActive(true);
                myCollider.enabled = true;
            }
            else
            {
                pPos.x = rPos.x;
                pPos.z = rPos.z;
                pPos.y = rPos.y + RaftHeight;
                Player.player.transform.position = pPos - Player.player.playerPlayspaceOffset;
                raftRigidBody.velocity = direction;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!riding && col.tag == "Player" && ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Raft].val)
        {
            Player.player.Immobilize();
            raft.SetActive(true);
            audioSource.Play();
            riding = true;
            myCollider.enabled = false;
            returnRide.gameObject.SetActive(false);
        }
    }
 
    private void OnCollisionEnter(Collision col)
    {
        OnTriggerEnter(col.collider);
    }
}
