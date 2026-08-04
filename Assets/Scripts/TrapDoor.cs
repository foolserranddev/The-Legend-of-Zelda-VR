using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour {

    public bool OpenWithDeadMobs;
    public bool isTrap;
    private Transform Left;
    private Transform Right;
    public GameObject MonsterRoom;

    private AudioSource audioSource;
    private bool opening;
    private bool closing;
    private bool isOpen = false;
    private bool triggeredOpen = false;
    private Vector3 startPosLeft;
    private Vector3 startPosRight;
    private Enemy[] mobs;
    private int axisIdx;
    private bool negativeAxis;

	// Use this for initialization
	void Start ()
    {
        // Audio Source is in Child to center the audio on the actual door. This objects center is the center of the room.
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if (t.name.Equals("LeftDoor")) Left = t;
            else if (t.name.Equals("RightDoor")) Right = t;
            if (Right != null && Left != null) break;
        }
        audioSource = GetComponentInChildren<AudioSource>(); 
        startPosLeft = Left.position;
        startPosRight = Right.position;
        axisIdx = Left.up[0] != 0 ? 0 : Left.up[2] != 0 ? 2 : 1;
        negativeAxis = Left.up[axisIdx] < 0;
        if (OpenWithDeadMobs)
        {
            mobs = MonsterRoom.GetComponentsInChildren<Enemy>();
            for (int i = 0; i < mobs.Length; i++)
            {
                if (mobs[i].GetComponent<Bubble>() != null) mobs[i] = null;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Player.player?.isUnderground??true) return;
		if (opening)
        {
            Left.position += -Left.right * Time.deltaTime * 2;
            Right.position += Right.right * Time.deltaTime * 2;
            if (Vector3.Distance(Left.position, startPosLeft) > 1)
            {
                opening = false;
            }
        }
        else if (closing)
        {
            Left.position += Left.right * Time.deltaTime * 2;
            Right.position += -Right.right * Time.deltaTime * 2;
            if (Vector3.Distance(Left.position, startPosLeft) <= 0.1)
            {
                closing = false;
                Left.position = startPosLeft;
                Right.position = startPosRight;
            }
        }
        else if (isTrap || OpenWithDeadMobs)
        {
            bool inFrontOfDoor = IsPlayerInFrontOfDoor();
            bool MonstersPresent = OpenWithDeadMobs && AreThereMonstersAlive();

            // Door is closed, check if we should open
            if (!isOpen && OpenWithDeadMobs && !MonstersPresent && NoNewMonstersSpawned()) Open(true);
            else if (!isOpen && (isTrap && !inFrontOfDoor)) Open(false);
            //Door is open, check if we should close
            else if (isOpen && !triggeredOpen && (isTrap && inFrontOfDoor && (MonsterRoom == null || MonstersPresent)))
                Close(Mathf.Abs(Camera.main.transform.position[axisIdx == 0 ? 2 : 0] - Left.position[axisIdx == 0 ? 2 : 0]) < 8);
        }
    }

    private void OnDisable()
    {
        triggeredOpen = false;
    }

    private bool NoNewMonstersSpawned()
    {
        Enemy[] mobcheck = MonsterRoom.GetComponentsInChildren<Enemy>();
        foreach (Enemy e in mobcheck)
        {
            if (e.gameObject.activeSelf && e.tag != "Collectible" && e.GetComponent<Bubble>() == null) // Fairies are using the MovingShooter/Enemy script
            {
                return false;
            }
        }
        return true;
    }

    public void TriggerOpen()
    {
        triggeredOpen = true;
        Open(true);
    }

    public void TriggerClose()
    {
        triggeredOpen = false;
        Close(true);
    }

    private bool IsPlayerInFrontOfDoor()
    {
        bool greater = Camera.main.transform.position[axisIdx] > Left.position[axisIdx];
        return ((greater && !negativeAxis) || (!greater && negativeAxis));
    }

    private bool AreThereMonstersAlive()
    {
        if (MonsterRoom.activeSelf)
        {
            foreach (Enemy e in mobs)
            {
                if (e == null) continue;
                if (e.gameObject.activeSelf && e.transform.parent.gameObject.activeSelf)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Open(bool withSound)
    {
        if (!opening && withSound) audioSource.Play();
        opening = true;
        isOpen = true;
    }

    private void Close(bool withSound)
    {
        if (!closing && withSound) audioSource.Play();
        closing = true;
        isOpen = false;
    }
}
