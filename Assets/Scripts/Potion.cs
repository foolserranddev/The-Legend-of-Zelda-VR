using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : InteractableObject {

    public AudioClip fillSound;
    public AudioClip endFillSound;
    public GameObject Cork;
    public GameObject Bottle;
    public GameObject Liquid;
    public GameObject Letter;

    private bool poured = false;
    private Renderer liquidRenderer;
    private Sprite letterSprite;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        if (!Player.player.pd.hasLetter)
        {
            letterSprite = StatusWindowIcons[0];
            StatusWindowIcons[0] = null;
        }
    }

    private void ShowBottleOrLetter()
    {
        if (itemLevelIndex == 0)
        {
            Cork.SetActive(false);
            Bottle.SetActive(false);
            Liquid.SetActive(false);
            if (Player.player.pd.hasLetter) Letter.SetActive(true);
            if (!Player.player.pd.showedLetter) Letter.GetComponent<Collider>().enabled = true;
            else Letter.GetComponent<Collider>().enabled = false;
        }
        else
        {
            Cork.SetActive(true);
            Bottle.SetActive(true);
            Liquid.SetActive(true);
            Letter.SetActive(false);
            liquidRenderer.material = material[itemLevelIndex];
        }
    }

    public override void OnEnable()
    {
        if (liquidRenderer == null) liquidRenderer = Liquid.GetComponent<Renderer>();
        if (Player.player != null) ShowBottleOrLetter();
    }

    // Update is called once per frame
    void Update ()
    {
        if (StatusWindow.statusWindow == null || itemLevelIndex == 0) return;
        if (Player.player.pd.hasLetter && StatusWindowIcons[0] == null) StatusWindowIcons[0] = letterSprite;
        bool inPourPosition = itemLevelIndex > 0 && transform.up.y < 0 && transform.position.y > Camera.main.transform.position.y;
        if (!Player.player.Dead && !poured && !StatusWindow.statusWindow.isLifeFilling() && inPourPosition)
        {
            poured = true;
            StatusWindow.statusWindow.fillLife();
            UpdateLevel(itemLevelIndex-1);
            ShowBottleOrLetter();
        }
        poured = inPourPosition;
    }
}
