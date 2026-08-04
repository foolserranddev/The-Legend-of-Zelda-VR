using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour {
    public Sprite[] SmokeImages;
    public float timeDelay = 0.01f;

    private float timeDelta = 0;
    private float startTime;
    private int index = 0;

    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        index = Random.Range(0, 2); // Start random so they don't all look identical
    }

    // Update is called once per frame
    void Update()
    {
        timeDelta += Time.deltaTime;
        if (timeDelta >= timeDelay)
        {
            index = (index + 1) % 2;
            // Toggle between 0 and 1 for first half and 1 and 2 for second half.
            GetComponent<SpriteRenderer>().sprite = SmokeImages[index + ((Time.time - startTime) > 0.4 ? 1 : 0)];
            timeDelta = 0;
        }
        if (Time.time - startTime > 0.5)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
