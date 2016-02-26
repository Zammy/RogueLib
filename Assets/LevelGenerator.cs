using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Coordinate
{
    public int X;
    public int Y;

    public Coordinate (int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static bool operator ==(Coordinate a, Coordinate b) 
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Coordinate a, Coordinate b) 
    {
        return !(a.X == b.X && a.Y == b.Y);
    }

    public override bool Equals(object obj)
    {
        if (obj is Coordinate)
        {
            return this == (Coordinate)obj;
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return 17 + this.X + this.Y * 23;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", this.X, this.Y);
    }
}

public enum TileType
{
    Wall,
    Ground,
    Start,
    End
}

public struct Tile
{
    public Coordinate Pos;
    public TileType Type;
}

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
            return new Coordinate( this.Pos.X + 1 + Random.Range(0, this.Width-2), this.Pos.Y + 1 + Random.Range(0, this.Height -2) );
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

    void Start () 
    {
        Random.seed = this.Seed;
    }

    public void Go()
    {
        var tiles = this.Generate();
        this.LevelDrawer.Draw(tiles);
    }

    List<Room> rooms = new List<Room>();

    public List<Tile> Generate()
    {
        this.rooms.Clear();
              
        var tiles = new List<Tile>();
        var startRoom = GenStartRoom();
        rooms.Add(startRoom);

        //add starting tile
        var startPos = startRoom.GetRandomSpaceInsideRoom();
        tiles.Add(new Tile()
        {
            Pos = startPos,
            Type = TileType.Start
        });

        this.TryGenRoom();

        foreach(Room room in this.rooms)
        {
            for (int x = 0; x <= room.Width; x++)
            {
                for (int y = 0; y <= room.Height; y++)
                {
                    var pos = new Coordinate( x + room.Pos.X, y + room.Pos.Y );
                    var tile = new Tile()
                    {
                        Pos = pos,
                        Type = TileType.Ground
                    };

                    if (pos == startPos)
                    {
                        tile.Type = TileType.Start;
                    }
                    else if ( (x == 0 || y == 0 || x == room.Width || y == room.Height) 
                        && !room.IsDoorOn( pos ))
                    {
                        tile.Type = TileType.Wall;
                    }
                    tiles.Add(tile);
                }
            }
        }

        return tiles;
    }

    Room GenStartRoom()
    {
        var room = new Room()
        {
            Pos = new Coordinate(0, 0),
            Width = Random.Range(Room_MinSize, Room_MaxSize),
            Height =  Random.Range(Room_MinSize, Room_MaxSize)
        };

        AddRandomDoorToRoom(room);

        return room;
    }

    void AddRandomDoorToRoom(Room room)
    {
        Coordinate doorPos;
        Direction dir =  Direction.Up; //(Direction) Random.Range(0,3);
        switch(dir)
        {
            case Direction.Up:
            {
                doorPos.X = room.Pos.X + Random.Range(1, room.Width-1);
                doorPos.Y = room.Pos.Y + room.Height;
                break;
            }
            case Direction.Right:
            {
                doorPos.X = room.Pos.X + room.Width;
                doorPos.Y = room.Pos.Y + Random.Range(1, room.Height-1);
                break;
            }
            case Direction.Down:
            {
                doorPos.X = room.Pos.X + Random.Range(1, room.Width-1);
                doorPos.Y = room.Pos.Y;
                break;
            }
            case Direction.Left:
            {
                doorPos.X = room.Pos.X;
                doorPos.Y = room.Pos.Y + Random.Range(1, room.Height);
                break;
            }
            default:
                throw new UnityException("Nope!");
        }

        room.Doors.Add(new Door(doorPos, dir, room));
    }

    void TryGenRoom()
    {
        if (this.rooms.Count == this.Room_Number)
        {
            Debug.Log("Gened all rooms");
            return;
        }

        //if last room do not forget to add end


        //get all doors with empty rooms
        List<Door> doors = new List<Door>();
        foreach(var r in this.rooms)
        {
            foreach (var d in r.Doors)
            {
                if (d.Rooms[1] == null)
                {
                    doors.Add(d);
                }
            }
        }

        if (doors.Count == 0)
        {
            throw new UnityException("Bah! No doors? This can't be!");
        }

        //choose 1 at random
        Door door = doors[ Random.Range(0, doors.Count - 1) ] ;

        //gen new room
        var oldRoom = door.Rooms[0];
        var newRoom = new Room()
        {
            Width = Random.Range(Room_MinSize, Room_MaxSize),
            Height =  Random.Range(Room_MinSize, Room_MaxSize)
        };

        var posOptions = new List<Coordinate>();
        switch (door.Dir)
        {
            case Direction.Up:
            {
                int y = oldRoom.Pos.Y + oldRoom.Height;
                for (int x = door.Pos.X - newRoom.Width+1; x < door.Pos.X + newRoom.Width -2; x++)
                {
                    posOptions.Add( new Coordinate( x, y ) );
                }
                break;
            }
            case Direction.Down:
            {
//                float y = door.Rooms[0].Pos.Y + door.Rooms[0].Height;

                break;
            }
            default:
                break;
        }

        for (int i = posOptions.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, posOptions.Count -1);
            var posOption = posOptions[ index ];
            posOptions.RemoveAt(index);

            newRoom.Pos = posOption;
            if (!IsRoomCollidingWithOtherRoom(newRoom))
            {
                break;
            }
        }

        newRoom.AddDoor(door);

        this.rooms.Add(newRoom);

        //add doors equal to or less than remaining rooms
        //recurse
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

//    if (rect1.x < rect2.x + rect2.width &&
//   rect1.x + rect1.width > rect2.x &&
//   rect1.y < rect2.y + rect2.height &&
//   rect1.height + rect1.y > rect2.y) {
//    // collision detected!
//}
}
