using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGame : MonoBehaviour
{

    public MoneyGame[] otherRupees;
    public float jiggleheight = 0.25f;
    public float timeToPeakHeight = 1;
    public float rotationPerSecond = 90f;
    public TextMesh textMesh;
    public bool Resetter = false;

    private bool jiggleUp = true;
    public bool Chosen = false;
    public bool stopMove = false;
    private bool showing = false;
    private float jiggleMax, jiggleMin;
    private int value;
    private int[] winOptions = { 20, 50 };
    private int[] loseOptions = { -10, -40 };
    // Use this for initialization
    void Start()
    {
        jiggleMax = transform.position.y + jiggleheight;
        jiggleMin = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Resetter && !stopMove)
        {
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
            transform.Rotate(Vector3.up, rotationPerSecond * Time.deltaTime);
        }
    }

    public void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" && Resetter)
        {
            otherRupees[0].Reset();
            otherRupees[1].Reset();
            otherRupees[2].Reset();
            return;
        }
        if (!Resetter && (col.tag == "Player" || col.tag == "Sword") && !Chosen && Player.player.pd.NumRupees >= 10)
        {
            Chose();
            otherRupees[0].Chose();
            otherRupees[1].Chose();
            int winningRupee = Random.Range(0, 3); // Choosing which rupee is a winner
            int winAmount = winOptions[Random.Range(0, 2)]; // Choosing How much the Winning Rupee is Worth
            int randLose = Random.Range(0, 2); // Choosing which of the two losing rupees loses 10
            int randLose2Amount = loseOptions[Random.Range(0, 2)]; // Choosing secondary lose amount
            
            if (winningRupee == 2) // If this rupee won (i.e. not a 0 or 1 index to the otherRupees array)
            {
                SetValue(winAmount);
                Player.player.AddRupees(winAmount); // Giving player winnings
                otherRupees[randLose].SetValue(loseOptions[0]); // Setting the -10 rupee
                otherRupees[(randLose + 1) % 2].SetValue(randLose2Amount); // Setting the second losing rupee's value
            }
            else
            {
                otherRupees[winningRupee].SetValue(winAmount); // Setting the winning rupee's value
                if (randLose == 1) // This rupee is considered index 1 for who lost the 10 rupees
                {
                    Player.player.AddRupees(loseOptions[0]);
                    SetValue(loseOptions[0]);
                    otherRupees[(winningRupee + 1) % 2].SetValue(randLose2Amount);
                }
                else
                {
                    Player.player.AddRupees(randLose2Amount);
                    SetValue(randLose2Amount);
                    otherRupees[(winningRupee + 1) % 2].SetValue(loseOptions[0]);
                }
            }
        }
    }

    public void Chose()
    {
        Chosen = true;
        stopMove = true;
        Vector3 v = transform.position;
        v.y = jiggleMin;
        transform.position = v;
        transform.localEulerAngles = new Vector3(0, 90, 0);
    }

    public void SetValue(int v)
    {
        textMesh.text = (v > 0 ? "+" : "") + v;
        textMesh.transform.localPosition += new Vector3(0, 1.44f, 0);
    }

    public void Reset()
    {
        if (Chosen)
        {
            textMesh.text = "";
            textMesh.transform.localPosition += new Vector3(0, -1.44f, 0);
            Chosen = false;
            showing = false;
        }
        else if (!showing)
        {
            textMesh.text = "-10";
            showing = true;
        }
        else
        {
            textMesh.text = "";
            showing = false;
        }
        stopMove = false;
    }
}