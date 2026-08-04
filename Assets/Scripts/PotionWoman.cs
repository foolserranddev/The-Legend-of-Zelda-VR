using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionWoman : MonoBehaviour
{

    public GameObject[] thingsToHide;
    public TextTyper textTyper;
    public DestroyStuff Destroyer;

	// Use this for initialization
	void OnEnable ()
    {
        if (Destroyer.Destroying()) return;
        foreach(GameObject o in thingsToHide)
        {
            o.SetActive(Player.player.pd.showedLetter);
        }
        GetComponent<Collider>().enabled = !Player.player.pd.showedLetter;
        if (Player.player.pd.showedLetter)
        {
            textTyper.InitiateTextTyping();
        }
    }
	
    private void OnTriggerEnter(Collider col)
    {
        Potion p = col.GetComponentInParent<Potion>();
        if (p != null && col.GetComponentInParent<InteractableObject>().itemLevelIndex == 0)
        {
            Player.player.pd.showedLetter = true;
            col.enabled = false;
            GetComponent<Collider>().enabled = false;
            foreach (GameObject o in thingsToHide)
            {
                o.SetActive(true);
            }
            textTyper.InitiateTextTyping();
        }
    }
}
