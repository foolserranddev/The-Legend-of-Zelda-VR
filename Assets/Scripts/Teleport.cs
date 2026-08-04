using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Tooltip("No Longer Used. Keeping in case I need to revert due to oversight.")]
    public Teleport pairedTeleport; // No longer needed?
    public Transform portLocation;
    public GameObject Underground;
    public PushSecret ActivatePushSecret;
    public bool playStairs = true;
    public bool stopMusic = true;
    public bool startMusic = true;
    [Tooltip("No Longer Used. Keeping in case I need to revert due to oversight.")]
    public bool allowTransport = false; // No longer needed?
    public bool leadsUnderground = false;

    private bool musicAfterSound = false;
    private AudioSource audioSource;
    private AudioSource colAudioSource;
    private bool requiresActivation = false;

	// Use this for initialization
	void Start ()
    {
        audioSource = transform.parent.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (musicAfterSound && !audioSource.isPlaying)
        {
            Player.player.StartMusic();
            musicAfterSound = false;
        }
        if (Underground != null && Underground.activeSelf && !Player.player.isUnderground)
        {
            if (Underground != null) Underground.SetActive(false);
            if (requiresActivation) gameObject.SetActive(false);
        }
	}

    void OnDisable()
    {
        if (Underground != null && !Player.player.isUnderground) Underground.SetActive(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && Player.player.isMobile)// && allowTransport)
        {
            Vector3 fixHeight = pairedTeleport.portLocation.transform.position;
            pairedTeleport.portLocation.transform.position = new Vector3(fixHeight.x, col.transform.position.y, fixHeight.z);
            Vector3 Offset = (Camera.main.transform.position - col.transform.position);
            Offset.y = 0;
            col.transform.position = portLocation.position - Offset;

            if (stopMusic)
            {
                Player.player.StopMusic();
            }
            if (playStairs)
            {
                Player.player.StopMusic();
                audioSource.Play();
                musicAfterSound = startMusic;
            }
            else if (startMusic)
            {
                Player.player.StartMusic();
            }
            Player.player.isUnderground = leadsUnderground;
            if (Underground != null) Underground.SetActive(!Underground.activeSelf);
            if (ActivatePushSecret != null) ActivatePushSecret.Reveal();
            //pairedTeleport.allowTransport = true;
            //allowTransport = false;
        }
    }

    public void triggered()
    {
        requiresActivation = true;
    }
}
