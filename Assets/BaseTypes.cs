public struct Coordinate
{
    public int X;
    public int Y;

    public Coordinate (int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public Coordinate(Coordinate toCopy)
    {
        this.X = toCopy.X;
        this.Y = toCopy.Y;
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

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public static class DirectionExt
{
    public static Direction Opposite(this Direction dir)
    {
        return (Direction) (((int)dir + 2) % 4);
    }
}
