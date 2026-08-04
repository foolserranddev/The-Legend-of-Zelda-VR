using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flute : InteractableObject
{
    public GameObject WhirlwindPrefab;

    private AudioSource audioSource;
    private bool activated = false;
    private bool flutePlaying = false;
    private bool fluteTargetResponding = false;
    private int fluteTargetIndex;
    private int dungeonIndex = 0;

    private bool initialized = false;
    private Whirlwind whirlwind;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        initialized = true;
    }
    // Update is called once per frame
    public void Update ()
    {
        bool fluteToFace = Vector3.Distance(Camera.main.transform.position, transform.position) < 0.27f;
        // If in the middle of a target action or haven't removed from face since, then kick out now.
        if (fluteTargetResponding && (!ObjectList.objectList.FluteTargets[fluteTargetIndex].responseComplete() || fluteToFace)
            || (whirlwind != null && whirlwind.gameObject.activeSelf)) return;
        else fluteTargetResponding = false;

        // Deactivate once removing flute from face
        if (activated && !fluteToFace) activated = false;

        // If Flute is to Face and we haven't started playing yet, then start playing
        if (!activated && fluteToFace && !flutePlaying && Player.player.isMobile)
        {
            flutePlaying = true;
            audioSource.Play();
            return;
        }
        // If We're still playing, then make sure flute is still to face or else stop playing
        else if (audioSource.isPlaying)
        {
            if (!fluteToFace)
            {
                audioSource.Stop();
                flutePlaying = false;
            }
            return;
        }
        // If we're done playing the song successfully, figure out what to do
        else if (flutePlaying)
        {
            flutePlaying = false;
            for (int i = 0; i < ObjectList.objectList.FluteTargets.Length; i++)
            {
                if (ObjectList.objectList.FluteTargets[i].WillRespond())
                { 
                    fluteTargetResponding = true;
                    fluteTargetIndex = i;
                }
            }
            if (!fluteTargetResponding)
            {
                activated = true;
                if (!Player.player.InDungeon && !Player.player.isUnderground)
                {
                    // Summon Whirlwind
                    bool east = Camera.main.transform.forward.x < 0;
                    GameObject destination = null;
                    for (int i = 0; i < 8; i++)
                    {
                        dungeonIndex += east ? 1 : -1;
                        if (dungeonIndex < 0) dungeonIndex = 7;
                        if (dungeonIndex > 7) dungeonIndex = 0;
                        if (Player.player.hasTriforce[dungeonIndex].val)
                        {
                            destination = StandardStuff.ss.DungeonLocations[dungeonIndex];
                            break;
                        }
                    }
                    if (destination != null)
                    {
                        whirlwind = Instantiate(WhirlwindPrefab, Player.player.transform.position + Player.player.playerPlayspaceOffset + new Vector3(16, 0, 0), WhirlwindPrefab.transform.rotation).GetComponent<Whirlwind>();
                        whirlwind.destination = destination;
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        if (!initialized) return;
        flutePlaying = false;
    }

}
