using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSword : InteractableObject
{
    public Mesh[] SwordMeshes;
    public int SaveIndex;
    public bool InStone;
    public GameObject Registration;

    public enum SwordEnum
    {
        WoodenSword,
        WhiteSword,
        MagicSword
    }

    private MeshFilter meshFilter;
    private Vector3 colliderSize;
    private BoxCollider boxCollider;
    private const float colliderHeightStep = 0.125f;
    private bool initialized = false;
    private bool latchedToHand;
    private HandController hand;
    private float handHeightOffset;
    private float startHeight;
    private float outHeight;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Collider myCollider;
    private bool AutoLitBomb = false;
    Rigidbody rb;


    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.localEulerAngles;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        meshFilter = GetComponent<MeshFilter>();
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null) colliderSize = boxCollider.size;
        if (boxCollider != null) boxCollider.size = colliderSize + new Vector3(0, colliderHeightStep * itemLevelIndex, 0);
        meshFilter.mesh = SwordMeshes[itemLevelIndex];
        initialized = true;
        startHeight = transform.position.y;
        outHeight = startHeight + 0.25f;
        if (Registration != null) Registration.SetActive(false);
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        transform.position = startPosition;
        transform.localEulerAngles = startRotation;
        myCollider.enabled = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        AutoLitBomb = false;
    }

    public void OnEnable()
    {
        AutoLitBomb = false;
    }

    public override void UpdateLevel(int level)
    {
        base.UpdateLevel(level);
        if (!initialized) return;
        meshFilter.mesh = SwordMeshes[level];
        boxCollider.size = colliderSize + new Vector3(0, colliderHeightStep * itemLevelIndex, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (latchedToHand)
        {
            Vector3 pos = transform.position;
            if (Vector3.Distance(hand.transform.position - new Vector3(0, handHeightOffset, 0), transform.position) > 0.1)
            {
                pos.y = startHeight;
                latchedToHand = false;
                hand = null;
            }
            else
            {
                pos.y = Mathf.Max(startHeight, Mathf.Min(outHeight + 0.1f, hand.transform.position.y - handHeightOffset));
            }
            if (pos.y >= outHeight)
            {
                //InitialSetup(hand, null);
                //hand.setMain(true);
                latchedToHand = false;
                //InStone = false;
                Registration.SetActive(true);
                Vector3 registrationPosition = Registration.transform.position;
                registrationPosition.y = Camera.main.transform.position.y;
                Registration.transform.position = registrationPosition;

                rb.velocity = new Vector3(0, 0.5f, 0);
                rb.angularVelocity = new Vector3(0, 0.5f, 0);
                myCollider.enabled = false;

            }
            else
            {
                transform.position = pos;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (latchedToHand) return;
        if (col.tag == "LeftHand" || col.tag == "RightHand")
        {
            if (InStone)
            {
                latchedToHand = true;
                hand = col.GetComponent<HandController>();
                handHeightOffset = hand.transform.position.y - transform.position.y;
            }
            else if (col.GetComponentInChildren<Bomb>() == null)
            {
                SaveData.saveData.SelectSavedata(SaveIndex);
                if (!SaveData.saveData.data.pd.handednessDetermined)
                {
                    SaveData.saveData.data.pd.handednessDetermined = true;
                    if (col.tag == "LeftHand")
                    {
                        SaveData.saveData.data.pd.isRightHanded = false;
                    }
                    else
                    {
                        SaveData.saveData.data.pd.isRightHanded = true;
                    }
                }
            }
            else if (!AutoLitBomb)
            {
                Bomb b = col.GetComponentInChildren<Bomb>();
                if (b.isThrown()) b.Detonate();
                else b.LightUp();
            }
        }
        else if (col.tag == "BombLive")
        {
            if (InStone)
            {
                Destroy(col.gameObject);
                col.GetComponentInParent<HandController>().ChangeHandPosition(HandController.HandPositionsEnum.Open);
            }
            else
            {
                Bomb b = col.GetComponent<Bomb>();
                if (!AutoLitBomb && b.isThrown())
                {
                    b.Detonate();
                    AutoLitBomb = false;
                }
                else if (!AutoLitBomb)
                {
                    b.LightUp();
                    AutoLitBomb = true;
                }
            }
        }
    }
}
