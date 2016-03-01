using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BSP
{
   

    //binary space partitioning
    public class BSPGenerator : MonoBehaviour 
    {
        //params
        public int Seed = 1;

        public int DUN_WIDTH = 100;
        public int DUN_HEIGHT= 100;

        public int MAX_LEAF_AREA = 1000;
        public int MIN_LEAF_SIDE = 10;

        public int MIN_ROOM_SIDE = 4;
        //

        public BSPDrawer Drawer;

        public LevelDrawer LevelDrawer;

        BinaryNode root;


        List<Corridor> corridors = new List<Corridor>();
        List<Room> rooms = new List<Room>();

        void Start()
        {
            Random.seed = Seed;
        }

        public void GenAndDrawRooms()
        {
            this.rooms.Clear();
            this.corridors.Clear();

            root = new BinaryNode(Point.Zero, Point.Zero, DUN_WIDTH, DUN_HEIGHT);
           
            Divide(root);

            GenRooms(root);

            GenCorridors(root);

            this.DrawWithTiles();

//            this.Drawer.DrawRoomsAndCorridors(rooms, corridors);

        }

        void Divide(BinaryNode node)
        {
            if (node.Area() < MAX_LEAF_AREA)
            {
                return;
            }

            int direction = 0;
            if (node.Width < node.Height)
            {
                direction = 1;
            }

            if (direction == 0) // split vertically
            {
                int x = Random.Range(MIN_LEAF_SIDE, node.Width-MIN_LEAF_SIDE);
                node.Children[0] = new BinaryNode(Point.Zero, new Point(node.GlobalPos), x, node.Height);
                node.Children[1] = new BinaryNode(new Point(x, 0), new Point(node.GlobalPos.X + x, node.GlobalPos.Y), node.Width - x, node.Height);
            }
            else // split horizontally
            {
                int y = Random.Range(MIN_LEAF_SIDE, node.Height-MIN_LEAF_SIDE);
                node.Children[0] = new BinaryNode(Point.Zero, new Point(node.GlobalPos), node.Width, y);
                node.Children[1] = new BinaryNode(new Point(0, y), new Point(node.GlobalPos.X, node.GlobalPos.Y + y), node.Width, node.Height - y);
            }

            Divide(node.Children[0]);
            Divide(node.Children[1]);
        }

        void GenRooms(BinaryNode node)
        {
            if (node.IsLeaf())
            {
                this.rooms.Add( node.GenRoom(MIN_ROOM_SIDE) );
            }
            else
            {
                GenRooms(node.Children[0]);
                GenRooms(node.Children[1]);
            }
        }

        void GenCorridors(BinaryNode node)
        {
            if (node.IsLeaf())
                return;

            if (!node.Children[0].IsLeaf())
            {
                GenCorridors(node.Children[0]);
            }
            if (!node.Children[1].IsLeaf())
            {
                GenCorridors(node.Children[1]);
            }

            BinaryNode leaf1 = node.Children[0];
            BinaryNode leaf2 = node.Children[1];

            var rooms1 = new List<Room>();
            var rooms2 = new List<Room>();

            leaf1.FillRoomsRecursively(rooms1);
            leaf2.FillRoomsRecursively(rooms2);

            Room[] roomsToConnect = FindRoomPairToConnect(rooms1, rooms2);
            Room room1 = roomsToConnect[0];
            Room room2 = roomsToConnect[1];

            //make a corridor
            if (leaf1.GlobalPos.Y == leaf2.GlobalPos.Y)
            {
                this.corridors.Add( DigCorridorRightTo(room1, room2) );
            }
            else
            {
                this.corridors.Add( DigCorridorUpTo(room1, room2) );
            }
        }

        Room[] FindRoomPairToConnect(List<Room> rooms1, List<Room> rooms2)
        {
            int smallestDist = int.MaxValue;
            Room[] closestsRooms = new Room[2];
            foreach (var r1 in rooms1)
            {
                foreach (var r2 in rooms2) 
                {
                    var diff = r1.GetCenter() - r2.GetCenter();
                    int dist = diff.Length;
                    if (dist < smallestDist)
                    {
                        smallestDist = dist;
                        closestsRooms[0] = r1;
                        closestsRooms[1] = r2;
                    }
                }   
            }
            return closestsRooms;
        }

        Corridor DigCorridorRightTo(Room fromRoom, Room goalRoom)
        {
            var startPoint = new Point( fromRoom.GlobalPos.X + fromRoom.Width-1,
                Random.Range(fromRoom.GlobalPos.Y+1, fromRoom.GlobalPos.Y + fromRoom.Height -1) );
            var corridor = new Corridor(startPoint);
            Point goal = goalRoom.GetRandomPointInsideRoom(2);
            corridor.Points.Add( new Point( goal.X, startPoint.Y) );
            corridor.Points.Add( new Point( goal.X, goal.Y) );
            return corridor;
        }

        Corridor DigCorridorUpTo(Room fromRoom, Room goalRoom)
        {
            var startPoint = new Point( Random.Range( fromRoom.GlobalPos.X+1, fromRoom.GlobalPos.X + fromRoom.Width -1),
                 fromRoom.GlobalPos.Y + fromRoom.Height-1);
            var corridor = new Corridor(startPoint);
            Point goal = goalRoom.GetRandomPointInsideRoom(2);
            corridor.Points.Add( new Point( startPoint.X, goal.Y) );
            corridor.Points.Add( new Point( goal.X, goal.Y) );
            return corridor;
        }

        void DrawWithTiles()
        {
            Tile[,] grid = new Tile[DUN_WIDTH, DUN_HEIGHT];
            System.Action<Tile> addTile = (Tile tile) => 
            {
                grid[tile.Pos.X, tile.Pos.Y] = tile;
            };
            foreach (var room in this.rooms)
            {
                int bottomY = room.GlobalPos.Y;
                int topY = room.GlobalPos.Y + room.Height -1;
                for (int x = room.GlobalPos.X; x < room.GlobalPos.X + room.Width; x++)
                {
                    var tile = new Tile();
                    tile.Pos = new Point(x, bottomY);
                    tile.Type = TileType.Wall;
                    addTile(tile);
                    tile = new Tile();
                    tile.Pos = new Point(x, topY);
                    tile.Type = TileType.Wall;
                    addTile(tile);
                }
                int leftX = room.GlobalPos.X ;
                int rightX = room.GlobalPos.X + room.Width -1;
                for (int y = room.GlobalPos.Y; y < room.GlobalPos.Y + room.Height; y++)
                {
                    var tile = new Tile();
                    tile.Pos = new Point(leftX, y);
                    tile.Type = TileType.Wall;
                    addTile(tile);

                    tile = new Tile();
                    tile.Pos = new Point(rightX, y);
                    tile.Type = TileType.Wall;
                    addTile(tile);
                }
                for (int x = room.GlobalPos.X +1; x < room.GlobalPos.X + room.Width -1; x++)
                {
                    for (int y = room.GlobalPos.Y +1; y < room.GlobalPos.Y + room.Height-1; y++)
                    {
                        var tile = new Tile();
                        tile.Pos = new Point(x, y);
                        tile.Type = TileType.Ground;
                        addTile(tile);
                    }
                }
            }

            System.Action<Point> ifNothingAddWall = (Point p) =>
            {
                if (grid[ p.X , p.Y] == null)
                {
                    addTile( new Tile()
                    {
                        Pos = p,
                        Type = TileType.Wall
                    });
                }
            };

            System.Action<Point, Point> addUp = (Point a, Point b) => 
            {
                for (int y = a.Y; y <= b.Y; y++) 
                {
                    if( grid[a.X, y] == null )
                    {
                        addTile( new Tile()
                        {
                            Pos = new Point( a.X, y )
                        });
                    }

                    grid[a.X, y].Type = TileType.Ground;

                    ifNothingAddWall (new Point(a.X -1, y));
                    ifNothingAddWall (new Point(a.X +1, y));
                }

                ifNothingAddWall (new Point(a.X - 1 , b.Y + 1));
                ifNothingAddWall (new Point(a.X + 1 , b.Y + 1));

            };

            System.Action<Point, Point> addRight = (Point a, Point b) => 
            {
                for (int x = a.X; x <= b.X; x++) 
                {
                    if( grid[x, a.Y] == null )
                    {
                        grid[x, a.Y] = new Tile()
                        {
                            Pos = new Point( x, a.Y )
                        };
                    }

                    grid[x, a.Y].Type = TileType.Ground;


                    ifNothingAddWall (new Point(x, a.Y-1));
                    ifNothingAddWall (new Point(x, a.Y+1));
                }

                ifNothingAddWall (new Point(b.X +1, a.Y-1));
                ifNothingAddWall (new Point(b.X +1, a.Y+1));
            };

            foreach (var corridor in this.corridors)
            {
                Point a = corridor.Points[0];
                for (int i = 0; i < corridor.Points.Count; i++)
                {
                    Point b = corridor.Points[i];
                    if (a.X == b.X)
                    {
                        if (a.Y > b.Y)
                        {
                            addUp(b, a);
                        }
                        else
                        {
                            addUp(a, b);
                        }
                    }
                    else
                    {
                        if (a.X > b.X)
                        {
                            addRight(b, a);
                        }
                        else
                        {
                            addRight(a, b);
                        }
                    }

                    a = b;
                }
            }

            List<Tile> tiles = new List<Tile>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int y = 0; y < grid.GetLength(1); y++) 
                {
                    if (grid[i, y] != null)
                    {
                        tiles.Add(grid[i, y]);
                    }
                }
            }

            this.LevelDrawer.DrawDungeon(tiles);
        }
    }
}
