using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectList : MonoBehaviour
{
    public static ObjectList objectList;
    public FluteTrigger[] FluteTargets;
    public GameObject Boomerang;
    public GameObject Bomb;
    public GameObject BowAndArrow;
    public GameObject Candle;
    public GameObject Flute;
    public GameObject Meat;
    public GameObject LetterOrPotion;
    public GameObject Wand;
    public GameObject Raft;
    public GameObject Book;
    public GameObject Ring;
    public GameObject Ladder;
    public GameObject LionKey;
    public GameObject Bracelet;
    public GameObject Sword;
    public GameObject Shield;
    public GameObject Key;
    public SaveVar<int>[] itemLevels = new SaveVar<int>[(int)prefabObjects.NumObjects];
    public SaveVar<bool>[] receivedObjects = new SaveVar<bool>[(int)prefabObjects.NumObjects];

    [HideInInspector]
    public GameObject[] prefabs;
    public enum prefabObjects
    {
        Boomerang,
        Bomb,
        BowAndArrow,
        Candle,
        Flute,
        Meat,
        LetterOrPotion,
        Wand,
        Raft,
        Book,
        Ring,
        Ladder,
        LionKey,
        Bracelet,
        Sword,
        Shield,
        Key,
        NumObjects
    }

    void Awake()
    {
        if (objectList == null) objectList = this;
        else
        {
            objectList.FluteTargets = FluteTargets;
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        Debug.Log("Objectlist Initializing");
        prefabs = new GameObject[] { Boomerang, Bomb, BowAndArrow, Candle, Flute, Meat, LetterOrPotion, Wand, Raft, Book, Ring, Ladder, LionKey, Bracelet, Sword, Shield, Key };
        for (int go = 0; go < prefabs.Length; go++)
        {
            if (prefabs[go] == null)
            {
                Debug.Log("Object " + go + " in ObjectList is null");
                continue;
            }
            prefabs[go] = Instantiate(prefabs[go], prefabs[go].transform.position, prefabs[go].transform.rotation);
            prefabs[go].SetActive(false);
            prefabs[go].transform.SetParent(transform);
        }
        UpdateFromSave();
    }

    public void UpdateFromSave()
    {
        Debug.Log("Updating Object List Data");
        for (int i = 0; i < itemLevels.Length; i++)
        {
            itemLevels[i] = new SaveVar<int>(0);
        }
        for (int i = 0; i < receivedObjects.Length; i++)
        {
            receivedObjects[i] = new SaveVar<bool>(false);
        }
        SaveData.saveData.data.registerIntArray("ItemLevels", itemLevels);
        SaveData.saveData.data.registerBoolArray("ReceivedObjects", receivedObjects);
        Debug.Log("Received Objects: " + receivedObjects);
        receivedObjects[(int)prefabObjects.Bomb].val = true; // Won't show without stock but need this to be true to show when stock is available.

        for (int go = 0; go < prefabs.Length; go++)
        {
            InteractableObject io = prefabs[go].GetComponent<InteractableObject>();
            if (io != null)
            {
                io.UpdateLevel(itemLevels[go].val);
            }
        }
    }
}

[System.Serializable]
public class GameItem
{
    public string name;
    public bool obtained;
    public int level;
    public GameObject prefab;

    public GameItem(string s)
    {
        name = s;
    }
}

