using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purchaseable : MonoBehaviour {

    public ObjectList.prefabObjects obj;
    public int itemLevel = 0;
    public DestroyStuff Destroyer;
    public int cost;
    public TextTyper TextToChange;
    public string PurchasedMessage;

    private int purchaseAmount = 0;

    private void Awake()
    {
        if (TextToChange != null) PurchasedMessage = PurchasedMessage.Replace("§", System.Environment.NewLine);
        if (tag == "Collectible" || tag == "Heart Container") purchaseAmount = GetComponent<Collectible>().getAmount();
    }

    private void OnTriggerEnter(Collider col)
    {
        if ((col.tag == "Player" || col.tag == "Sword" || col.tag == "LeftHand" || col.tag == "RightHand") && Player.player.pd.NumRupees >= cost)
        {
            Player.player.AddRupees(cost * -1);
            if (TextToChange != null)
            {
                TextToChange.InitiateTextTyping(PurchasedMessage);
            }
            else if (tag == "Collectible" || tag == "Heart Container")
            {
                GetComponent<Collectible>().SetAmount(purchaseAmount);
                Player.player.HandleCollisions(GetComponent<Collider>());
            }
            else if(obj == ObjectList.prefabObjects.BowAndArrow)
            {
                Player.player.pd.hasArrows = true;
                ItemFrame.itemFrame.UpdateImages();
            }
            else
            {
                Player.player.ObtainPrefab(obj, itemLevel);
            }
            Destroyer.GetReadyToDestroy();
        }
    }
}
