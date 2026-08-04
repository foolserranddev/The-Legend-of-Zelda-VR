using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public string mapName;
    public Sprite mapSprite;
    public bool playerHasMap;
    public bool playerHasCompass;
    public float worldWidth = 255;
    public float worldHeight = 128;
    public float mapWidth = 1.92f;
    public float mapHeight = 0.96f;
    public GameObject Triforce;
    [HideInInspector] public Vector3 greenDotPosition;
    [HideInInspector] public Vector3 compassPosition = Vector3.zero;

    private SaveVar<bool> hasMap = new SaveVar<bool>(false);
    private SaveVar<bool> hasCompass = new SaveVar<bool>(false);

    // Use this for initialization
    void Start ()
    {
        hasMap.val = playerHasMap;
        hasCompass.val = playerHasCompass;
        SaveData.saveData.data.registerBool(mapSprite.name, hasMap);
        SaveData.saveData.data.registerBool(mapSprite.name + "_Compass", hasCompass);
        playerHasMap = hasMap.val;
        playerHasCompass = hasCompass.val;
        if (Triforce != null && Triforce != gameObject)
        {
            compassPosition.x = -(Triforce.transform.position.x / worldWidth) * mapWidth;
            compassPosition.y = -(Triforce.transform.position.z / worldHeight) * mapHeight;
            if (!Triforce.activeSelf) StatusWindow.statusWindow.RedDot.GetComponent<TextureSwapper>().stop = true;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Player.player == null) return;
        if (!Player.player.InDungeon || !Player.player.isUnderground)
        {
            greenDotPosition.x = -(Player.player.transform.position.x / worldWidth) * mapWidth;
            greenDotPosition.y = -(Player.player.transform.position.z / worldHeight) * mapHeight;
        }
    }

    public void obtainMap()
    {
        playerHasMap = true;
        hasMap.val = true;
    }

    public void obtainCompass()
    {
        playerHasCompass = true;
        hasCompass.val = true;
    }

}
