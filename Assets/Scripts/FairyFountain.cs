using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyFountain : MonoBehaviour {

    public GameObject HeartRing;
    public int rotateSpeed = 1;
    public AudioClip refill;
    public AudioClip end;

    private AudioSource audioSource;
    private bool activated = false;
    private Player p;
    private StatusWindow sw;

//    int numHearts;

	// Use this for initialization
	void Start ()
    {
        p = Player.player;
        audioSource = GetComponent<AudioSource>();
        sw = StatusWindow.statusWindow;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (activated)
        {
            HeartRing.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            p.LifeRemaining = Mathf.Min(p.LifeRemaining + 1f * Time.deltaTime, p.pd.NumHeartContainers);
            sw.UpdateLife();
            if (p.AtMaxHealth)
            {
                activated = false;
                HeartRing.SetActive(false);
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = end;
                audioSource.Play();
                p.Mobilize();
            }
        }
	}

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !activated && !p.AtMaxHealth)
        {
            HeartRing.SetActive(true);
            p.Immobilize();
            audioSource.clip = refill;
            audioSource.loop = true;
            audioSource.Play();
            activated = true;
//            numHearts = (int)(p.LifeRemaining * 2);
        }
    }
}
