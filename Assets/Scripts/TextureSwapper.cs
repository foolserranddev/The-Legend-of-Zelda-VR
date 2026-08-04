using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSwapper : MonoBehaviour {

    public Material[] SwapImages;
    public Sprite[] SwapSprites;
    public float timeDelay;
    public bool stopWhenDone;
    public bool awaitTrigger = false;

    private float timeDelta = 0;
    private int index = 0;
    [HideInInspector]
    public bool stop = false;
    private Renderer r;
    private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
        sr = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (awaitTrigger) return;
        if (stop)
        {
            if (index != 0)
            {
                index = 0;
                if (sr == null) r.material = SwapImages[index];
                else sr.sprite = SwapSprites[index];
            }
            return;
        }
        timeDelta += Time.deltaTime;
        if (timeDelta >= timeDelay)
        {
            if (sr == null)
            {
                index = (index + 1) % SwapImages.Length;
                r.material = SwapImages[index];
            }
            else
            {
                index = (index + 1) % SwapSprites.Length;
                sr.sprite = SwapSprites[index];
            }
            timeDelta = 0;
        }
        if (stopWhenDone && index == SwapImages.Length - 1)
        {
            stop = true;
            awaitTrigger = true;
        }
    }

    public void Trigger()
    {
        index = 0;
        timeDelta = 0;
        awaitTrigger = false;
        stop = false;
        r.material = SwapImages[index];
    }
}
