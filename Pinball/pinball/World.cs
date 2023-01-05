using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pinball
{
    internal class World
    {
        private Random _random;
        private int _seed;
        private static readonly int _timespan = 4;
        private static readonly int _height = 4;
        private static readonly int _width = 4;
        private int[,,] _map;

        public World(int seed)
        {
            _seed = seed;
            GenerateWorld();
        }

        public World()
        {
            _seed = new Random().Next();
            GenerateWorld();

        }

        private void GenerateWorld()
        {
            _random = new Random(_seed);
            _map = new int[_timespan, _width, _height];
            // width is x, length is z
            int y = 0;
            while (y < _height)
            {
                int dir = _random.Next(5);
                switch (dir)
                {
                    //case 0:

                }
            }
        }


    }

    internal class RoomRequirements
    {
        public bool Entrance;
        public bool Left;
        public bool Right;
        public bool Up;
        public bool Down;
        public bool Exit;
    }

    public class Room
    {
        private RoomRequirements Requirements;

        public Room(int type)
        {
            Requirements = new RoomRequirements();
            switch (type)
            {
                case 1:
                    Requirements.Left = true;
                    Requirements.Right = true;
                    break;
                case 2:
                    Requirements.Left = true;
                    Requirements.Down = true;
                    break;
            }
        }

        public int[,] MatrixLayout(int width, int height)
        {
            //int[,] layout = new int[width, height];
            int[,] layout = new int[4, 4]
            {
                { 1, 1, 1, 1},
                { 1, 0, 0, 1},
                { 1, 0, 0, 1},
                { 1, 1, 1, 1},
            };
            //for (int i = 0; i < width; ++i)
            //{
            //    for (int j = 0; j < height; ++j)
            //    {
            //        layout[i, j] = 1;
            //    }
            //}

            if (Requirements.Left)
            {
                layout[1, 0] = 0;
                layout[2, 0] = 0;
            }
            if (Requirements.Right)
            {
                layout[1, 3] = 0;
                layout[2, 3] = 0;
            }
            if (Requirements.Down)
            {
                layout[3, 1] = 0;
                layout[3, 2] = 0;
            }

            return layout;
        }
    }
}
