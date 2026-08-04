using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceOnOff : MonoBehaviour {

    private static readonly List<DistanceOnOff> instances = new List<DistanceOnOff>();
    private static readonly HashSet<Vector2Int> visibilityDisabledMobRooms = new HashSet<Vector2Int>();

    public bool isMobManagement = false;
    public int quadrantRadius = 2;
    private string playerQuadrant;
    private bool deadPlayer = false;

    private void OnEnable()
    {
        if (!instances.Contains(this)) instances.Add(this);
    }

    private void OnDisable()
    {
        instances.Remove(this);
    }

    public static void SetMobRoomVisible(int x, int z, bool visible)
    {
        Vector2Int room = new Vector2Int(x, z);
        if (visible) visibilityDisabledMobRooms.Remove(room);
        else visibilityDisabledMobRooms.Add(room);

        foreach (DistanceOnOff manager in instances)
        {
            if (manager == null || !manager.isMobManagement) continue;
            manager.SetMatchingCellVisible(x, z, visible);
        }
    }

    private void SetMatchingCellVisible(int x, int z, bool visible)
    {
        foreach (Transform row in transform)
        {
            foreach (Transform cell in row)
            {
                if ((int)(cell.position.x / 16) != x || (int)(cell.position.z / 16) != z) continue;
                if (!visible)
                {
                    cell.gameObject.SetActive(false);
                }
                else if (Player.player != null)
                {
                    int playerZ = (int)(Camera.main.transform.position.z / 16);
                    int playerX = (int)(Camera.main.transform.position.x / 16);
                    SetCellActive(cell, playerX, playerZ, false);
                }
            }
        }
    }
    
    public void Init(bool mobs, bool dungeon, int radius)
    {
        isMobManagement = mobs;
        quadrantRadius = radius;
    }
	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        bool ResetEverything = false;
        if (Player.player.Dead && !deadPlayer) deadPlayer = true;
        else if (!Player.player.Dead && deadPlayer)
        {
            deadPlayer = false;
            ResetEverything = true;
//            Debug.Log("Resetting Everything");
        }

        if (Player.player?.isUnderground?? true) return;
        if (ResetEverything || playerQuadrant != Player.player.quadrant)
        {
            playerQuadrant = Player.player.quadrant;
            int playerZ = (int)(Camera.main.transform.position.z / 16);
            int playerX = (int)(Camera.main.transform.position.x / 16);
            foreach (Transform row in transform)
            {
                foreach (Transform cell in row)
                {
                    Vector2Int cellRoom = new Vector2Int((int)(cell.position.x / 16), (int)(cell.position.z / 16));
                    if (isMobManagement && visibilityDisabledMobRooms.Contains(cellRoom))
                    {
                        cell.gameObject.SetActive(false);
                    }
                    else if (!ResetEverything && Mathf.Abs((int)(cell.position.z / 16) - playerZ) > quadrantRadius || Mathf.Abs((int)(cell.position.x / 16) - playerX) > quadrantRadius)
                    {
                        cell.gameObject.SetActive(false);
                    }
                    else
                    {
                        SetCellActive(cell, playerX, playerZ, ResetEverything);
                    }
                }
            }
        }
        ResetEverything = false;
    }

    private void SetCellActive(Transform cell, int playerX, int playerZ, bool resetEverything)
    {
        cell.gameObject.SetActive(true);
        if (!isMobManagement) return;

        Enemy[] enemies = cell.GetComponentsInChildren<Enemy>(true);
        bool foundActive = false;
        foreach (Enemy e in enemies)
        {
            if (e.tag == "Armos") e.gameObject.SetActive(true);
            else if (e.tag == "Zora")
            {
                int zDistance = Mathf.Abs((int)(cell.position.z / 16) - playerZ);
                int xDistance = Mathf.Abs((int)(cell.position.x / 16) - playerX);
                if (e.dead && zDistance + xDistance >= 2) e.gameObject.SetActive(true);
            }
            else if (e.gameObject.activeSelf)
            {
                foundActive = true;
            }
        }
        if (foundActive) return;

        bool cellFound = false;
        string cellname = StandardStuff.getQuadrant(cell.position);
        foreach (string pastRoom in Player.player.pastRooms)
            if (pastRoom.Equals(cellname)) cellFound = true;

        if (!cellFound || resetEverything)
        {
            foreach (Enemy enemy in enemies)
                if (enemy.GetComponent<Wallmaster>() == null) enemy.gameObject.SetActive(true);
        }
    }
}
