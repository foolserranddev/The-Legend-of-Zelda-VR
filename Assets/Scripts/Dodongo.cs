using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodongo : MovingShooter
{
    [Header("Dodongo Specific")]
    public AudioClip ShieldBlock;
    public GameObject BigBody;
    public GameObject LittleBody;
    public GameObject BigHead;
    public GameObject LittleHead;

    private SaveVar<bool> defeated = new SaveVar<bool>(false);
    private bool exploding;
    private float explodeTime;
    private float inflateTime;
    private float deflateTime;
    private float blinkTime;
    private string deathCollider;
    private MeshRenderer[] BigRenderers = new MeshRenderer[2];
    private MeshRenderer[] LittleRenderers = new MeshRenderer[2];
    //    private AudioSource shieldAudio;
    private Dodongo[] others;
    private bool RolledForInitiative = false;

    public override void Start()
    {
        base.Start();
        SaveData.saveData.data.registerBool("Dodongo", defeated);
        if (defeated.val) gameObject.SetActive(false);
        BigRenderers[0] = BigBody.GetComponent<MeshRenderer>();
        BigRenderers[1] = BigHead.GetComponent<MeshRenderer>();
        LittleRenderers[0] = LittleBody.GetComponent<MeshRenderer>();
        LittleRenderers[1] = LittleHead.GetComponent<MeshRenderer>();
//        shieldAudio = GetComponents<AudioSource>()[2];
    }

    public void RollForAudioInitiative()
    {
        others = transform.parent.GetComponentsInChildren<Dodongo>();
        bool foundAudio = false;
        foreach (Dodongo d in others)
        {
            if (d == this) continue;
            else if (d.BossAudio.enabled) foundAudio = true;
        }
        if (foundAudio) BossAudio.enabled = false;
        RolledForInitiative = true;
    }

    public void CheckAudio()
    {
        bool foundAudio = false;
        foreach (Dodongo d in others)
        {
            if (d == this || !d.gameObject.activeSelf) continue;
            if (d.BossAudio.enabled) foundAudio = true;
        }
        if (!foundAudio) BossAudio.enabled = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //        Debug.DrawRay(transform.position + new Vector3(0, 0.3f, 0) + (transform.forward * 1.5f), transform.forward / 2, Color.green);
        if (!RolledForInitiative) RollForAudioInitiative();
        else if (others.Length > 1) CheckAudio();
        if (!stunned && currentHealth <= 0 && !dead)
        {
            Die(deathCollider);
        }
        else if (Time.time > deflateTime)
        {
            if (BigRenderers[0].enabled)
            {
                BigRenderers[0].enabled = false;
                BigRenderers[1].enabled = false;
                LittleRenderers[0].enabled = true;
                LittleRenderers[1].enabled = true;
                exploding = false;
            }
        }
        else if (stunned && (Time.time > inflateTime) && LittleRenderers[0].enabled)
        {
            BigRenderers[0].enabled = true;
            BigRenderers[1].enabled = true;
            LittleRenderers[0].enabled = false;
            LittleRenderers[1].enabled = false;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        HandleContact(collision.collider);
    }

    public override void HandleContact(Collider col)
    {
        if (col.tag == "Boomerang") return;
        if (col.tag == "BlinkySword" || col.tag == "Arrow")
        {
            audioSource.clip = ShieldBlock;
            audioSource.Play();
        }
        else if (col.tag == "Sword" && col.GetComponent<Sword>().IsSwinging())
        {
            if (!stunned)
            {
                audioSource.clip = ShieldBlock;
                audioSource.Play();
            }
            else if (stunned && !exploding)
            {
                Die(col.tag);
            }
        }
        else if (stunned) return;
        else if (col.tag == "BombLive" && col.GetComponent<Bomb>().isThrown())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0) + (transform.forward * 1.5f), transform.forward / 2, out hit) && hit.transform.tag == "BombLive")
            {
                Destroy(col.gameObject);
                stunTime = Time.time + stunAmount;
                inflateTime = Time.time + 0.4f;
                deflateTime = Time.time + 1.6f;
                exploding = true;
                stunned = true;
                currentHealth -= 1;
            }
        }
        else if (col.tag == "Bomb" && Vector3.Distance(col.transform.position, transform.position) < 2)
        {
            rigidBody.velocity = Vector3.zero;
            stunTime = Time.time + stunAmount;
            stunned = true;
            deathCollider = col.tag;
            Debug.Log("Dodongo hit with smoke at distance " + Vector3.Distance(col.transform.position, transform.position));
        }
    }
}
