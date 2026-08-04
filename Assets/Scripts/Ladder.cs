using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : InteractableObject {

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
    //public Collider WaterCollider;
    public bool LadderDown;
    public GameObject standPosition;
    public GameObject forwardStop;
    private LayerMask lMask;
    private float timeout;

    public void Awake()
    {
        lMask = LayerMask.GetMask(new string[] { "Ground" });
    }

    public override void Start()
    {
        base.Start();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Player")// && WaterCollider != null)
        {
            timeout = Time.time + 0.05f;
        }
    }

    void Update()
    {
        if (Time.time > timeout)
        {
            gameObject.SetActive(false);
            LadderDown = false;
        }
        Debug.DrawRay(transform.position, transform.right * 0.5f);

    }

    public void PlaceLadder(Collider col)
    {
        if (LadderDown) return;
        BoxCollider bcol = (BoxCollider)col;
        Vector3 colPos = bcol.transform.TransformPoint(bcol.center);
        Vector3 playerPos = Player.player.transform.position + Player.player.playerPlayspaceOffset;
        transform.position = playerPos;

        // If the max x is greater and the min x is lesser than my x, then it's running horizontally
        bool horizontal = bcol.bounds.max.x > playerPos.x && bcol.bounds.min.x < playerPos.x;
        // If the max z is greater and the min z is lesser than my z, then it's running vertically
        bool vertical = bcol.bounds.max.z > playerPos.z && bcol.bounds.min.z < playerPos.z;

        float offset = 0.75f;
        // if East 
        if (vertical && colPos.x < playerPos.x)
        {
            //Debug.Log("Placed East");
            transform.localEulerAngles = new Vector3(-90, 0, 180);
            transform.localPosition += new Vector3(-offset, 0f, 0f);
        }

        // if West
        else if (vertical && colPos.x > playerPos.x)
        {
            //Debug.Log("Placed West");
            transform.localEulerAngles = new Vector3(-90, 0, 0);
            transform.localPosition += new Vector3(offset, 0f, 0f);
        }

        // if North
        else if (horizontal && colPos.z < playerPos.z)
        {
            //Debug.Log("Placed North");
            transform.localEulerAngles = new Vector3(-90, 0, 90);
            transform.localPosition += new Vector3(0f, 0f, -offset);
        }

        // if South
        else if (horizontal && colPos.z > playerPos.z)
        {
            //Debug.Log("Placed South");
            transform.localEulerAngles = new Vector3(-90, 0, -90);
            transform.localPosition += new Vector3(0f, 0f, offset);
        }

        else
        {
            //Debug.Log("No clue where to place Ladder. Not placing.");
            return;
        }
        //Debug.Log("Forward = " + transform.forward);
        //Debug.Log("Right = " + transform.right);
        //Debug.Log("Up = " + transform.up);

        timeout = Time.time + 0.05f;

        LadderDown = true;
        //col.enabled = false;
        Player.player.transform.position = standPosition.transform.position - Player.player.playerPlayspaceOffset;
        gameObject.SetActive(true);
        RaycastHit hit;
        Debug.Log("Starting Ladder at " + transform.position);
        Debug.Log("Moving Spherecast in Direction " + transform.right);
        Debug.Log("Ending position is about " + (transform.position + (transform.right * 0.75f)));
        if (Physics.SphereCast(transform.position, 0.1f, transform.right, out hit, 0.75f, lMask))
        {
            Debug.Log("LADDER: Deactivating Forward Stop");
            forwardStop.SetActive(false);
        }
        else
        {
            Debug.Log("LADDER: Activating Forward Stop");
            forwardStop.SetActive(true);
        }
        //WaterCollider = col;
    }
}
