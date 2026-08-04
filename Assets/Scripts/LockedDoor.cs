using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public SaveVar<bool> DoorUnlocked = new SaveVar<bool>(false);
    public bool SecretD1Unlock = false;

    private Vector3 startPos;
    private AudioSource audioSource;
    private SaveVar<bool> secretUnlockOkay;
    private string playerFirstRoom;

    // Use this for initialization
    public void Start()
    {
        string AutoSaveString = StandardStuff.getName(transform) + transform.localEulerAngles[1];
        SaveData.saveData.data.registerBool(AutoSaveString, DoorUnlocked);
        if (DoorUnlocked.val) Destroy(gameObject);
        else
        {
            startPos = transform.position;
            audioSource = GetComponent<AudioSource>();
            if (SecretD1Unlock)
            {
                secretUnlockOkay = new SaveVar<bool>(true);
                SaveData.saveData.data.registerBool("SecretD1Unlock", secretUnlockOkay);
                playerFirstRoom = StandardStuff.getQuadrant(Player.player.transform.position);
            }
        }
    }

    void Update()
    {
        if (SecretD1Unlock)
        {
            if (secretUnlockOkay.val && StandardStuff.getQuadrant(Player.player.transform.position) != playerFirstRoom)
            {
                secretUnlockOkay.val = false;
            }
        }
        if (DoorUnlocked.val)
        {
            transform.position += new Vector3(0, 2, 0) * Time.deltaTime;
            if (Vector3.Distance(transform.position, startPos) > 3) Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" || col.tag == "Sword" || col.tag == "RightHand" || col.tag == "LeftHand")
        {
            if (!DoorUnlocked.val && Player.player.pd.NumKeys > 0 || ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.LionKey].val)
            {
                DoorUnlocked.val = true;
                Player.player.pd.NumKeys--;
                StatusWindow.statusWindow.UpdateKeys();
                audioSource.Play();
            }
        }
    }

    public void OnDisable()
    {
        if (SecretD1Unlock && !DoorUnlocked.val && secretUnlockOkay.val) DoorUnlocked.val = true;
    }
}
