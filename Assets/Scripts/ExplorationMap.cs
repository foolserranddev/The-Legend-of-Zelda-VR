using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationMap : MonoBehaviour
{
    public static ExplorationMap explorationMap;
    public TriforceHolder triforceHolder;
    public GameObject mapRoom;
    public GameObject arrow;
    public float startA = 0.9f;
    public float start8 = 0.5f;

    private float roomWidth;
    //private float doorWidth;

    private const float DOOR_RATIO_TO_ROOM_SPRITE = 2f/ 18f;
    private const float FULL_ROOM_RATIO_TO_DOOR_SPRITE = 1 + (DOOR_RATIO_TO_ROOM_SPRITE*2);
    private const int X_OFFSET_FROM_OVERWORLD = 5;
    private const int Z_OFFSET_FROM_OVERWORLD = 2;
    MapRoom[,] mapSquares = new MapRoom[16,8];

    public enum direction
    {
        North,
        East,
        South,
        West
    }

    private void Awake()
    {
        if (explorationMap == null)
        {
            explorationMap = this;
            explorationMap.gameObject.SetActive(SceneManager.GetActiveScene().buildIndex > 1);
        }
        else
        {
            explorationMap.ClearMap();
            explorationMap.gameObject.SetActive(SceneManager.GetActiveScene().buildIndex > 1);
            Destroy(gameObject);
        }

        roomWidth = mapRoom.GetComponent<SpriteRenderer>().bounds.size.x * FULL_ROOM_RATIO_TO_DOOR_SPRITE;
    }

   	// Update is called once per frame
	void Update ()
    {
        if (Player.player.isUnderground) return;
        Vector3 pos = arrow.transform.localPosition;
        pos.x = (Player.player.transform.position.x / 16f + X_OFFSET_FROM_OVERWORLD) * -roomWidth + roomWidth/2;
        pos.y = (Player.player.transform.position.z / 16f + Z_OFFSET_FROM_OVERWORLD) * -roomWidth + roomWidth/2;
        arrow.transform.localPosition = pos;
    }

    public void ClearMap()
    {
        foreach (MapRoom mr in mapSquares)
        {
            if (mr != null) mr.gameObject.SetActive(false);
        }
    }

    public void ShowRoom(int x, int y)
    {
        if (mapSquares[x, y] == null) mapSquares[x, y] = Instantiate(mapRoom, mapRoom.transform).GetComponent<MapRoom>();
        mapSquares[x, y].gameObject.SetActive(true);
        mapSquares[x, y].transform.parent = transform;
        Vector3 pos = Vector3.zero;
        pos.x = (x + X_OFFSET_FROM_OVERWORLD) * -roomWidth;
        pos.y = (y + Z_OFFSET_FROM_OVERWORLD) * -roomWidth;
        mapSquares[x, y].transform.localPosition = pos;
        mapSquares[x, y].transform.localEulerAngles = Vector3.zero;
        mapSquares[x, y].transform.localScale = Vector3.one;
    }

    public void ShowDoor(bool show, int x, int z, direction d)
    {
        if (mapSquares[x,z] == null) return;
        switch (d)
        {
            case direction.North:
                mapSquares[x, z].NorthDoor.SetActive(show);
                break;
            case direction.South:
                mapSquares[x, z].SouthDoor.SetActive(show);
                break;
            case direction.East:
                mapSquares[x, z].EastDoor.SetActive(show);
                break;
            case direction.West:
                mapSquares[x, z].WestDoor.SetActive(show);
                break;
        }
    }
}
