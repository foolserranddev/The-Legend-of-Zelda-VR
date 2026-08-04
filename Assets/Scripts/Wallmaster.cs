using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallmaster : Enemy {

    [HideInInspector]
    public bool attacking;
    public float speedMultiplier = 1.5f;

    private WallmasterManager manager;
    private Vector3 moveStart;
    private Vector3[] moveDirections;
    private int moveIdx;
    [HideInInspector] public bool caught;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

    }
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();
        if (attacking && !(stunned || ClockStun))
        {
            transform.position += moveDirections[moveIdx].normalized * Time.deltaTime * speedMultiplier;
            if (Vector3.Distance(transform.position, moveStart) >= Vector3.Distance(Vector3.zero, moveDirections[moveIdx]))
            {
                moveIdx++;
                if (moveIdx < 3)
                {
                    moveStart = transform.position;
                    Vector3 lookat = moveDirections[moveIdx];
                    lookat.y = transform.position.y;
                    transform.LookAt(transform.position + moveDirections[moveIdx]);
                }
                else
                {
                    attacking = false;
                    gameObject.SetActive(false);
                    //mr.enabled = false;
                    if (caught)
                    {
                        Vector3 newPos = Player.player.startPosition;
                        newPos.y = transform.position.y;
                        transform.position = newPos;
                        Player.player.transform.SetParent(StandardStuff.ss.transform);
                        Player.player.Mobilize();
                        caught = false;
                        invulnerable = false;
                        manager.SetCaught(false);
                    }
                }
            }
        }
	}

    public void Attack(Vector3 StartPoint, Vector3[] directions)
    {
        attacking = true;
        gameObject.SetActive(true);
        transform.position = StartPoint;
        moveStart = StartPoint;
        moveDirections = directions;
        moveIdx = 0;

        Vector3 lookat = moveDirections[moveIdx];
        lookat.y = transform.position.y;
        transform.LookAt(transform.position + moveDirections[moveIdx]);

        //mr.enabled = true;
    }

    public void OnDisable()
    {
        attacking = false;
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !manager.IsCaught() && !stunned)
        {
            manager.SetCaught(true);
            Player.player.Immobilize();
            invulnerable = true;
            caught = true;
            Vector3 newPos = Camera.main.transform.position;
            newPos.y -= 0.5f;
            transform.position = newPos;
            Player.player.transform.SetParent(transform);
            moveDirections[2] = new Vector3(0, 2, 0);
            moveIdx = 2;
            moveStart = transform.position;
        }
    }

    public void SetManager(WallmasterManager m)
    {
        manager = m;
    }
}
