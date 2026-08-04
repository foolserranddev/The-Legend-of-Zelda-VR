using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkroom : MonoBehaviour
{
    public GameObject headBox;
    public GameObject floor;
    public GameObject room;

    [Header("GanonRoomSpecific")]
    public Ganon ganon;
    public GameObject Triforce;
    public GameObject ForcedEntrancePoint;
    public float TriforceTimeToCenter = 5;

    private string location;
    private bool onHead = false;
    private bool firedUp = false;

    private bool TriforceMoving;
    private float TriforceMaxScale;
    private float TriforceStartScale;
    private float TriforceScaleSpeed;
    private float TriforceSpeed;
    private Vector3 TriforceDestinationPosition;
    private Material darknessMat;
    private Color startColor;
    private Color invisible = new Color(0, 0, 0, 0);
    private float colorTransitionTime;
    private float timeForTransition = 2;

    // Use this for initialization
    void Start ()
    {
        location = StandardStuff.getQuadrant(transform.position);
        room.SetActive(true);
        floor.SetActive(true);
        darknessMat = headBox.GetComponent<Renderer>().sharedMaterial;
        startColor = darknessMat.color;
        if (Triforce != null)
        {
            Triforce.SetActive(false);
            TriforceDestinationPosition = Triforce.transform.localPosition;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!onHead && Player.player?.quadrant == location && !firedUp)
        {
            enableDarkness();
        }
        else if (onHead && firedUp && colorTransitionTime < timeForTransition)
        {
            colorTransitionTime += Time.deltaTime;
            Color lerpColor = Color.Lerp(startColor, invisible, colorTransitionTime / timeForTransition);
            darknessMat.color = lerpColor;
        }
        else if (onHead && (firedUp || Player.player?.quadrant != location))
        {
            removeDarkness();
        }
        else if (firedUp && Player.player?.quadrant != location)
        {
            firedUp = false;
        }
        if (TriforceMoving)
        {
            Vector3 direction = (TriforceDestinationPosition - Triforce.transform.localPosition).normalized;
            Triforce.transform.localPosition += direction * TriforceSpeed * Time.deltaTime;
            Triforce.transform.localScale = Vector3.one * Mathf.Min(TriforceMaxScale, Triforce.transform.localScale.x + (TriforceScaleSpeed * Time.deltaTime));
            if (Vector3.Distance(Triforce.transform.localPosition, TriforceDestinationPosition) < 0.1f)
            {
                TriforceMoving = false;
                removeDarkness();
                ganon.BeginFight();
                Triforce.SetActive(false);
                Player.player?.Mobilize();
                Player.player?.GetComponent<AudioSource>().Play();
            }
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (ganon == null)
        {
            if (col.tag == "Fire")
            {
                firedUp = true;
                colorTransitionTime = 0;
            }
        }
        else if (!TriforceMoving && !firedUp)
        {
            if (col.tag == "Player")
            {
                Debug.Log("Player Touched Ganon Room");
                Player.player.Immobilize();
                Player.player.GetComponent<AudioSource>().Stop();
                Vector3 movePos = ForcedEntrancePoint.transform.position - Camera.main.transform.localPosition;
                movePos.y = Player.player.transform.position.y;
                Player.player.transform.position = movePos;
                Vector3 startPosition = transform.InverseTransformPoint(Camera.main.transform.position);
                startPosition.y = TriforceDestinationPosition.y / 3;
                Debug.Log("Start Position of Triforce is " + startPosition);
                enableDarkness();
                TriforceMoving = true;
                firedUp = true;
                Triforce.SetActive(true);
                Triforce.transform.localPosition = startPosition;
                TriforceMaxScale = Triforce.transform.localScale.x;
                TriforceStartScale = TriforceMaxScale / 2;
                Triforce.transform.localScale /= 2;
                TriforceSpeed = Vector3.Distance(TriforceDestinationPosition, startPosition) / TriforceTimeToCenter;
                TriforceScaleSpeed = (TriforceMaxScale - TriforceStartScale) / TriforceTimeToCenter;
                GetComponent<AudioSource>().Play();
                colorTransitionTime = 0;
            }
        }
    }

    private void removeDarkness()
    {
        headBox.transform.SetParent(transform.parent);
        headBox.transform.position = transform.position;
        headBox.SetActive(false);
        floor.SetActive(false);
        room.SetActive(false);
        onHead = false;
        darknessMat.color = startColor;
    }

    private void enableDarkness()
    {
        headBox.transform.SetParent(Camera.main.transform);
        headBox.transform.position = Camera.main.transform.position;
        headBox.SetActive(true);
        floor.SetActive(true);
        room.SetActive(true);
        onHead = true;
        darknessMat.color = startColor;
    }
}
