using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorReveal : MonoBehaviour
{
    public bool stayRevealed = true;
    public enum RevealType
    {
        RevealsWithFire,
        RevealsWithBomb,
        RevealsWhenTriggered
    }
    public RevealType RevealBy;
    public AudioClip RevealSound;
    public GameObject RevealObject;
    public SaveVar<bool> Revealed = new SaveVar<bool>(false);
    public Vector3 CenterOffset = Vector3.zero;

    private static string[] tags = { "Fire", "Bomb" };
    private AudioSource audioSource;
    private Vector3 myCenter;

    // Use this for initialization
    public void Start()
    {
        if (stayRevealed)
        {
            SaveData.saveData.data.registerBool(StandardStuff.getName(transform), Revealed);
            if (Revealed.val)
            {
                Reveal(true);
            }
            else
            {
                RevealObject.SetActive(false);
            }
        }
        audioSource = transform.parent.GetComponent<AudioSource>();
        Collider myCollider = GetComponent<Collider>();
        if (myCollider.GetType() == typeof(BoxCollider)) myCenter = transform.TransformPoint(((BoxCollider)myCollider).center);
        else if (myCollider.GetType() == typeof(SphereCollider)) myCenter = transform.TransformPoint(((SphereCollider)myCollider).center);
        else myCenter = transform.position;
        myCenter += CenterOffset;
    }


    void OnTriggerStay(Collider col)
    {
        if (RevealBy == RevealType.RevealsWhenTriggered) return;
        Debug.Log("Distance from myCenter " + myCenter + " at position " + col.transform.position + " = " + Vector3.Distance(col.transform.position, myCenter));
        if (col.tag == tags[(int)RevealBy])
        {
            if (Vector3.Distance(col.transform.position, myCenter) <= 2)
            {
                Reveal(false);
            }
        }
    }

    public void Reveal(bool quiet)
    {
        Revealed.val = true;
        if (RevealSound != null && !quiet)
        {
            audioSource.clip = RevealSound;
            audioSource.Play();
        }
        if (RevealObject != null) RevealObject.SetActive(true);
        if (RevealBy != RevealType.RevealsWhenTriggered) Destroy(gameObject);
    }

    public void Unreveal()
    {
        Revealed.val = false;
        RevealObject.SetActive(false);
    }

}
