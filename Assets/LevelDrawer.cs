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

        private TileBehavior[,] tiles;

        public static LevelDrawer Instance;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public TileBehavior GetTileBehavior(Point pos)
        {
            try
            {
                return this.tiles[pos.X, pos.Y];
            }
            catch
            {
                return null;
            }
        }

        public TileBehavior[] TilesAroundInRange(Point pos, int range)
        {
            var tiles = new List<TileBehavior>();

            System.Action<Point> tryAddTileBehavior = (p) =>
            {
                var tile = GetTileBehavior( p );
                if (tile != null)
                {
                    tiles.Add(tile);
                }
            };

            for (int i = -range; i <= range; i++)
            {
                int x = pos.X + i;
                int left = range - Mathf.Abs(i);

                if (left == 0)
                {
                    tryAddTileBehavior( new Point(x, pos.Y) );
                }
                else
                {
                    tryAddTileBehavior( new Point(x, pos.Y + left) );
                    tryAddTileBehavior( new Point(x, pos.Y - left) );
                }
            }

            return tiles.ToArray();
        }

        public void DrawDungeon(Tile[,] tiles)
        {
            AddTilesTo(tiles, this.Level);
        }

        void AddTilesTo(Tile[,] tiles, Transform parent)
        {
            this.tiles = new TileBehavior[tiles.GetLength(0), tiles.GetLength(1)];

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

                    var behavior = tileGo.GetComponent<TileBehavior>();
                    behavior.Tile = tile;
                    behavior.Pos = new Point(x, y);
                    this.tiles[x, y] = behavior;
                }
            }
        }
    }
}