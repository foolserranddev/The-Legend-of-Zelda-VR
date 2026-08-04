using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawningManager : MonoBehaviour
{
    //private Vire[] vire;
    private Zol[] zol;
    private Vire[] vire;
    private string quad;
    private string playerQuad;

	// Use this for initialization
	void Start ()
    {
        zol = GetComponentsInChildren<Zol>();
        vire = GetComponentsInChildren<Vire>();
        quad = StandardStuff.getQuadrant(transform.position);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (playerQuad == quad)
        {
            if (Player.player.quadrant != playerQuad)
            {
                playerQuad = Player.player.quadrant;
                foreach (Zol z in zol)
                {
                    if (!z.stayDead)
                    {
                        z.gameObject.SetActive(true);
                    }
                }
                foreach (Vire v in vire)
                {
                    if (!v.stayDead)
                    {
                        v.gameObject.SetActive(true);
                    }
                }
            }
        }
        //playerQuad = Player.player.quadrant;
	}
}
