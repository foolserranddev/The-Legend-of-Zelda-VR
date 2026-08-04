using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class MonsterRoomReveal : MonoBehaviour {

    public AudioClip revealSound;
    public GameObject MonsterRoom;
    private AudioSource audioSource;
    private Rigidbody rigidBody;
    private Collider thisCollider;
    private Enemy[] mobs;
    private bool revealed = false;
    private MeshRenderer mr;

	// Use this for initialization
	void Awake ()
    {
        //while (SaveData.saveData == null) ;
        //if (SaveData.saveData.OverworldSecretsRevealed[(int)dungeonEnum])
        //{
        //    Destroy(gameObject);
        //    return;
        //}
            
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = 0;
        rigidBody.drag = 10;
        thisCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        mobs = MonsterRoom.GetComponentsInChildren<Enemy>();
        for (int i = 0; i < mobs.Length; i++)
        {
            if (mobs[i].GetComponent<Bubble>() != null) mobs[i] = null;
        }
        mr = GetComponent<MeshRenderer>();
        if (mr == null) mr = GetComponentInChildren<MeshRenderer>();
        OnDisable();
    }

    public void Init()
    {
        revealed = false;
        thisCollider.enabled = false;
        mr.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void OnDisable()
    {
        Init();
    }

    // Update is called once per frame
    void Update ()
    {
		if (!revealed && MonsterRoom.activeSelf)
        {
            bool reveal = true;
            foreach(Enemy e in mobs)
            {
                if (e == null) continue;
                if (e.gameObject.activeSelf)
                {
                    reveal = false;
                    break;
                }
            }
            if (reveal && NoNewMonstersSpawned() && StandardStuff.getQuadrant(transform.position) == Player.player.quadrant)
            {
                revealed = true;
                thisCollider.enabled = true;
                audioSource.clip = revealSound;
                audioSource.loop = false;
                audioSource.Play();
                rigidBody.constraints = ~RigidbodyConstraints.FreezePositionY;
                mr.enabled = true;
            }
        }
        else if (revealed && StandardStuff.getQuadrant(transform.position) != Player.player.quadrant)
        {
            Init();
        }

    }

    private bool NoNewMonstersSpawned()
    {
        Enemy[] mobcheck = MonsterRoom.GetComponentsInChildren<Enemy>();
        foreach (Enemy e in mobcheck)
        {
            if (e.gameObject.activeSelf && e.GetComponent<Bubble>() == null && e.GetComponent<Collectible>() == null) // Fairies use the MovingShooter code
            {
                return false;
            }
        }
        return true;
    }

}
