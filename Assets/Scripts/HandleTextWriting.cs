using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTextWriting : MonoBehaviour
{

    public TextTyper textTyper;
    public GameObject[] RevealExtraObjects;

    public enum TyperOption
    {
        StartText,
        StopText
    }

    public TyperOption textChoice = TyperOption.StartText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player" || textTyper == null) return;

        if (textChoice == TyperOption.StartText)
        {
            textTyper.InitiateTextTyping();
            foreach (GameObject go in RevealExtraObjects)
            {
                go.SetActive(true);
            }
        }
        else if (textChoice == TyperOption.StopText)
        {
            textTyper.StopText();
            foreach (GameObject go in RevealExtraObjects)
            {
                go.SetActive(false);
            }
        }
    }
}
