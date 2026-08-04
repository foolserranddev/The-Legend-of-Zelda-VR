using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : InteractableObject {

    public GameObject firePrefab;
    public bool isRedCandle = false;
    public AudioClip fireSound;
    public GameObject flame;
    public Color[] flames;
    public float timeBetweenFire = 1.25f;

    private AudioSource audioSource;
    private float timeDelay;
    private bool readyToFlame = true;
    private int lastTransitionCount;
    private HandController hand;

    public float VelocityLow = 0.5f;
    public float VelocityHigh = 2;
    //private bool velocityAboveThreshold;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        timeDelay = Time.time;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }
	
	// Update is called once per frame
	void Update ()
    {
        readyToFlame = lastTransitionCount != Player.player.roomTransitionCount || itemLevelIndex == 1;
        flame.SetActive(readyToFlame);
    }

    public override void UpdateLevel(int level)
    {
        base.UpdateLevel(level);
        flame.GetComponent<SpriteRenderer>().color = flames[itemLevelIndex];
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in renderers)
        {
            mr.material = material[itemLevelIndex];
        }
    }
    //public override void performTriggerUnclicked(HandController hand, ClickedEventArgs e)
    //{
    //    base.performTriggerUnclicked(hand, e);
    //    FlameOn();
    //}

    private void FlameOn()
    {
        if (Time.time > timeDelay && (itemLevelIndex == 1 || readyToFlame))
        {
            audioSource.Play();
            Vector3 loc = transform.position + transform.forward;
            loc.y = Player.player.transform.position.y;
            GameObject fire = Instantiate(firePrefab, loc, firePrefab.transform.rotation);

            Vector3 v = Camera.main.transform.position - loc;
            v.x = 0f;
            v.z = 0f;
            fire.transform.LookAt(Camera.main.transform.position - v);

            Vector3 vel = transform.forward * 2;
            vel.y = 0;
            fire.GetComponent<Rigidbody>().velocity = vel;
            lastTransitionCount = Player.player.roomTransitionCount;
            timeDelay = Time.time + timeBetweenFire;
        }
    }

    public override void performAction()
    {
        FlameOn();
    }
}
