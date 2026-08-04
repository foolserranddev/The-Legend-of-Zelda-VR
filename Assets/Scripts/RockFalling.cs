using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFalling : MonoBehaviour {

    public GameObject boulder;
    public float spawnRate = 1f;
    public float maxXangle = 0.1f;
    public float maxZangle = 0.1f;
    public float maxYangle = 0.1f;
    public float minX = 0.5f;
    public float maxX = 14.5f;
    public float minZ = 0.5f;
    public float maxZ = 0.5f;
    public float minY = 15f;
    public float maxY = 17f;


    private float timeToSpawn;

	// Use this for initialization
	void Start ()
    {
        Spawn();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time > timeToSpawn) Spawn();
	}

    private void Spawn()
    {
        Vector3 startPoint = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        Vector3 vel = new Vector3(
            (startPoint.x > 8 ? -1 : 1) * Random.Range(maxXangle / 2, maxXangle),
            Random.Range(maxYangle / 2, maxYangle),
            (startPoint.z > 8 ? -1 : 1) * Random.Range(maxZangle / 2, maxZangle));
        startPoint = startPoint + transform.position;
        GameObject go = Instantiate(boulder, startPoint, boulder.transform.rotation);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.velocity = vel;
        timeToSpawn = Time.time + spawnRate;
        go.transform.SetParent(transform);
    }
}
