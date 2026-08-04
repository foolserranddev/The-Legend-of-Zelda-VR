using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeTrap : Enemy
{
    [Header("Blade Trap Specific")]

    public bool moveUp;
    public bool moveDown;
    public bool moveLeft;
    public bool moveRight;
    public float attackWidth = 0.75f;
    public float attackSpeed = 8;
    public float returnSpeed = 3;
    public float attackDistance = 7;

    private Vector3 startPos;
    private float minXrange;
    private float maxXrange;
    private float minZrange;
    private float maxZrange;
    private bool attacking;
    private bool returning;
    private Vector3 attackOffset;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        startPos = transform.position;
        minXrange = startPos.x - attackWidth;
        maxXrange = startPos.x + attackWidth;
        minZrange = startPos.z - attackWidth;
        maxZrange = startPos.z + attackWidth;
    }

    // Update is called once per frame
    public override void Update ()
    {
        if (Player.player.Dead) return;
        if (attacking) Attack();
        else if (returning) Retreat();
        else if (!Player.player?.isUnderground?? false) CheckAttack();
	}

    private void Attack()
    {
        Vector3 newPos = transform.position;
        transform.position += (attackOffset * Time.deltaTime * attackSpeed);
        if (Vector3.Distance(transform.position, startPos) > attackDistance)
        {
            returning = true;
            attacking = false;
            attackOffset *= -1;
        }
    }

    private void Retreat()
    {
        Vector3 newPos = transform.position;
        transform.position += (attackOffset * Time.deltaTime * returnSpeed);
        if (Vector3.Distance(transform.position, startPos) < 0.1f)
        {
            returning = false;
            transform.position = startPos;
        }
    }

    private void CheckAttack()
    {
        float x = Camera.main.transform.position.x;
        float z = Camera.main.transform.position.z;
        float xDiff = transform.position.x - x; // if positive, blade is left of player
        float zDiff = transform.position.z - z; // if positive, blade is below player
        if (moveUp && x > minXrange && x < maxXrange && zDiff < 15 && zDiff > 0)
        {
            attacking = true;
            attackOffset = new Vector3(0, 0, -1);
        }
        else if (moveDown && x > minXrange && x < maxXrange && zDiff > -15 && zDiff < 0)
        {
            attacking = true;
            attackOffset = new Vector3(0, 0, 1);
        }
        else if (moveLeft && z > minZrange && z < maxZrange && xDiff > -15 && xDiff < 0)
        {
            attacking = true;
            attackOffset = new Vector3(1, 0, 0);
        }
        else if (moveRight && z > minZrange && z < maxZrange && xDiff < 15 && xDiff > 0)
        {
            attacking = true;
            attackOffset = new Vector3(-1, 0, 0);
        }
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (!returning && col.tag == "Untagged" || col.tag == "Wall")
        {
            returning = true;
            attacking = false;
            attackOffset *= -1;
        }
    }
}
