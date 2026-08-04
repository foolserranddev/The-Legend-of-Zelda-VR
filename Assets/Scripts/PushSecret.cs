using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSecret : MonoBehaviour {

    public bool stayRevealed = false;
    public TrapDoor [] TrapDoorsToOpen;
    public GameObject RevealObject;
    public GameObject HideObject;
    public GameObject MonsterRoom;
    public bool RequireBracelet = false;
    public bool allowNorth = true;
    public bool allowSouth = true;
    public bool allowEast = true;
    public bool allowWest = true;
    public float centerXOffset = 0f;
    public float centerZOffset = 0f;
    public float pushDistanceFromCenter = 0.8f;
    public float speed = 1;
    private Vector3 startPos;
    private Enemy[] mobs;
    private bool moveable = false;
    private Rigidbody rigidBody;
    private AudioSource audioSource;
    public SaveVar<bool> Revealed = new SaveVar<bool>(false);
    private bool[] allowable;
    private string quadrant;
    private bool isReset;
    private BoxCollider boxCollider;

    // Use this for initialization
    void Start ()
    {
        allowable = new bool[] { allowNorth, allowSouth, allowEast, allowWest };
        startPos = transform.position;
        quadrant = StandardStuff.getQuadrant(transform.position);
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = 1000000;
        boxCollider = GetComponent<BoxCollider>();
        if (MonsterRoom != null) mobs = MonsterRoom.GetComponentsInChildren<Enemy>(true);
        audioSource = GetComponent<AudioSource>();

        if (stayRevealed)
        {
            SaveData.saveData.data.registerBool(StandardStuff.getName(transform), Revealed);
            if (Revealed.val)
            {
                Reveal();
            }
        }
    }

    private void OnDisable()
    {
        Reset();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isReset && (Player.player.isUnderground || Player.player.quadrant != quadrant))
        {
            Reset();
        }
        else if (moveable && Vector3.Distance(transform.position, startPos) >= 1)
        {
            Debug.Log("Pushed Far Enough to Reveal");
            audioSource.Play();
            Reveal();
            moveable = false;
            rigidBody.mass = 1000000;
            rigidBody.drag = 1;
            rigidBody.angularDrag = 0.05f;
            rigidBody.velocity = Vector3.zero;
        }
        else if (moveable)
        {
            Vector3 p = Camera.main.transform.position;
            p.y = 0;
            Vector3 t = transform.position + new Vector3(centerXOffset, 0, centerZOffset);
            t.y = 0;
            if (Vector3.Distance(transform.position, startPos) >= 0.25)
            {
                rigidBody.drag = 0;
                rigidBody.angularDrag = 0;
                rigidBody.velocity = new Vector3(allowEast ? -1 : allowWest ? 1 : 0, 0, allowNorth ? -1 : allowSouth ? 1 : 0) * speed;
            }
            else if (Vector3.Distance(p, t) > pushDistanceFromCenter)
            {
                Reset();
            }
        }
        //else
        //{
        //    Debug.Log("Preventing Push");
        //    rigidBody.mass = 1000000;
        //    rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        //}

    }

    public void Reset()
    {
        transform.position = startPos;
        moveable = false;
        rigidBody.mass = 1000000;
        isReset = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        allowNorth = allowable[0];
        allowSouth = allowable[1];
        allowEast = allowable[2];
        allowWest = allowable[3];
        rigidBody.drag = 1;
        rigidBody.angularDrag = 0.05f;
        rigidBody.velocity = Vector3.zero;
        if (!stayRevealed)
        {
            if (RevealObject != null) RevealObject.SetActive(false);
            if (HideObject != null) HideObject.SetActive(true);
        }
        isReset = true;
    }

    public void Reveal()
    {
        Revealed.val = true;
        foreach (TrapDoor td in TrapDoorsToOpen)
        {
            td.TriggerOpen();
        }
        if (RevealObject != null)
        {
            RevealObject.SetActive(true);
            Teleport t = RevealObject.GetComponent<Teleport>();
            if (t != null && !stayRevealed) t.triggered();
        }
        if (HideObject != null) HideObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && Vector3.Distance(transform.position, startPos) < 1)
        {
            if (!moveable && !(MonsterRoom != null && AreThereMonstersAlive()) 
                && !(RequireBracelet && !ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Bracelet].val)) checkMovement();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            if (transform.localEulerAngles.x > 260)
            {
                if (boxCollider.size.z > 0) boxCollider.size -= new Vector3(0, 0, 0.001f);
                if (boxCollider.size.z < 0) throw new Exception("*********WARNING********* Push Secret Rotated Funny! " + transform.name);
            }
            else
            {
                if (boxCollider.size.y > 0) boxCollider.size -= new Vector3(0, 0.01f, 0);
                if (boxCollider.size.y < 0) throw new Exception("*********WARNING********* Push Secret Rotated Funny! " + transform.name);
            }
        }
    }

    private void checkMovement()
    {
        Debug.Log("Checking if movement direction allowed");
        Vector3 p = Camera.main.transform.position;
        Vector3 t = transform.position + new Vector3(centerXOffset, 0, centerZOffset);
        if (allowNorth && p.z > t.z && p.x < t.x + 0.5f && p.x > t.x - 0.5f)
        {
            Debug.Log("Allowing North");
            isReset = false;
            allowSouth = false;
            allowEast = false;
            allowWest = false;
            moveable = true;
            rigidBody.mass = 5;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
        }
        else if (allowSouth && p.z < t.z && p.x < t.x + 0.5f && p.x > t.x - 0.5f)
        {
            Debug.Log("Allowing South");
            isReset = false;
            allowNorth = false;
            allowEast = false;
            allowWest = false;
            moveable = true;
            rigidBody.mass = 5;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
        }
        else if(allowEast && p.x > t.x  && p.z < t.z + 0.5f && p.z > t.z - 0.5f)
        {
            Debug.Log("Allowing East");
            isReset = false;
            allowNorth = false;
            allowSouth = false;
            allowWest = false;
            moveable = true;
            rigidBody.mass = 5;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else if (allowWest && p.x < t.x  && p.z < t.z + 0.5f && p.z > t.z - 0.5f)
        {
            Debug.Log("Allowing West");
            isReset = false;
            allowNorth = false;
            allowSouth = false;
            allowEast = false;
            moveable = true;
            rigidBody.mass = 5;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            Debug.Log("Disallowing Motion Player at " + p + " and block at " + t);
            rigidBody.mass = 1000000;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private bool AreThereMonstersAlive()
    {
        Debug.Log("Checking Monsters");
        if (MonsterRoom.activeSelf)
        {
            foreach (Enemy e in mobs)
            {
                if (!e.dead && !e.invulnerable)
                {
                    Debug.Log("Found Monster " + e.name);
                    return true;
                }
            }
        }
        Debug.Log("No Monsters Found");
        return false;
    }

}
