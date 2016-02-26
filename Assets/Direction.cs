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