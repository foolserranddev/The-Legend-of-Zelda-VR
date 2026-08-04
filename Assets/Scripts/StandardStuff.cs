using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StandardStuff : MonoBehaviour
{
    public static StandardStuff ss;
    public GameObject[] DungeonLocations;
    public bool isOpening = false;

    private bool initialized = false;
    private static string letters = "HGFEDCBAZZZZZ";

    // Use this for initialization
    void Awake ()
    {
        if (ss == null)
        {
            ss = this;
            DontDestroyOnLoad(ss);
        }
        else
        {
            ss.DungeonLocations = DungeonLocations;
            Destroy(gameObject);
        }
	}
	
    void Start ()
    {
        Debug.Log("Standard Stuff Starting Up");
        ObjectList.objectList.Initialize();
        SaveData.saveData.Initialize();
        Player.player.Initialize();
        StatusWindow.statusWindow.Initialize();
    }

    //void Update()
    //{
    //    if (!XRDevice.isPresent) Time.timeScale = 0;
    //    else Time.timeScale = 1;
    //}

    public static string getName(Transform t)
    {
        string name = t.name;
        while (t.parent != null)
        {
            name = t.parent.name + ">" + name;
            t = t.parent;
        }
        return name;
    }

    public static string getQuadrant(Vector3 position)
    {
        return "" + letters[(int)(position.z / 16)] + (16 - (int)(position.x / 16));
    }
}
