using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Basics")]
    public bool dieOnceAndForever = false;
    public bool isBoss = false;
    public bool isRingLeader = false;
    public AudioClip HitSound;
    public AudioClip BossCry;
    public AudioClip PersistentSound;
    public GameObject DeathPrefab;
    public float DeathPrefabHeightOffset = 0;
    public ItemDropper.EnemyGroup enemyGroup;
    public Material[] HitMaterials;
    [Header("Boundaries")]
    public float MinX = 0.5f;
    public float MinZ = 0.5f;
    public float MaxX = 15.5f;
    public float MaxZ = 15.5f;
    [Header("Characteristics")]
    public float MaxHealth = 0.5f;
    public float damageDealt = 0.5f;
    public float dropPercent = 0.25f;
    public bool isStunnable = true;
    public float stunAmount = 3f;
    public bool invulnerable = false;
    public bool resistArrows = false;
    public bool resistBombs = false;
    public bool resistFire = false;
    public bool resistWand = false;
    public bool partOfALargerMob = false;
    public bool weakToArrows = false;
    //[HideInInspector]
    public bool dead = false;
    [HideInInspector]public bool ClockStun = false;
    protected AudioSource audioSource;
    protected Rigidbody rigidBody;
    protected float currentHealth;
    protected string myQuadrant;

    private Renderer[] renderers;
    protected bool stunned = false;
    protected float stunTime;
    protected float hitDelay;
    protected float timeDelta = 0;
    private int index = 0;
    private Vector3 startPosition;
    private SaveVar<bool> died = new SaveVar<bool>(false);
    protected AudioSource BossAudio;
    protected AudioSource CryAudio;
    protected bool allowBlink = true;
    private string roarRooms;
    private Animation mob_animation;
    private Animator mob_animator;

    // Use this for initialization
    public virtual void Start ()
    {
        mob_animation = GetComponentInChildren<Animation>();
        mob_animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = transform.parent.GetComponent<AudioSource>();
        hitDelay = Time.time;
        startPosition = transform.position;
        myQuadrant = StandardStuff.getQuadrant(startPosition);
        if (PersistentSound != null)
        {
            BossAudio = GetComponent<AudioSource>();
            BossAudio.clip = PersistentSound;
            roarRooms = myQuadrant;
        }
        if (BossCry != null)
        {
            CryAudio = GetComponents<AudioSource>()[1];
            CryAudio.clip = BossCry;
            CryAudio.loop = false;
        }
        if (isBoss)
        {
            Vector3 p = transform.position;
            roarRooms += StandardStuff.getQuadrant(new Vector3(p.x - 16, p.y, p.z));
            roarRooms += StandardStuff.getQuadrant(new Vector3(p.x + 16, p.y, p.z));
            roarRooms += StandardStuff.getQuadrant(new Vector3(p.x, p.y, p.z - 16));
            roarRooms += StandardStuff.getQuadrant(new Vector3(p.x, p.y, p.z + 16));
        }

        // All these gymnastics are for preventing changing the material
        // of an attached item on the enemy while still allowing for the 
        // character to have multiple parts with multiple renderers like
        // the Octoroks.
        //        if (HitMaterials.Length == 0) return;
        Renderer[] ra = GetComponentsInChildren<Renderer>();
        // These low-poly, brightly colored enemies were authored before the
        // newer mobile/OpenXR shadow path. Receiving their own real-time
        // shadows causes unstable dark patches (shadow acne) as the XR camera
        // moves. Keep casting ground shadows, but do not self-shadow.
        for (int i = 0; i < ra.Length; i++)
        {
            ra[i].receiveShadows = false;
        }
        bool[] keepers = new bool[ra.Length];
        int totalRenderers = 0;
        for (int i = 0; i < ra.Length; i++)
        {
            for (int j = 0; j < HitMaterials.Length; j++)
            {
                if (ra[i].sharedMaterial == HitMaterials[j])
                {
                    if (j != 0)
                    {
                        Material m = HitMaterials[0];
                        HitMaterials[0] = HitMaterials[j];
                        HitMaterials[j] = m;
                    }
                    totalRenderers++;
                    keepers[i] = true;
                    break;
                }
            }
        }
        renderers = new Renderer[totalRenderers];
        int count = 0;
        for (int i = 0; i < ra.Length; i++)
        {
            if (keepers[i])
            {
                renderers[count] = ra[i];
                count++;
            }
        }
        Init();
    }

    private void Init()
    {
        currentHealth = MaxHealth;
        dead = false;
        if (!partOfALargerMob && startPosition != Vector3.zero)
        {
            transform.position = startPosition;
        }
        if (dieOnceAndForever)
        {
            SaveData.saveData.data.registerBool(StandardStuff.getName(transform), died);
            if (died.val)
            {
                Enemy[] childMobs = GetComponentsInChildren<Enemy>();
                foreach (Enemy e in childMobs)
                {
                    e.dead = true;
                    e.gameObject.SetActive(false);
                }
                dead = true;
                gameObject.SetActive(false);
            }
        }
    }

    public virtual void OnEnable()
    {
        if (rigidBody == null) return; // Have not gone through Start yet
        Init();
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        if (Player.player.Dead)
        {
            stunned = true;
            if (mob_animation != null) mob_animation.Stop();
            if (mob_animator != null) mob_animator.enabled = false;
            if (rigidBody != null) rigidBody.velocity = Vector3.zero;
            return;
        }
        if (dieOnceAndForever && myQuadrant != Player.player.quadrant) currentHealth = MaxHealth;
        if (isBoss || roarRooms != null)
        {
            if (roarRooms.Contains(Player.player?.quadrant))
            {
                if (Player.player.quadrant.Equals(myQuadrant)) BossAudio.volume = 1;
                else BossAudio.volume = 0.5f;
                if (!BossAudio.isPlaying)
                {
                    BossAudio.loop = true;
                    BossAudio.Play();
                }
            }
            else
            {
                BossAudio.volume = 0.5f;
                BossAudio.loop = false;
            }
        }
        if (stunned || ClockStun)
        {
            ClockStun = ClockStun && myQuadrant == Player.player.quadrant;
            if (Time.time > stunTime) stunned = false;
            if (rigidBody != null) rigidBody.velocity = Vector3.zero;
            if (!stunned && !ClockStun)
            {
                if (mob_animation != null) mob_animation.Play();
                if (mob_animator != null) mob_animator.enabled = true;
            }
        }
        if (ClockStun && allowBlink || hitDelay > Time.time && HitMaterials.Length > 1)
        {
            timeDelta += Time.deltaTime;
            if (timeDelta >= 0.05f)
            {
                index = (index + 1) % HitMaterials.Length;
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material = HitMaterials[index];
                }
                timeDelta = 0;
            }
        }
        else if (index != 0)
        {
            index = 0;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = HitMaterials[index];
            }
        }
    }

    public virtual void HandleContact(Collider col)
    {
        if (invulnerable 
            || resistArrows && col.tag == "Arrow"
            || resistFire && col.tag == "Fire"
            || resistBombs && col.tag == "Bomb"
            || resistWand && col.tag == "BlinkySword") return;
        if (col.tag == "Boomerang")
        {
            if (col.GetComponent<Boomerang>().IsThrown() && isStunnable)
            {
                Debug.Log("Stunning");
                stunTime = Time.time + stunAmount;
                stunned = true;
                if (currentHealth > 0) return;
            }
            else
            {
                return;
            }
        }

        if (Time.time > hitDelay)
        {
            float damage = 0;
            if (col.tag == "Arrow" && weakToArrows) damage = MaxHealth;
            else if ((col.tag == "Sword" && col.GetComponent<Sword>().IsSwinging()) || col.tag == "BlinkySword" || col.tag == "Arrow") damage = col.GetComponent<InteractableObject>().damageDealt();
            else if (col.tag == "Bomb") damage = 2;
            else if (col.tag == "Fire") damage = 0.5f;


            if (damage > 0 || col.tag == "Boomerang")
            {
//                Debug.Log("Damage = " + damage + " Health = " + currentHealth);
                currentHealth -= damage;
                hitDelay = Time.time + 0.5f;
                timeDelta = 0;
//                Debug.Log(StandardStuff.getName(transform) + " Already Dead = " + dead);
                if (!dead && currentHealth <= 0)
                {
                    Die(col.tag);
                }
                else
                {
                    audioSource.clip = HitSound;
                    audioSource.Play();
                    CryAudio?.Play();
                }
            }
        }
    }

    public virtual void Die(string colliderTag)
    {
        if (dieOnceAndForever) died.val = true;
        dead = true;
        Vector3 pos = transform.position;
        pos.y += DeathPrefabHeightOffset;
        if (DeathPrefab.activeSelf) Instantiate(DeathPrefab, pos, transform.rotation);
        else
        {
            DeathPrefab.transform.position = transform.position + new Vector3(0,DeathPrefabHeightOffset,0);
            DeathPrefab.SetActive(true);
        }
        ItemDropper.itemDropper.DropItem(dropPercent, enemyGroup, transform, colliderTag);
        Enemy[] childMobs = GetComponentsInChildren<Enemy>();
        foreach (Enemy e in childMobs) e.gameObject.SetActive(false);
        gameObject.SetActive(false);

        if (isRingLeader)
        {
            foreach (Enemy e in transform.parent.GetComponentsInChildren<Enemy>())
            {
                if (!e.dead)
                {
                    e.Die("NA");
                }
            }
        }
    }

    public virtual void OnTriggerEnter(Collider col)
    {
        HandleContact(col);
    }

    public virtual void OnTriggerStay(Collider col)
    {
        if (col.tag != "Untagged") HandleContact(col);
    }

    public float DamageDealt()
    {
        if(hitDelay > Time.time || stunned || ClockStun)
        {
            return 0f;
        }
        else
        {
            return damageDealt;
        }
    }

    public virtual void OnCollisionEnter(Collision col)
    {
        HandleContact(col.collider);
    }
}
