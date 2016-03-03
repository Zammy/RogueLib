using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RogueLib
{
    public class LevelDrawer : MonoBehaviour 
    {
        public GameObject GroundPrefab;
        public GameObject WallPrefab;
        public GameObject StartPrefab;
        public GameObject EndPrefab;

        public float TileSize;

        public Transform Level;

        private Tile[,] tiles;

        public void DrawDungeon(Tile[,] tiles)
        {
            this.tiles = tiles;

            AddTilesTo(tiles, this.Level);
        }

        void AddTilesTo(Tile[,] tiles, Transform parent)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Vector2 pos = new Vector2(TileSize * x, TileSize * y);
                    var tile = tiles[x, y];
                    if (tile == null)
                        continue;

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
    }
}