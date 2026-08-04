using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMDPresenterCam : MonoBehaviour {

    private GameObject firstPersonCam;
    private GameObject thirdPersonCam;
    private Camera[] camLocations;

    //Serialized just for debugging
    private GameObject playSpace;
    private GameObject head;

    private int selectedIndex = 0;
    private bool isActive;

	// Use this for initialization
	void Start () {
        camLocations = GetComponentsInChildren<Camera>();
        firstPersonCam = camLocations[0].gameObject;
        thirdPersonCam = camLocations[1].gameObject;
        firstPersonCam.SetActive(false);
        thirdPersonCam.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            firstPersonCam.SetActive(true);
            thirdPersonCam.SetActive(true);
            isActive = true;
            updateThirdPersonPosition();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            firstPersonCam.SetActive(false);
            thirdPersonCam.SetActive(false);
            isActive = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextCamera();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            previousCamera();
        }

        if (playSpace == null && isActive)
        {
           Player player = FindObjectOfType<Player>();
           if(player != null)
           {
                playSpace = player.gameObject;
           }
        }

        if (head == null && isActive)
        {
            Camera xrCamera = Camera.main;
            if (xrCamera != null) head = xrCamera.gameObject;
        }

        if(isActive && head != null && playSpace != null)
        {
            transform.position = playSpace.transform.position;
            transform.rotation = playSpace.transform.rotation;

            firstPersonCam.transform.position = head.transform.position;
            firstPersonCam.transform.rotation = head.transform.rotation;
        }
    }

    void nextCamera()
    {
        selectedIndex++;
        selectedIndex = selectedIndex >= camLocations.Length ? 0 : selectedIndex;
        updateThirdPersonPosition();
    }

    void previousCamera()
    {
        selectedIndex--;
        selectedIndex = selectedIndex < 0 ? camLocations.Length - 1  : selectedIndex;
        updateThirdPersonPosition();
    }

    void updateThirdPersonPosition()
    {
        GameObject camHolder = camLocations[selectedIndex].gameObject;
        thirdPersonCam.transform.position = camHolder.transform.position;
        thirdPersonCam.transform.rotation = camHolder.transform.rotation;
    }
}
