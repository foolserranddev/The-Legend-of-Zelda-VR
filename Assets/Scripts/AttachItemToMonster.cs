using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachItemToMonster : MonoBehaviour
{
    public Enemy EnemyToAttach;
    public Vector3 ScaleWhileAttached;
    public Vector3 OffsetPosition;

    private Enemy currentEnemyTarget;
    private bool revealed = false;
    private Vector3 startLocation;
    private Vector3 enemyStartRotation;
    private Vector3 objectStartRotation;
    private Collider itemCollider;
    private Collectible collectible;
    private MeshRenderer mr;
    private AttachItemToMonster clone;
    private bool initialized = false;
    private bool reenabling = false;

    // Use this for initialization
    void Start ()
    {
        objectStartRotation = transform.localEulerAngles;
        currentEnemyTarget = EnemyToAttach;
        startLocation = transform.position;
        enemyStartRotation = EnemyToAttach.gameObject.transform.localEulerAngles;
        itemCollider = GetComponent<Collider>();
        collectible = GetComponent<Collectible>();
        mr = GetComponent<MeshRenderer>();
        if (mr == null) mr = GetComponentInChildren<MeshRenderer>();
    }

    private void Init()
    {

    }

    private void CloneSetup(Enemy attachment)
    {
        if (clone != null) DestroyImmediate(clone.gameObject);
        if (collectible.obtained())
        {
            Destroy(gameObject);
            return;
        }
        currentEnemyTarget = attachment;
        revealed = false;
        itemCollider.enabled = false;
        collectible.enabled = false;
        mr.enabled = true; // makes copy visible
        clone = Instantiate(this);
        clone.GetComponent<AttachItemToMonster>().enabled = false;
        Destroy(clone.GetComponent<Collider>());
        Destroy(clone.GetComponent<Rigidbody>());
        attachment.gameObject.transform.localEulerAngles = enemyStartRotation;
        clone.transform.localEulerAngles = objectStartRotation;
        clone.transform.parent = attachment.transform;
        clone.transform.localPosition = OffsetPosition;
        clone.transform.localScale = ScaleWhileAttached;
        mr.enabled = false;
    }

    void OnEnable()
    {
        reenabling = true;
        if (initialized)
        {
            if (EnemyToAttach.gameObject.activeSelf)
            {
                CloneSetup(EnemyToAttach);
            }
            else
            {
                FindNewAttachment();
            }
        }
        reenabling = false;
    }

    void OnDestroy()
    {
        if (clone != null) DestroyImmediate(clone.gameObject);
    }

    void OnDisable()
    {
        if (clone != null) DestroyImmediate(clone.gameObject);
    }

    private void FindNewAttachment()
    {
        foreach (Enemy e in EnemyToAttach.transform.parent.GetComponentsInChildren<Enemy>())
        {
            if (e.GetType() == EnemyToAttach.GetType() && e.gameObject.activeSelf)
            {
                CloneSetup(e);
                revealed = false;
                break;
            }
        }
        if (revealed && reenabling)
        {
            transform.position = startLocation;
            itemCollider.enabled = true;
            collectible.enabled = true;
            mr.enabled = true;
            if (clone != null) DestroyImmediate(clone.gameObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!initialized)
        {
            CloneSetup(EnemyToAttach);
            initialized = true;
            return;
        }

        if (revealed && Player.player?.quadrant != StandardStuff.getQuadrant(transform.position))
        {
            FindNewAttachment();
        }
        else if (!revealed)
        {
            if (!(currentEnemyTarget.gameObject.activeSelf))
            {
                revealed = true;
                itemCollider.enabled = true;
                collectible.enabled = true;
                mr.enabled = true;
                transform.position = new Vector3(currentEnemyTarget.transform.position.x, transform.position.y, currentEnemyTarget.transform.position.z);
                DestroyImmediate(clone.gameObject);
            }
        }
	}
}
