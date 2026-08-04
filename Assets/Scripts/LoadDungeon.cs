using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDungeon : MonoBehaviour {

    public int Dungeon;
    public Transform returnLocation;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            SceneManager.LoadScene(Dungeon+1);
            // If null, then we're already in a dungeon and exiting.
            if (returnLocation == null)
            {
                Player.player.returnFromDungeon = true;
                TriforceHolder.triforceHolder.gameObject.SetActive(true);
                ExplorationMap.explorationMap.gameObject.SetActive(false);
            }
            // Otherwise, we're entering a dungeon.
            else
            {
                Player.player.dungeonReturnLocation = returnLocation.position;
                TriforceHolder.triforceHolder.gameObject.SetActive(false);
                ExplorationMap.explorationMap.gameObject.SetActive(true);
            }
        }
    }
}
