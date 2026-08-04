using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriforceHolder : MonoBehaviour
{
    public static TriforceHolder triforceHolder;
    public ExplorationMap explorationMap;
    public GameObject[] Pieces;

    private void Awake()
    {
        if (triforceHolder == null)
        {
            triforceHolder = this;
        }
        else
        {
            triforceHolder.gameObject.SetActive(SceneManager.GetActiveScene().buildIndex <= 1);
            Destroy(gameObject);
        }
    }

	// Update is called once per frame
	public void UpdateTriforce ()
    {
		for (int i = 0; i < Pieces.Length; i++)
        {
            Pieces[i].SetActive(Player.player.hasTriforce[i].val);
        }
	}
}
