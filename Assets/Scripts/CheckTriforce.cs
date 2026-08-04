using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTriforce : MonoBehaviour
{

    public GameObject[] DestroyItems;
    public TrapDoor[] trapDoors;

    private SaveVar<bool> Opened;

    void Start()
    {
        SaveData.saveData.data.registerBool("OpenedForTriforce", Opened);
        if (Opened.val) DestroyItems[0].SetActive(false);
    }
    
    private void OpenTheDoors()
    {
        foreach (GameObject go in DestroyItems)
        {
            Destroy(go);
        }
        trapDoors[0].TriggerOpen();
        trapDoors[1].TriggerOpen();
        GetComponent<Collider>().enabled = false;
        GetComponent<AudioSource>().Play();
        Opened.val = true;
    }

    public void OnTriggerEnter(Collider col)
    {
        bool HasAllTriforce = true;
        foreach (SaveVar<bool> b in Player.player.hasTriforce)
        {
            if (b.val == false)
            {
                HasAllTriforce = false;
                break;
            }
        }
        if (HasAllTriforce)
        {
            OpenTheDoors();
        }
    }
}
