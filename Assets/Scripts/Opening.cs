using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening : MonoBehaviour
{
    public GameObject[] StuffToHide;
    public GameObject StoryBoard;
    public Light Sun;
    public float firstSunLerp;

    private float startSunLerp;
    private float startTime;
    private int state;
    private Vector3 storyBoardStartPosition;
    private Rigidbody storyBoardRigidBody;
    private float SunLerpTime;
    private AudioSource playerAudio;

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time;
        storyBoardStartPosition = StoryBoard.transform.position;
        storyBoardRigidBody = StoryBoard.GetComponent<Rigidbody>();
        startSunLerp = Sun.transform.localEulerAngles.x;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 newRotation;
        switch (state)
        {
        case 0:
            if (Time.time > startTime + 10)
            {
                state++;
            }
            break;
        case 1:
            SunLerpTime += Time.deltaTime;
            newRotation = Sun.transform.localEulerAngles;
            newRotation.x = Mathf.Lerp(startSunLerp, firstSunLerp, SunLerpTime);
            Sun.transform.localEulerAngles = newRotation;
            if (SunLerpTime >= 1)
            {
                state++;
                SunLerpTime = 0;
            }
            break;
        case 2:
            foreach (GameObject go in StuffToHide)
            {
                go.SetActive(false);
            }
            StoryBoard.SetActive(true);
            storyBoardRigidBody.velocity = new Vector3(0, 1.1f, 0);
            state++;
            break;
        case 3:
            if (playerAudio == null) playerAudio = Player.player.GetMusicAudioSource();
            if (!(playerAudio.isPlaying))
            {
                playerAudio.Play();
                foreach (GameObject go in StuffToHide)
                {
                    go.SetActive(true);
                }
                StoryBoard.SetActive(false);
                storyBoardRigidBody.velocity = Vector3.zero;
                StoryBoard.transform.position = storyBoardStartPosition;
                state++;
                startTime = Time.time;
            }
            break;
        case 4:
            SunLerpTime += Time.deltaTime;
            newRotation = Sun.transform.localEulerAngles;
            newRotation.x = Mathf.Lerp(firstSunLerp, startSunLerp, SunLerpTime);
            Sun.transform.localEulerAngles = newRotation;
            if (SunLerpTime >= 1)
            {
                state = 0;
                SunLerpTime = 0;
            }
            break;
        }
	}
}
