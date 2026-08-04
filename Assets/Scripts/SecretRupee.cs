using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRupee : MonoBehaviour {

    public TextMesh RupeeValue;
    public int numRupees = 30;
    public DestroyStuff destroyerOfThings;
    public bool bestowBombs = false;

    private float jiggleheight = 0.25f;
    private float timeToPeakHeight = 1;
    private float rotationPerSecond = 90f;
    private bool jiggleUp = true;
    private float jiggleMax, jiggleMin;
    private bool jiggle = true;
    private Vector3 RotateAround = new Vector3(0, 1, 0);

    void Start()
    {
        jiggleMax = transform.position.y + jiggleheight;
        jiggleMin = transform.position.y;

    }

    private void Update()
    {
        if (!jiggle) return;

        if (jiggleUp)
        {
            transform.position += new Vector3(0, 1 / timeToPeakHeight * jiggleheight * Time.deltaTime / 2, 0);
            if (transform.position.y >= jiggleMax)
            {
                jiggleUp = false;
            }
        }
        else
        {
            transform.position += new Vector3(0, -1 / timeToPeakHeight * jiggleheight * Time.deltaTime / 2, 0);
            if (transform.position.y <= jiggleMin)
            {
                jiggleUp = true;
            }
        }
        transform.Rotate(RotateAround, rotationPerSecond * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!(bestowBombs && Player.player.pd.NumRupees < Mathf.Abs(numRupees)) && numRupees != 0 && (col.tag == "Player" || col.tag == "Sword" || col.tag == "Shield"))
        {
            Player.player.AddRupees(numRupees);
            if (bestowBombs)
            {
                Player.player.IncreaseBombs();
                Player.player.pd.NumBombs += 4;
                StatusWindow.statusWindow.UpdateBombs();
            }
            numRupees = 0;
            RupeeValue.gameObject.SetActive(true);
            GetComponent<TextureSwapper>().stop = true;
            destroyerOfThings.GetReadyToDestroy();
            jiggle = false;
            transform.localEulerAngles = new Vector3(0,90,0);
                StatusWindow.statusWindow.Refresh();
        }
    }
}
