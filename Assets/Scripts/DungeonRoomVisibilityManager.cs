using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomVisibilityManager : MonoBehaviour
{
    private class RoomEntry
    {
        public GameObject roomRoot;
        public GameObject darkness;
        public int x;
        public int z;
        public bool sleeping;
    }

    private static DungeonRoomVisibilityManager instance;
    private readonly List<RoomEntry> rooms = new List<RoomEntry>();
    private float nextUpdate;

    public static void Register(GameObject roomRoot, int x, int z, Vector3 roomCenter)
    {
        if (instance == null)
        {
            GameObject managerObject = new GameObject("Dungeon Room Visibility Manager");
            instance = managerObject.AddComponent<DungeonRoomVisibilityManager>();
        }

        foreach (RoomEntry existing in instance.rooms)
            if (existing.roomRoot == roomRoot) return;

        GameObject darkness = GameObject.CreatePrimitive(PrimitiveType.Cube);
        darkness.name = "Distant Room Darkness";
        darkness.transform.SetParent(instance.transform, true);
        darkness.transform.position = roomCenter + Vector3.up * 3.75f;
        darkness.transform.localScale = new Vector3(15.8f, 8f, 15.8f);

        Collider darknessCollider = darkness.GetComponent<Collider>();
        if (darknessCollider != null) Destroy(darknessCollider);

        Renderer darknessRenderer = darkness.GetComponent<Renderer>();
        Shader unlit = Shader.Find("Unlit/Color");
        darknessRenderer.material = new Material(unlit != null ? unlit : Shader.Find("Standard"));
        darknessRenderer.material.color = Color.black;
        darknessRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        darknessRenderer.receiveShadows = false;
        darkness.SetActive(false);

        instance.rooms.Add(new RoomEntry
        {
            roomRoot = roomRoot,
            darkness = darkness,
            x = x,
            z = z
        });
    }

    private void Update()
    {
        if (Player.player == null || Time.time < nextUpdate) return;
        nextUpdate = Time.time + 0.25f;

        int playerX = Mathf.FloorToInt(Player.player.transform.position.x / 16f);
        int playerZ = Mathf.FloorToInt(Player.player.transform.position.z / 16f);

        foreach (RoomEntry room in rooms)
        {
            if (room.roomRoot == null || room.darkness == null) continue;
            bool shouldSleep = Mathf.Abs(playerX - room.x) + Mathf.Abs(playerZ - room.z) > 1;
            if (shouldSleep == room.sleeping) continue;

            room.sleeping = shouldSleep;
            if (shouldSleep)
            {
                // Stop mob animation/physics before removing the room beneath it.
                DistanceOnOff.SetMobRoomVisible(room.x, room.z, false);
                room.roomRoot.SetActive(false);
                room.darkness.SetActive(true);
            }
            else
            {
                // Restore the floor and room geometry before waking its mobs.
                room.roomRoot.SetActive(true);
                room.darkness.SetActive(false);
                DistanceOnOff.SetMobRoomVisible(room.x, room.z, true);
            }
        }
    }
}
