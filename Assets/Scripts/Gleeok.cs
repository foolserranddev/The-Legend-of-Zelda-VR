using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Gleeok : MovingShooter
{
    [Header("Gleeok Specific")]
    public Gleeok FlyingHead;
    public GameObject[] AttachedHeads;
    public GameObject MyNeckStructure;
    public Gleeok Body;
    private bool isFlyingHead = false;
    private bool isBody;
    private bool isHead;
    private int numHeads = 0;
    private Enemy[] EnemyObjects;


    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        if (isBody) EnemyObjects = GetComponentsInChildren<Enemy>();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        //base.OnCollisionEnter(collision);
    }

    public override void OnTriggerStay(Collider col)
    {
        //base.OnTriggerStay(col);
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Sword")
        {
            if (isBody || isFlyingHead)
            {
                Debug.Log("Hit Body, Playing Sound");
                if (!audioSource.isPlaying) audioSource.Play();
            }
            else if (isHead)
            {
                float h = Body.currentHealth;
                float mh = Body.MaxHealth;
                float deathThreshold = (mh - h) >= 10 ? ((h % 6) == 0 ? 6 : h % 6) : (h - (mh - 10));

                Body.HandleContact(col);
                float newH = Body.currentHealth;
                if (newH <= 0) return;
                else if (h - newH >= deathThreshold)
                {
                    Vector3 v = transform.parent.parent.position;
                    v.y = 2.35f;
                    Gleeok p = Instantiate(FlyingHead, v, transform.parent.parent.rotation).GetComponent<Gleeok>();
                    p.transform.SetParent(Body.transform);
                    MyNeckStructure.SetActive(false);
                }
            }
        }
    }

    public override void Die(string colliderTag)
    {
        // Must deactivate for trap door to open.
        foreach (Enemy e in EnemyObjects)
        {
            e.gameObject.SetActive(false);
        }
        base.Die(colliderTag);
    }

    public override void OnEnable()
    {
        isBody = Body == null && AttachedHeads.Length > 0;
        isHead = Body != null;
        isFlyingHead = !isBody && !isHead;
        if (isBody)
        {
            if (numHeads == 0)
            {
                foreach (GameObject g in AttachedHeads)
                {
                    if (g.gameObject.activeSelf)
                    {
                        numHeads++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < numHeads; i++)
                {
                    AttachedHeads[i].gameObject.SetActive(true);
                }
                for (int i = numHeads; i < AttachedHeads.Length; i++)
                {
                    AttachedHeads[i].gameObject.SetActive(false);
                }
            }
            MaxHealth = 4 + numHeads * 6;
            currentHealth = MaxHealth;
        }
    }

    public void OnDisable()
    {
        if (isFlyingHead) Destroy(gameObject);
    }
}
