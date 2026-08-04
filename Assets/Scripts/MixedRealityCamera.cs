using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedRealityCamera : MonoBehaviour {

    public bool TopLayerCamera;
    public GameObject marker;
    public Camera cam2;
    public GameObject GreenScreen;

    private Camera cam;
    // Use this for initialization
    void Start ()
    {
        cam = GetComponent<Camera>();
        cam2 = GetComponentInParent<Camera>();
        Display.displays[0].Activate();
        Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("TransparentFX")) & Camera.main.cullingMask;
        //Display.displays[1].Activate();
    }

    // Update is called once per frame
    void Update ()
    {

        if (TopLayerCamera)
        {
            float y = GreenScreen.transform.position.y;
            Vector3 pos = Camera.main.transform.position;
            pos.y = y;
            GreenScreen.transform.position = pos;
            cam.farClipPlane = Vector3.Distance(transform.position, Camera.main.transform.position);
        }
        //else
        //{
        //    cam.nearClipPlane = Vector3.Distance(transform.position, Camera.main.transform.position); ;
        //}
    }

    public void placeMarker(Transform pos)
    {
        GameObject go = Instantiate(marker, pos.position, new Quaternion());
        go.transform.SetParent(null);
    }

    public void AdjustCam(bool gripPressed, float triggerPercent, float padX, float padY)
    {
        if (triggerPercent < 0.1f) return;
        float angle = Mathf.Atan2(padY, padX);
        
        // RIGHT
        if (angle > -0.375 && angle <= 0.375)
        {
            //transform.position += transform.right * triggerPercent / 100;
        }
        // TOP RIGHT
        else if (angle > 0.375 && angle <= 1.125)
        {
            if (gripPressed)
            {
                transform.Rotate(transform.forward, triggerPercent / 100);
            }
            else
            {
                transform.Rotate(transform.up, triggerPercent / 100);
            }
        }
        // TOP
        else if (angle > 1.125 && angle <= 1.875)
        {
            if (gripPressed)
            {
                cam.fieldOfView += triggerPercent / 100;
                cam2.fieldOfView = cam.fieldOfView;
            }
            else
            {
                //transform.position += transform.forward * triggerPercent / 100;
            }
        }
        // TOP LEFT
        else if (angle > 1.875 && angle <= 2.625)
        {
            if (gripPressed)
            {
                transform.Rotate(-transform.forward, triggerPercent / 100);
            }
            else
            {
                transform.Rotate(-transform.up, triggerPercent / 100);
            }
        }
        // BOTTOM RIGHT
        else if (angle <= -0.375 && angle > -1.125)
        {
            if (gripPressed)
            {
                transform.Rotate(transform.right, triggerPercent / 100);
            }
            else
            {
                transform.position += transform.up * triggerPercent / 100;
            }
        }
        // BOTTOM
        else if (angle <= -1.125 && angle > -1.875)
        {
            if (gripPressed)
            {
                cam.fieldOfView -= triggerPercent / 100;
                cam2.fieldOfView = cam.fieldOfView;
            }
            else
            {
                //transform.position -= transform.forward * triggerPercent / 100;
            }
        }
        // BOTTOM LEFT
        else if (angle <= -1.875 && angle > -2.625)
        {
            if (gripPressed)
            {
                transform.Rotate(-transform.right, triggerPercent / 100);
            }
            else
            {
                //transform.position -= transform.up * triggerPercent / 100;
            }
        }
        // LEFT
        else
        {
            //transform.position -= transform.right * triggerPercent / 100;
        }
    }
}
