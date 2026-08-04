using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFrame : MonoBehaviour
{
    public static ItemFrame itemFrame;

    public GameObject[] ItemSlots;
    public GameObject Arrow;
    public Sprite[] ArrowSprites;
    public GameObject Bow;
    public Sprite[] BowSprites;
    public GameObject[] OtherItems;
    public GameObject Selector;

    private float timeout;
    private bool initialized;

    void Awake ()
    {
        if (itemFrame == null) itemFrame = this;
        //else if (SaveData.saveData.Reloading)
        //{
        //    Destroy(itemFrame.gameObject);
        //    itemFrame = this;
        //}
        else
        {
            Destroy(gameObject);
        }
	}
	
	//// Update is called once per frame
	void Update ()
    {
        if (!initialized && Player.player != null || Player.player.pd != null)
        {
            if (Player.player.pd.secondaryObjectIndex != -1) SelectItem(Player.player.pd.secondaryObjectIndex);
            initialized = true;
        }

        if (Time.time > timeout) gameObject.SetActive(false);	
	}

    public void OnEnable()
    {
        if (itemFrame != this) return;
        timeout = Time.time + 3;
    }

    public void UpdateImages()
    {
        // Determine Bomb Image
        int BombIdx = (int)ObjectList.prefabObjects.Bomb;
        ItemSlots[BombIdx].SetActive(Player.player.pd.NumBombs > 0);

        // Determine Bow and Arrow Image
        int BowIdx = (int)ObjectList.prefabObjects.BowAndArrow;
        int BowLevel = ObjectList.objectList.prefabs[BowIdx].GetComponent<InteractableObject>().itemLevelIndex;
        Arrow.GetComponent<SpriteRenderer>().sprite = ArrowSprites[BowLevel];
        Arrow.SetActive(Player.player.pd.hasArrows);
        Bow.GetComponent<SpriteRenderer>().sprite = BowSprites[BowLevel];
        Bow.SetActive(ObjectList.objectList.receivedObjects[BowIdx].val);

        // Determine Potion Image
        int PotionIdx = (int)ObjectList.prefabObjects.LetterOrPotion;
        int PotionLevel = (int)ObjectList.objectList.prefabs[PotionIdx].GetComponent<InteractableObject>().itemLevelIndex;
        ItemSlots[PotionIdx].GetComponent<SpriteRenderer>().sprite = ObjectList.objectList.prefabs[PotionIdx].GetComponent<InteractableObject>().GetSprite();
        ItemSlots[PotionIdx].SetActive(PotionLevel > 0 || Player.player.pd.hasLetter);
        

        for (int i = 0; i < 14; i++)
        {
            // These ones have unique requirements to show and update
            if (i == BombIdx || i == BowIdx || i == PotionIdx)
            {
                continue;
            }
            // The rest should all be able to pick their images normally
            else
            {
                if(ObjectList.objectList.prefabs[i] != null)
                {
                    if (i < 8) ItemSlots[i].GetComponent<SpriteRenderer>().sprite = ObjectList.objectList.prefabs[i].GetComponent<InteractableObject>().GetSprite();
                    // Only the ring will have a level so it's the only one needing to be an Interactable Object. The others would break here.
                    else if (i == (int)ObjectList.prefabObjects.Ring) OtherItems[i % 8].GetComponent<SpriteRenderer>().sprite = ObjectList.objectList.prefabs[i].GetComponent<InteractableObject>().GetSprite();
                    if (i < 8) ItemSlots[i].SetActive(ObjectList.objectList.receivedObjects[i].val);
                    else OtherItems[i % 8].SetActive(ObjectList.objectList.receivedObjects[i].val);
                }
                else
                {
                    // This means I haven't developed the item yet
                    if (i < 8) ItemSlots[i].SetActive(false);
                    else OtherItems[i % 8].SetActive(false);
                }
            }
        }
    }

    public void SelectItem(int i)
    {
        timeout = Time.time + 3;
        Selector.transform.position = ItemSlots[i < 0 ? 0 : i].transform.position;
    }
}
