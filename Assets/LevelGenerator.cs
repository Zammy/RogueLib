using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour
{
    private class Door
    {
        public Coordinate Pos;
        public Room[] Rooms = new Room[2];
        public Direction Dir; //from Rooms[0] to Rooms[1]

        public Door(Coordinate pos, Direction dir, Room room)
        {
            this.Pos = pos;
            this.Dir = dir;
            this.Rooms[0] = room;
        }

        public override string ToString()
        {
            return string.Format("[Door] {0}", this.Pos);
        }
    }

    private class Room
    {
        public Coordinate Pos; //this is the bottom left corner of the room
        public int Width;
        public int Height;

        public List<Door> Doors = new List<Door>();

        public bool IsDoorOn(Coordinate pos)
        {
            foreach (var door in this.Doors)
            {
                if (door.Pos.X == pos.X && door.Pos.Y == pos.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public Direction OnWhichWallIsDoor(Door door)
        {
            if (door.Pos.X == this.Pos.X)
            {
                return Direction.Left;
            }
            if (door.Pos.X == this.Pos.X + this.Width)
            {
                return Direction.Right;
            }
            if (door.Pos.Y == this.Pos.Y)
            {
                return Direction.Up;
            }
            if (door.Pos.Y == this.Pos.Y + this.Height)
            {
                return Direction.Down;
            } 

            throw new UnityException("This door is totally not in this room!");
        }


        public Coordinate GetRandomSpaceInsideRoom()
        {
            return new Coordinate( this.Pos.X + 1 + Random.Range(0, this.Width-1), this.Pos.Y + 1 + Random.Range(0, this.Height -1) );
        }

        public void AddRandomDoorToRoom()
        {
            Coordinate doorPos;

            List<Direction> allDirections = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            foreach (var door in this.Doors)
            {
                if (door.Rooms[0] == this)
                {
                    allDirections.Remove(door.Dir);
                }
                else
                {
                    allDirections.Remove(door.Dir.Opposite());
                }
            }
            Direction dir = allDirections[Random.Range(0, allDirections.Count)];
            switch(dir)
            {
                case Direction.Up:
                {
                    doorPos.X = this.Pos.X + Random.Range(1, this.Width);
                    doorPos.Y = this.Pos.Y + this.Height;
                    break;
                }
                case Direction.Right:
                {
                    doorPos.X = this.Pos.X + this.Width;
                    doorPos.Y = this.Pos.Y + Random.Range(1, this.Height);
                    break;
                }
                case Direction.Down:
                {
                    doorPos.X = this.Pos.X + Random.Range(1, this.Width);
                    doorPos.Y = this.Pos.Y;
                    break;
                }
                case Direction.Left:
                {
                    doorPos.X = this.Pos.X;
                    doorPos.Y = this.Pos.Y + Random.Range(1, this.Height);
                    break;
                }
                default:
                    throw new UnityException("Nope!");
            }

            this.Doors.Add(new Door(doorPos, dir, this));
        }

        public void AddDoor(Door door)
        {
            this.Doors.Add(door);
            door.Rooms[1] = this;
        }

        public override string ToString()
        {
            return string.Format("[Room] {0} Rooms:({1})", this.Pos, this.Doors);
        }
    }

    public LevelDrawer LevelDrawer;

    public int Seed;

    public int Room_MinSize;
    public int Room_MaxSize;
    public int Room_MaxDoorCount;
    public int Room_Number;

    List<Room> rooms = new List<Room>();

    void Start () 
    {
        Random.seed = this.Seed;
    }

    public void GenerateAndDrawWholeDungeon()
    {
        this.Generate();
       
        this.LevelDrawer.ClearRooms();
        foreach(Room room in this.rooms)
        {
            this.LevelDrawer.DrawRoom( GetTilesForRoom(room) );
        }
    }

    public bool Step()
    {
        if (this.rooms.Count == Room_Number)
        {
            return true;
        }
        else
        {
            this.GenRoom();
            return false;
        }
    }

    void Generate()
    {
        this.rooms.Clear();
        for (int i = 0; i < Room_Number; i++)
        {
            this.GenRoom();
        }
    }

    void AddRoom(Room room)
    {
        this.rooms.Add(room);
        this.LevelDrawer.DrawRoom( GetTilesForRoom(room) );
    }

    void GenRoom()
    {
        //get all doors with empty rooms
        List<Door> doors = new List<Door>();
        foreach (var r in this.rooms)
        {
            bool foundEmptyDoor = false;
            foreach (var d in r.Doors)
            {
                if (d.Rooms[1] == null)
                {
                    foundEmptyDoor = true;
                    doors.Add(d);
                }
            }
            if (foundEmptyDoor)
                break;
        }

        //gen new room
        var newRoom = new Room()
        {
            Width = Random.Range(Room_MinSize, Room_MaxSize+1),
            Height = Random.Range(Room_MinSize, Room_MaxSize+1)
        };

        if (doors.Count == 0)
        {
            //first room
            newRoom.Pos = new Coordinate(0, 0);
        }
        else
        {
            Door door = doors[ Random.Range(0, doors.Count) ] ;

            this.FindPosForNewRoomOnDoor(door, newRoom);

            newRoom.AddDoor(door);
        }

        //add doors equal to or less than remaining rooms
        int doorsToAdd = Room_MaxDoorCount - 1;
        doorsToAdd = Mathf.Min(doorsToAdd, Room_Number - this.rooms.Count - 1); //-1 because we add current room at the end
        Debug.Log("max doorsToAdd " + doorsToAdd);
        if (doorsToAdd > 0)
        {
            doorsToAdd = Random.Range(1, doorsToAdd+1);
        }
        Debug.Log("doorsToAdd " + doorsToAdd);

        for (int i = 0; i < doorsToAdd; i++)
        {
            newRoom.AddRandomDoorToRoom();
        }

        this.AddRoom(newRoom);
    }

    void FindPosForNewRoomOnDoor(Door door, Room newRoom)
    {
        var oldRoom = door.Rooms[0];
        var posOptions = new List<Coordinate>();
        switch (door.Dir)
        {
            case Direction.Up:
            {
                int y = oldRoom.Pos.Y + oldRoom.Height;
                for (int x = door.Pos.X - (newRoom.Width - 1); x < door.Pos.X - 1; x++)
                {
                    posOptions.Add(new Coordinate(x, y));
                }
                break;
            }
            case Direction.Down:
            {
                int y = oldRoom.Pos.Y - newRoom.Height;
                for (int x = door.Pos.X - (newRoom.Width - 1); x < door.Pos.X - 1; x++)
                {
                    posOptions.Add(new Coordinate(x, y));
                }
                break;
            }
            case Direction.Left:
            {
                int x = oldRoom.Pos.X - newRoom.Width;
                for (int y = door.Pos.Y - (newRoom.Height - 1); y < door.Pos.Y - 1; y++)
                {
                    posOptions.Add(new Coordinate(x, y));
                }
                break;
            }
            case Direction.Right:
            {
                int x = oldRoom.Pos.X + oldRoom.Width;
                for (int y = door.Pos.Y - (newRoom.Height - 1); y < door.Pos.Y - 1; y++)
                {
                    posOptions.Add(new Coordinate(x, y));
                }
                break;
            }
            default:
                break;
        }
        for (int i = posOptions.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, posOptions.Count);
            var posOption = posOptions[index];
            posOptions.RemoveAt(index);
            newRoom.Pos = posOption;
            if (!IsRoomCollidingWithOtherRoom(newRoom))
            {
                break;
            }
        }
    }

    bool IsRoomCollidingWithOtherRoom(Room room)
    {
        foreach(var r in this.rooms)
        {
            if (r.Pos.X < room.Pos.X + room.Width &&
                r.Pos.X + r.Width > room.Pos.X &&
                r.Pos.Y < room.Pos.Y + room.Height &&
                r.Pos.Y + r.Height  > room.Pos.Y)
            {
                return true;
            }
        }

        return false;
    }

    static List<Tile> GetTilesForRoom(Room room)
    {
        var tiles = new List<Tile>(room.Width * room.Height);
        for (int x = 0; x <= room.Width; x++)
        {
            for (int y = 0; y <= room.Height; y++)
            {
                var pos = new Coordinate(x + room.Pos.X, y + room.Pos.Y);
                var tile = new Tile() {
                    Pos = pos,
                    Type = TileType.Ground
                };
                //                    if (pos == startPos)
                //                    {
                //                        tile.Type = TileType.Start;
                //                    }
                //                    else
                if ((x == 0 || y == 0 || x == room.Width || y == room.Height) && !room.IsDoorOn(pos))
                {
                    tile.Type = TileType.Wall;
                }
                tiles.Add(tile);
            }
        }
        return tiles;
    }
}
