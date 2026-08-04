//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public enum ItemType
    {
        Heart,
        Rupees,
        Bombs,
        Fairy,
        Clock,
        HeartContainer,
        Triforce,
        Key,
        Compass,
        Map,
        Bracelet
    }
    public bool OneTimeCollectible = false;
    public ItemType item;
    public int amount = 1;
    public AudioClip collectSound;
    public bool ArrowOrBoomerangCanCollect = true;
    public float heightOffset = 0f;
    public float jiggleheight = 0.25f;
    public float timeToPeakHeight = 1;
    public float rotationPerSecond = 90f;
    public float TimeToDestroy = 10f;
    public Vector3 RotateAround = new Vector3(0, 1, 0);
    public bool DontDestroy = false;
    public DestroyStuff Destroyer;
    public TrapDoor[] OpenTrapDoors;
    public bool restartMusic = false;
    [Header("Collectible Movement")]
    public bool isMobile = false;
    public float MinX = 0.5f;
    public float MinZ = 0.5f;
    public float MaxX = 15.5f;
    public float MaxZ = 15.5f;
    public float speed = 3;
    public float minSpeed = 2;
    public float minMoveTime = 2f;
    public float maxMoveTime = 4f;

    public SaveVar<bool> ReceivedItem = new SaveVar<bool>(false);
    private bool jiggleUp = true;
    private float jiggleMax, jiggleMin;
    private bool initialized = false;
    private bool registered = false;
    private Rigidbody rigidBody;
    private float moveTime;

    // Use this for initialization
    void Start()
    {
        jiggleMax = transform.position.y + jiggleheight;
        jiggleMin = transform.position.y;
        if (TimeToDestroy != 0) TimeToDestroy += Time.time;
        if (!registered && OneTimeCollectible) SaveData.saveData.data.registerBool(StandardStuff.getName(transform), ReceivedItem);
        if (ReceivedItem.val) Destroy(gameObject);
        initialized = true;
        registered = true;
        if (isMobile) rigidBody = GetComponent<Rigidbody>();
    }

    public bool obtained()
    {
        if (!registered && OneTimeCollectible) SaveData.saveData.data.registerBool(StandardStuff.getName(transform), ReceivedItem);
        registered = true;
        return ReceivedItem.val;
    }

    public void SetAmount(int purchaseAmount)
    {
        amount = purchaseAmount;
    }

    void Update()
    {
        if ((TimeToDestroy != 0 && Time.time > TimeToDestroy) || (!DontDestroy && ReceivedItem.val))
        {
            Destroy(gameObject);
            return;
        }

        if (jiggleheight > 0)
        {
            if (jiggleUp)
            {
                transform.position += new Vector3(0, 1 / timeToPeakHeight * jiggleheight * Time.deltaTime / 2, 0);
                if (transform.position.y >= jiggleMax && !ReceivedItem.val)
                {
                    jiggleUp = false;
                }
            }
            else if (!ReceivedItem.val) // If item is received and we get here, then it's a Triforce which just slowly goes upward now
            {
                transform.position += new Vector3(0, -1 / timeToPeakHeight * jiggleheight * Time.deltaTime / 2, 0);
                if (transform.position.y <= jiggleMin)
                {
                    jiggleUp = true;
                }
            }
        }
        transform.Rotate(RotateAround, rotationPerSecond * Time.deltaTime);

        if (isMobile)
        {
            float x = transform.localPosition.x;
            float z = transform.localPosition.z;
            float currSpeed = Vector3.Distance(rigidBody.velocity, Vector3.zero);
            if (Time.time > moveTime || x < MinX || x > MaxX || z < MinZ || z > MaxZ || currSpeed < minSpeed)
            {
                NewDirection();
            }
        }

    }

    private void NewDirection()
    {
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        float randX = Random.Range(x < MinX ? 0f : -1f, x > MaxX ? 0f : 1f) * speed;
        float randZ = Random.Range(z < MinZ ? 0f : -1f, z > MaxZ ? 0f : 1f) * speed;
        Vector3 vel = new Vector3(randX, 0, randZ);
        rigidBody.velocity = vel * (speed / Vector3.Distance(vel, Vector3.zero));
        moveTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
    }

    public int getAmount()
    {
        if (initialized) ReceivedItem.val = true;
        //jiggleUp = true;
        //rotationPerSecond *= 2;
        int a = amount;
        amount = 0;
        foreach (TrapDoor td in OpenTrapDoors)
        {
            Debug.Log("Triggering Trap Doors");
            td.TriggerOpen();
        }
        if (restartMusic) Player.player.StartMusic();
        return a;
    }

    public void Received()
    {
        ReceivedItem.val = true;
        if (Destroyer != null) Destroyer.GetReadyToDestroy();
    }

    public AudioClip getCollectSound() { return collectSound; }

}
