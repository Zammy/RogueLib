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

    public void Draw(List<Tile> tiles)
    {
//        List<GameObject> oldTiles = new List<GameObject>();
//        foreach (Transform item in this.Level)
//        {
//            oldTiles.Add(item.gameObject);
//        }
//        foreach (var go in oldTiles)
//        {
//            go.transform.SetParent(null);
//            Destroy(go);
//        }

        foreach (var tile in tiles)
        {
            Vector2 pos = new Vector2( TileSize * tile.Pos.X, TileSize * tile.Pos.Y );
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

            var tileGo = (GameObject) Instantiate(prefabToUse, pos, Quaternion.identity);
            tileGo.transform.SetParent(this.Level);
        }
    }
}
