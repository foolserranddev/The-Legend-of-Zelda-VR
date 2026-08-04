using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostWoodsHills : MonoBehaviour
{
    public bool isHills;
    public GameObject North;
    public GameObject East;
    public GameObject South;
    public GameObject West;
    public GameObject Floor;
    public GameObject WhiteWalls;

    private AudioSource audioSource;
    private GameObject[] DirObjects;
    private dir[] dirHistory = new dir[] { dir.None, dir.None, dir.None, dir.None };

    private enum dir
    {
        North,
        South,
        East,
        West,
        None,
    }

    void Awake()
    {
        audioSource = transform.parent.GetComponent<AudioSource>();
        DirObjects = new GameObject[] { North, South, East, West };
    }

    public void OnDisable()
    {
        dirHistory[3] = dir.None;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {

            AppendDirection();

            if (isHills)
            {
                // If East, let him go
                if (dirHistory[3] == dir.East)
                {
                    return;
                }
                // If Secret path, let through with music
                else if (dirHistory[0] == dir.North &&
                        dirHistory[1] == dir.West &&
                        dirHistory[2] == dir.South &&
                        dirHistory[3] == dir.West)
                {
                    dirHistory[3] = dir.None;
                    audioSource.Play();
                    return;
                }
            }
            else
            {
                if (dirHistory[3] == dir.West)
                {
                    return;
                }
                // If Secret path, let through with music
                else if (dirHistory[0] == dir.North &&
                        dirHistory[1] == dir.North &&
                        dirHistory[2] == dir.North &&
                        dirHistory[3] == dir.North)
                {
                    audioSource.Play();
                    return;
                }
            }
            //
            // If we didn't get through with the above conditions, then it's time to swap locations.
            //
            Vector3 playerOffset = Player.player.playerPlayspaceOffset;
            // If North, port to South
            if (dirHistory[3] == dir.North) Player.player.transform.position = DirObjects[(int)dir.South].transform.position - playerOffset;
            // If West, port to East
            else if (dirHistory[3] == dir.West) Player.player.transform.position = DirObjects[(int)dir.East].transform.position - playerOffset;
            // If South, port to North
            else if (dirHistory[3] == dir.South) Player.player.transform.position = DirObjects[(int)dir.North].transform.position - playerOffset;
            // If East, port to West
            else if (dirHistory[3] == dir.East) Player.player.transform.position = DirObjects[(int)dir.West].transform.position - playerOffset;
        }
    }

    private void AppendDirection()
    {
        dir direction = dir.North;
        float distance = Vector3.Distance(Camera.main.transform.position, North.transform.position);
        float newDistance = Vector3.Distance(Camera.main.transform.position, East.transform.position);
        if (newDistance < distance)
        {
            distance = newDistance;
            direction = dir.East;
        }
        newDistance = Vector3.Distance(Camera.main.transform.position, South.transform.position);
        if (newDistance < distance)
        {
            distance = newDistance;
            direction = dir.South;
        }
        newDistance = Vector3.Distance(Camera.main.transform.position, West.transform.position);
        if (newDistance < distance)
        {
            distance = newDistance;
            direction = dir.West;
        }

        dirHistory[0] = dirHistory[1];
        dirHistory[1] = dirHistory[2];
        dirHistory[2] = dirHistory[3];
        dirHistory[3] = direction;
    }

}
