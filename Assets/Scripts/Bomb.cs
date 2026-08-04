using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : InteractableObject {

    public GameObject SmokeBomb;
    public float VelocityLow = 0.5f;
    public float VelocityHigh = 2;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public bool isEraseBomb = false;

    private AudioSource audioSource;
    private float startTime;
    private bool blowingUp = false;
    private bool blowUp = false;
    private float detonationTime = 1;
    private ParticleSystem particleSystem_;

    //private HandController hand;
    //private bool velocityAboveThreshold = false;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (particleSystem_ == null) particleSystem_ = GetComponent<ParticleSystem>();
        if (!isThrown()) particleSystem_.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (blowUp)
        {
            blowingUp = true;
            blowUp = false;
            audioSource.Play();
            startTime = Time.time;
        }
        else if (blowingUp)
        {
            float timeDiff = Time.time - startTime;
            if (timeDiff > detonationTime)
            {
                Detonate();
            }
        }
    }

    public void Detonate()
    {
        Instantiate(SmokeBomb, transform.position + new Vector3(0, 1f, 0), transform.rotation);
        if (isEraseBomb) mainHand.ChangeHandPosition(HandController.HandPositionsEnum.Open);
        Destroy(gameObject);
    }

    public bool isThrown()
    {
        return blowUp || blowingUp;
    }

    //private override void OnEnable()
    //{
    //    if (Player.player != null && Player.player.pd.NumBombs == 0)
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    private void ThrowBomb()
    {
        GameObject go = Instantiate(ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Bomb], transform);
        go.transform.SetParent(null);
        go.transform.localEulerAngles = new Vector3(-90, 0, 0);
        go.GetComponent<BoxCollider>().isTrigger = false;
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        Vector3 planarDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (planarDirection.sqrMagnitude < 0.0001f)
        {
            planarDirection = Vector3.ProjectOnPlane(Player.player.transform.forward, Vector3.up);
        }
        rb.velocity = planarDirection.normalized * 10;
        go.GetComponent<Bomb>().BlowUp();
    }

    public void BlowUp()
    {
        blowUp = true;
        if (particleSystem_ == null) particleSystem_ = GetComponent<ParticleSystem>();
        particleSystem_.Play();
    }

    public override void InitialSetup(HandController MainHand, HandController Offhand)
    {
        base.InitialSetup(MainHand, Offhand);
        GetComponent<BoxCollider>().isTrigger = true;
    }

    public override void performAction()
    {
        if (Player.player.pd.NumBombs > 0)
        {
            ThrowBomb();
            Player.player.pd.NumBombs -= 1;
            StatusWindow.statusWindow.UpdateBombs();
        }
        if (Player.player.pd.NumBombs == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (isEraseBomb)
        {
            if (col.tag == "Fire" && !blowingUp)
            {
                LightUp();
            }
        }
    }

    public void LightUp()
    {
        Debug.Log("LightUp Called");
        particleSystem_.Play();
        startTime = Time.time + 3;
        blowingUp = true;
    }

    public void SetAsEraseBomb()
    {
        isEraseBomb = true;
    }
}
