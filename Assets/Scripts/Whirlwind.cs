using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlwind : MonoBehaviour
{

    public GameObject destination;
    public float speed = 15;
    public float speedIncrease = 25;

    private bool Captured;
    private float currSpeed;
    private Vector3 startPos;
    private Rigidbody rigidBody;

	// Use this for initialization
	void Start ()
    {
        startPos = transform.position;
        currSpeed = speed;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = new Vector3(-1, 0, 0) * currSpeed * Time.deltaTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Captured)
        {
            rigidBody.velocity = (destination.transform.position - transform.position).normalized * currSpeed * Time.deltaTime;
            Player.player.transform.position = transform.position - Player.player.playerPlayspaceOffset;
            if (Vector3.Distance(transform.position, destination.transform.position) < 0.5f)
            {
                Captured = false;
                Player.player.transform.position = destination.transform.position - Player.player.playerPlayspaceOffset;
                startPos = destination.transform.position + new Vector3(16, 0, 0);
                rigidBody.velocity = new Vector3(-1, 0, 0) * speed * Time.deltaTime;
                Player.player.leftHand.gameObject.SetActive(true);
                Player.player.rightHand.gameObject.SetActive(true);
                Player.player.GetComponent<Collider>().enabled = true;
                Player.player.Mobilize();
            }
        }
        else if (Vector3.Distance(startPos, transform.position) > 32)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            rigidBody.velocity = new Vector3(-1, 0, 0) * currSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && Player.player.isMobile)
        {
            Player.player.Immobilize();
            Player.player.leftHand.gameObject.SetActive(false);
            Player.player.rightHand.gameObject.SetActive(false);
            Player.player.GetComponent<Collider>().enabled = false;
            Captured = true;
            currSpeed += speedIncrease;
            GetComponent<Collider>().enabled = false;
            transform.position = Player.player.transform.position - Player.player.playerPlayspaceOffset;
            rigidBody.velocity = (destination.transform.position - transform.position).normalized * currSpeed * Time.deltaTime;
        }
    }
}
