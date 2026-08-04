using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallmasterManager : MonoBehaviour
{
    public float timeBetweenHands = 1;
    public float outsideOffset = 1;
    public float insideOffset = 3;
    public float playerSpawnOffset = 2;

    private float nextSpawnTime = 0;
    private Wallmaster[] wallMasters;
    private string quadrant = "";
    private bool on;
    private bool spawnFront = true;
    private bool isCaught;
    // Use this for initialization
	void Awake ()
    {
        wallMasters = GetComponentsInChildren<Wallmaster>();
        foreach(Wallmaster w in wallMasters)
        {
            w.gameObject.SetActive(false);
            w.SetManager(this);
        }
	}

    private void Start()
    {
        quadrant = StandardStuff.getQuadrant(transform.position);
    }

    private void OnEnable()
    {
        on = true;
        //foreach (Wallmaster w in wallMasters)
        //{
        //    w.gameObject.SetActive(false);
        //}
    }
    
    // Update is called once per frame
    void Update ()
    {
		if (on)
        {
            if (!quadrant.Equals(Player.player.quadrant))
            {
                on = false;
                foreach(Wallmaster m in wallMasters)
                {
                    if (!m.caught) m.gameObject.SetActive(false);
                }
            }
        }
	}
    
    public void SetCaught(bool b)
    {
        isCaught = b;
    }

    public bool IsCaught()
    {
        return isCaught;
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.tag != "Player") return;
        if (quadrant.Equals("")) quadrant = string.Copy(Player.player.quadrant);
        on = true;
        if (Time.time > nextSpawnTime)
        {
            Wallmaster wm = wallMasters[0];
            bool foundOne = false;
            foreach(Wallmaster w in wallMasters)
            {
                if (!w.attacking && !w.dead)
                {
                    wm = w;
                    foundOne = true;
                    break;
                }
            }
            if (!foundOne) return;
            nextSpawnTime = Time.time + timeBetweenHands;
            Vector3 startPoint = Camera.main.transform.position;
            startPoint.y = Player.player.transform.position.y;
            Vector3[] directions = new Vector3[3];
            float westD = Vector3.Distance(transform.TransformPoint(new Vector3(16, 0, 8)), Camera.main.transform.position);
            float southD = Vector3.Distance(transform.TransformPoint(new Vector3(8, 0, 16)), Camera.main.transform.position);
            float northD = Vector3.Distance(transform.TransformPoint(new Vector3(8, 0, 0)), Camera.main.transform.position);
            float eastD = Vector3.Distance(transform.TransformPoint(new Vector3(0, 0, 8)), Camera.main.transform.position);
            if (westD < southD && westD < northD && westD < eastD) // West Collider
            {
                startPoint.x = 16f + outsideOffset;
                float spawnFrontOffset = Camera.main.transform.forward.z > 0 ? playerSpawnOffset : -playerSpawnOffset;
                startPoint = transform.TransformPoint(startPoint);
                startPoint.z = Camera.main.transform.position.z + (spawnFront ? spawnFrontOffset : -spawnFrontOffset);
                directions[0] = new Vector3(-insideOffset, 0, 0);
                directions[1] = new Vector3(0, 0, spawnFront ? -spawnFrontOffset * 2 : spawnFrontOffset * 2);
            }
            else if (southD < westD && southD < eastD && southD < northD) // South Collider
            {
                startPoint.z = 16f + outsideOffset;
                float spawnFrontOffset = Camera.main.transform.forward.x > 0 ? playerSpawnOffset : -playerSpawnOffset;
                startPoint = transform.TransformPoint(startPoint);
                startPoint.x = Camera.main.transform.position.x + (spawnFront ? spawnFrontOffset : -spawnFrontOffset);
                directions[0] = new Vector3(0, 0, -insideOffset);
                directions[1] = new Vector3(spawnFront ? -spawnFrontOffset * 2 : spawnFrontOffset * 2, 0, 0);
            }
            else if (northD < southD && northD < eastD && northD < westD) // North Collider
            {
                startPoint.z = -outsideOffset;
                float spawnFrontOffset = Camera.main.transform.forward.x > 0 ? playerSpawnOffset : -playerSpawnOffset;
                startPoint = transform.TransformPoint(startPoint);
                startPoint.x = Camera.main.transform.position.x + (spawnFront ? spawnFrontOffset : -spawnFrontOffset);
                directions[0] = new Vector3(0, 0, insideOffset);
                directions[1] = new Vector3(spawnFront ? -spawnFrontOffset * 2 : spawnFrontOffset * 2, 0, 0);
            }
            else // East Collider
            {
                startPoint.x = -outsideOffset;
                float spawnFrontOffset = Camera.main.transform.forward.z > 0 ? playerSpawnOffset : -playerSpawnOffset;
                startPoint = transform.TransformPoint(startPoint);
                startPoint.z = Camera.main.transform.position.z + (spawnFront ? spawnFrontOffset : -spawnFrontOffset);
                directions[0] = new Vector3(insideOffset, 0, 0);
                directions[1] = new Vector3(0, 0, spawnFront ? -spawnFrontOffset * 2 : spawnFrontOffset * 2);
            }
            directions[2] = -directions[0];
            wm.Attack(startPoint, directions);
            //spawnFront = !spawnFront;
        }
    }
}
