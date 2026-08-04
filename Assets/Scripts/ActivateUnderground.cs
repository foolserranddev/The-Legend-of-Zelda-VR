using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateUnderground : MonoBehaviour {

    public GameObject TheShop;
    public bool Activate = true;

    private void OnTriggerEnter(Collider other)
    {
        TheShop.SetActive(Activate);
    }
}
