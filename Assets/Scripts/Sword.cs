using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : InteractableObject
{
    public Mesh[] SwordMeshes;
    public GameObject BlinkySwordPrefab;
    public float InstantiateUpOffset = 0;
    public float InstantiateRightOffset = 0;
    public float InstantiateForwardOffset = 0;
    public AudioClip SwingSound;
    public AudioClip ShootSound;
    public float []damagePerHit = { 0.5f, 1.0f, 2.0f };
    public float VelocityLow = 1f;
    public float VelocityHigh = 2;
    public bool isDisplay = false;
    public int requiredHearts = 0;

    public enum SwordEnum
    {
        WoodenSword,
        WhiteSword,
        MagicSword
    }

    private AudioSource swingAudioSource;
    private AudioSource shootAudioSource;
    private HandController hand;
    private bool velocityAboveThreshold;
    private Projectile blinkySword;
    private bool isDisabled = false;
    private float disableTime;
    private Renderer swordRenderer;
    private MeshFilter meshFilter;
    private Vector3 colliderSize;
    private BoxCollider boxCollider;
    private CapsuleCollider capsuleCollider;
    private const float colliderHeightStep = 0.125f;
    private bool initialized = false;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        swingAudioSource = GetComponents<AudioSource>()[0];
        swingAudioSource.clip = SwingSound;
        shootAudioSource = GetComponents<AudioSource>()[1];
        shootAudioSource.clip = ShootSound;
        swordRenderer = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (boxCollider != null) colliderSize = boxCollider.size;
        if (boxCollider != null) boxCollider.size = colliderSize + new Vector3(0, colliderHeightStep * itemLevelIndex, 0);
        meshFilter.mesh = SwordMeshes[itemLevelIndex];
        initialized = true;
    }

    public override void UpdateLevel(int level)
    {
        base.UpdateLevel(level);
        if (!initialized) return;
        meshFilter.mesh = SwordMeshes[level];
        if (boxCollider != null) boxCollider.size = colliderSize + new Vector3(0, colliderHeightStep * itemLevelIndex, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.player == null) return;
        if (isDisplay)
        {
            if (boxCollider.enabled && Player.player.pd.NumHeartContainers < requiredHearts)
            {
                boxCollider.enabled = false;
                capsuleCollider.enabled = false;
            }
            else if (!boxCollider.enabled && Player.player.pd.NumHeartContainers >= requiredHearts)
            {
                boxCollider.enabled = true;
                capsuleCollider.enabled = true;
            }
            return;
        }
        if (Player.player.Dead)
        {
            boxCollider.enabled = false;
            isDisabled = true;
        }
        else
        {
            if (isDisabled)
            {
                if (Time.time > disableTime)
                {
                    isDisabled = false;
                    swordRenderer.enabled = true;
                    boxCollider.enabled = true;
                }
                else
                {
                    swordRenderer.enabled = !swordRenderer.enabled;
                }
                return;
            }
            float currSpeed = Vector3.Distance(hand.velocity, Vector3.zero);
            if (currSpeed > VelocityHigh)
            {
                if (!swingAudioSource.isPlaying)
                {
                    swingAudioSource.clip = SwingSound;
                    swingAudioSource.Play();
                }
                velocityAboveThreshold = true;
            }
            else if (velocityAboveThreshold == true && currSpeed < VelocityLow)
            {
                ShootSword();
            }
        }
    }

    public void ShootSword()
    {
        velocityAboveThreshold = false;
        // Only shoot one at a time. Wait for other one to be destroyed to create a new one.
        // Also require max health for the sword. BoxCollider is empty for the Wand and that
        // can always be shot.
        if (blinkySword == null && !(boxCollider != null && !Player.player.AtMaxHealth)) 
        {
            shootAudioSource.Play();
            Vector3 startPos = transform.position 
                + transform.up * InstantiateUpOffset 
                + transform.right * InstantiateRightOffset 
                + transform.forward * InstantiateForwardOffset;
            blinkySword = Instantiate(BlinkySwordPrefab, startPos, transform.rotation).GetComponent<Projectile>();
            if (boxCollider != null)
            {
                blinkySword.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
                blinkySword.GetComponent<Projectile>().damagePerHit = damageDealt();
                BoxCollider bc = blinkySword.GetComponent<BoxCollider>();
                bc.center = boxCollider.center;
            }
            blinkySword.Shoot(transform.up, gameObject);
        }
    }

    public override void InitialSetup(HandController MainHand, HandController Offhand)
    {
        base.InitialSetup(MainHand, Offhand);
        if (MainHand != null) hand = MainHand;
    }

    public void disableSword()
    {
        isDisabled = true;
        disableTime = Time.time + 4;
        boxCollider.enabled = false;
    }

    //public override ObjectList.prefabObjects PerformGrabResponse(HandController h)
    //{
    //    hand = h;
    //    hand.setMain();
    //    jiggle = false;
    //    foreach (GameObject obj in objectsToDestroy)
    //    {
    //        Destroy(obj);
    //    }
    //    return ObjectList.prefabObjects.Sword;
    //}

    public bool IsSwinging()
    {
        return velocityAboveThreshold || Vector3.Distance(hand.velocity, Vector3.zero) > 2 || swingAudioSource.isPlaying;
    }

    public override float damageDealt()
    {
        return damagePerHit[(int)itemLevelIndex];
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Collectible" || col.tag == "Heart Container")
        {
            Player.player.HandleCollisions(col);
        }
        else if (col.tag == "Item")
        {
            Player.player.rightHand.OnTriggerEnter(col);
        }
    }
}
