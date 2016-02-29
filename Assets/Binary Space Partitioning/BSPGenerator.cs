using UnityEngine;
using System.Collections;

namespace BSP
{
    public class BinaryNode
    {
        public Coordinate LocalPos;
        public Coordinate GlobalPos;
        public int Width;
        public int Height;

        public BinaryNode[] Leaves = new BinaryNode[2];

        public Room Room {get; private set; }

        public BinaryNode(Coordinate local, Coordinate glob, int width, int height)
        {
            this.LocalPos = local;
            this.GlobalPos = glob;
            this.Width = width;
            this.Height = height;
        }

        public int Area()
        {
            return this.Width * this.Height;
        }

        public void GenRoom(int MIN_ROOM_SIDE)
        {
            var room = new Room();

            var newPos = new Coordinate ();
            newPos.X = Random.Range(0, Mathf.Min(this.Width/2, this.Width - MIN_ROOM_SIDE));
            newPos.Y = Random.Range(0, Mathf.Min(this.Height/2, this.Height -MIN_ROOM_SIDE));
            room.LocalPos = newPos;

            room.GlobalPos = this.GlobalPos + room.LocalPos;

            room.Width = Random.Range(MIN_ROOM_SIDE, this.Width - newPos.X  );
            room.Height = Random.Range(MIN_ROOM_SIDE, this.Height - newPos.Y );

            this.Room = room;
        }
    }

    public class Room
    {
        public Coordinate LocalPos;
        public Coordinate GlobalPos;
        public int Width;
        public int Height;
    }


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

        BinaryNode root;

        void Start()
        {
            Random.seed = Seed;
        }

        public void GenAndDraw()
        {
            root = new BinaryNode(Coordinate.Zero, Coordinate.Zero, DUN_WIDTH, DUN_HEIGHT);
           
            Divide(root);

            GenRooms(root);

            this.Drawer.DrawNode(root);
        }

        void Divide(BinaryNode node)
        {
            if (node.Area() < MAX_LEAF_AREA)
            {
                return;
            }

//            int direction  = Random.Range(0, 2);
            int direction = 0;
            if (node.Width < node.Height)
            {
                direction = 1;
            }

            if (direction == 0) // split vertically
            {
                int x = Random.Range(MIN_LEAF_SIDE, node.Width-MIN_LEAF_SIDE);
                node.Leaves[0] = new BinaryNode(Coordinate.Zero, new Coordinate(node.GlobalPos), x, node.Height);
                node.Leaves[1] = new BinaryNode(new Coordinate(x, 0), new Coordinate(node.GlobalPos.X + x, node.GlobalPos.Y), node.Width - x, node.Height);
            }
            else // split horizontally
            {
                int y = Random.Range(MIN_LEAF_SIDE, node.Height-MIN_LEAF_SIDE);
                node.Leaves[0] = new BinaryNode(Coordinate.Zero, new Coordinate(node.GlobalPos), node.Width, y);
                node.Leaves[1] = new BinaryNode(new Coordinate(0, y), new Coordinate(node.GlobalPos.X, node.GlobalPos.Y + y), node.Width, node.Height - y);
            }

            Divide(node.Leaves[0]);
            Divide(node.Leaves[1]);
        }

        void GenRooms(BinaryNode node)
        {
            if (node == null)
                return;

            node.GenRoom(MIN_ROOM_SIDE);

            GenRooms(node.Leaves[0]);
            GenRooms(node.Leaves[1]);
        }
    }
}
