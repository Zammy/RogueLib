using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDrawer : MonoBehaviour 
{
    public GameObject GroundPrefab;
    public GameObject WallPrefab;
    public GameObject StartPrefab;
    public GameObject EndPrefab;

    public float TileSize;

    public Transform Level;

    List<Transform> roomsParents = new List<Transform>();

    int counter = 0;

    public void DrawRoom(List<Tile> tiles)
    {
        GameObject roomParent = new GameObject();
        roomParent.transform.position = Vector3.zero;
        roomParent.name = "Room " + this.roomsParents.Count;

        AddTilesTo(tiles, roomParent.transform);

        this.roomsParents.Add(roomParent.transform);
    }

    public void DrawDungeon(List<Tile> tiles)
    {
        GameObject roomParent = new GameObject();
        roomParent.transform.position = Vector3.zero;
        roomParent.name = "Dungeon " + counter++;

        AddTilesTo(tiles, roomParent.transform);
    }

    public void ClearRooms()
    {
        foreach (var roomTrans in this.roomsParents)
        {
            roomTrans.SetParent(null);
            Destroy(roomTrans.gameObject);
        }

        this.roomsParents.Clear();
    }

    void AddTilesTo(List<Tile> tiles, Transform parent)
    {
        foreach (var tile in tiles)
        {
            Vector2 pos = new Vector2(TileSize * tile.Pos.X, TileSize * tile.Pos.Y);
            GameObject prefabToUse = null;
            switch (tile.Type)
            {
                case TileType.Ground:
                {
                    prefabToUse = this.GroundPrefab;
                    break;
                }
                case TileType.Wall:
                {
                    prefabToUse = this.WallPrefab;
                    break;
                }
                case TileType.Start:
                {
                    prefabToUse = this.StartPrefab;
                    break;
                }
                case TileType.End:
                {
                    prefabToUse = this.EndPrefab;
                    break;
                }
                default:
                    break;
            }
            var tileGo = (GameObject)Instantiate(prefabToUse, pos, Quaternion.identity);
            tileGo.transform.SetParent(parent);
        }
    }
}
