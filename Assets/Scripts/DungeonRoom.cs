using System.Collections;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    Vector3 NorthDoor, EastDoor, SouthDoor, WestDoor;
    private const float DOOR_OFFSET_FROM_CENTER = 7.5f;
    private const float COLLISION_RADIUS = 0.2f;
    private const float DISTANCE_FROM_ROOM_TO_UPDATE = 11f;
    private float checkTime;
    private const float CHECK_RATE = 0.5f;
    private int x;
    private int z;
    private SaveVar<bool> EnteredRoom = new SaveVar<bool>(false);
    private GameObject[] lights;
    private bool lightsOn = true;
    private float lightingTime;
    private string quadrant;
	// Use this for initialization
	void Start ()
    {
        x = (int)(transform.position.x / 16);
        z = (int)(transform.position.z / 16);
        Light [] l = GetComponentsInChildren<Light>();
        lights = new GameObject[l.Length];
        for (int i = 0; i < l.Length; i++)
        {
            lights[i] = l[i].transform.parent.gameObject;
        }
        quadrant = StandardStuff.getQuadrant(transform.position);
        SaveData.saveData.data.registerBool(transform.parent.parent.parent.gameObject.name + transform.parent.gameObject.name + "Entered", EnteredRoom);
        if (EnteredRoom.val) ExplorationMap.explorationMap.ShowRoom(x, z);
        NorthDoor = transform.position + new Vector3(0, 1, -DOOR_OFFSET_FROM_CENTER);
        SouthDoor = transform.position + new Vector3(0, 1, DOOR_OFFSET_FROM_CENTER);
        WestDoor = transform.position + new Vector3(DOOR_OFFSET_FROM_CENTER, 1, 0);
        EastDoor = transform.position + new Vector3(-DOOR_OFFSET_FROM_CENTER, 1, 0);
        SetupDoors();
        if (transform.parent != null)
            DungeonRoomVisibilityManager.Register(transform.parent.gameObject, x, z, transform.position);
//        Debug.Log("Checking for doors at " + NorthDoor + " by " + transform.parent.gameObject.name);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Player.player.isUnderground) return;
        bool inRoom = Player.player?.quadrant.Equals(quadrant)??false;
        if (Time.time < lightingTime && inRoom)
        {
            foreach (GameObject l in lights)
            {
                l.SetActive(Random.Range(0, 2) == 1 ? true : false);
            }
        }
        else if (lightsOn && inRoom && lightingTime > 0.1)
        {
            foreach (GameObject l in lights)
            {
                l.SetActive(true);
                lightingTime = 0;
            }
        }
        else if (lightsOn && !inRoom) LightsOn(false);
        else if (!lightsOn && inRoom) LightsOn(true);
        if (Time.time > checkTime)
        {
            checkTime = Time.time + CHECK_RATE;
            if (!EnteredRoom.val)
            {
//                Debug.Log("[" + x + "," + z + "] != [" + Player.player.transform.position.x / 16 + "," + Player.player.transform.position.z / 16 + "]");
                if ((int)(Player.player.transform.position.x / 16) == x && (int)(Player.player.transform.position.z / 16) == z)
                {
                    EnteredRoom.val = true;
                    ExplorationMap.explorationMap.ShowRoom(x, z);
                }
            }
            if (Vector3.Distance(Player.player.transform.position, transform.position) < DISTANCE_FROM_ROOM_TO_UPDATE) SetupDoors();
        }
    }

    private void SetupDoors()
    {
        if (EnteredRoom.val)
        {
            ExplorationMap.explorationMap.ShowDoor(!IsBlocked(Physics.OverlapSphere(NorthDoor, COLLISION_RADIUS)), x, z, ExplorationMap.direction.North);
            ExplorationMap.explorationMap.ShowDoor(!IsBlocked(Physics.OverlapSphere(EastDoor, COLLISION_RADIUS)), x, z, ExplorationMap.direction.East);
            ExplorationMap.explorationMap.ShowDoor(!IsBlocked(Physics.OverlapSphere(SouthDoor, COLLISION_RADIUS)), x, z, ExplorationMap.direction.South);
            ExplorationMap.explorationMap.ShowDoor(!IsBlocked(Physics.OverlapSphere(WestDoor, COLLISION_RADIUS)), x, z, ExplorationMap.direction.West);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(WestDoor, COLLISION_RADIUS);
    //    Gizmos.DrawSphere(NorthDoor, COLLISION_RADIUS);
    //    Gizmos.DrawSphere(SouthDoor, COLLISION_RADIUS);
    //    Gizmos.DrawSphere(EastDoor, COLLISION_RADIUS);

    //}

    private void LightsOn(bool on)
    {
        foreach (GameObject l in lights)
        {
            l.SetActive(on);
            lightsOn = on;
            if (on) lightingTime = Time.time + 0.25f;
            else lightingTime = 0;
        }
    }

    private bool IsBlocked (Collider [] colliders)
    {
        bool blocked = false;
        foreach (Collider c in colliders)
        {
//            Debug.Log(c.transform.parent.parent.gameObject.name + ">" + c.transform.parent.gameObject.name + ">" + c.gameObject.name);
            if (c.transform.parent != null && (c.gameObject.name.Contains("Door") || c.transform.parent.gameObject.name.Contains("Door") || c.gameObject.name.Contains("Bombable")))
            {
                blocked = true;
                break;
            }
        }
        return blocked;
    }
}
