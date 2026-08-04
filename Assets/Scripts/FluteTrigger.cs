using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluteTrigger : MonoBehaviour
{
    protected string quadrant;
    protected bool activated;
    protected bool responding;

	// Use this for initialization
	public virtual void Start ()
    {
        quadrant = StandardStuff.getQuadrant(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual bool WillRespond()
    {
        //Debug.Log("Player Quadrant = " + Player.player.quadrant + "\nFlute Target Quadrant = " + quadrant);
        if (responding || Player.player.quadrant == quadrant && !activated)
        {
            responding = true;
            return true;
        }
        return false;
    }

    public virtual bool responseComplete()
    {
        return !responding;
    }
}
