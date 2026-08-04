using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public GameObject[] ObjectsToReenable;

    public void OnEnable()
    {
        foreach (GameObject go in ObjectsToReenable)
        {
            go.SetActive(true);
        }
    }

}
