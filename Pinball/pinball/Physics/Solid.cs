using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace pinball.Physics
{

    public class Level
    {
        public List<Actor> Actors;
        private LevelMap _layout;
        private Texture2D _playerSprite;
        public Level(GraphicsDevice device)
        {
            Actors = new List<Actor>();
            _layout = new LevelMap(device);
            Actors.Add(new Actor(new Rectangle(0, 0, 16, 16)));
        }

        public void LoadContent(ContentManager content)
        {
            _playerSprite = content.Load<Texture2D>("player");
        }

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);
        }

        public void Update(float duration)
        {
            KeyboardState keyState = Keyboard.GetState();
            foreach (Actor actor in Actors)
            {
                actor.ProcessInput(keyState);
                float xDist = _layout.DistanceToWall(actor.ForwardEdgeX, actor.BoundingBox.Top, actor.BoundingBox.Bottom, actor.Velocity.X > 0, LevelMap.Axis.X);
                //Debug.WriteLine("xdist: " + xDist.ToString());
                if (Math.Abs(xDist) < Math.Abs(actor.Velocity.X))
                {
                    actor.Velocity.X = xDist;
                }
                actor.BoundingBox.X += (int) actor.Velocity.X;

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _layout.Draw(spriteBatch);
            foreach (Actor actor in Actors)
            {
                actor.Draw(spriteBatch, _playerSprite);
            }
        }
    }

    public class LevelMap
    {
        public int CellSize;
        public int Width;  // in cells
        public int Height;  // in cells
        public int[,] Map;
        private Texture2D _texture;

        public enum Axis
        {
            X,
            Y,
        }

        public LevelMap(GraphicsDevice device)
        {
            Map = new int[4, 4]
            {
                { 0, 0, 0, 1},
                { 0, 1, 0, 1},
                { 1, 0, 0, 1},
                { 1, 1, 0, 0}, 
            };
            Width = 4;
            Height = 4;
            CellSize = 32;
            _texture = CreateTexture(device);
        }

        public float DistanceToWallX(int worldX, int worldYStart, int worldYEnd, bool facingRight)
        {
            int x = worldX / CellSize;
            int yStart = worldYStart / CellSize;
            int yEnd = worldYEnd / CellSize;
            int step = facingRight ? 1 : -1;

            while (x >= 0 && x < Width)
            {
                for(int y = yStart; y <= yEnd; y++)
                {
                    if (Map[y, x] == 1) { goto ConvertToWorldCoord; }
                }
                x += step;
            }

        ConvertToWorldCoord:
            x = facingRight ? x : x + 1;
            return x * CellSize - worldX;
        }

        public float DistanceToWall(int worldCross, int worldStart, int worldEnd, bool seekPositive, Axis movementAxis)
        {
            int cross = worldCross / CellSize;
            int start = worldStart / CellSize;
            int end = worldEnd / CellSize;
            int step = seekPositive ? 1 : -1;
            int limit = movementAxis == Axis.X ? Width : Height;

            while (cross >=0 && cross < Width)
            {
                for (int i = start; i <= end; i++)
                {
                    if (movementAxis == Axis.X && Map[i, cross] == 1) { goto ConvertToWorldCoord; }
                    if (movementAxis == Axis.Y && Map[cross, i] == 1) { goto ConvertToWorldCoord; }
                }
                cross += step;
            }
        ConvertToWorldCoord:
            cross = seekPositive ? cross : cross + 1;
            return cross * CellSize - worldCross;
        }

        private Texture2D CreateTexture(GraphicsDevice device)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, Width * CellSize, Height * CellSize);
            //the array holds the color for each pixel in the texture
            Color[] data = new Color[Width * Height * CellSize * CellSize];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {

                //the function applies the color according to the specified pixel
                int x = pixel / (CellSize * Width) / CellSize;
                int y = pixel % (CellSize * Width) / CellSize;
                data[pixel] = Map[x, y] == 1 ? Color.White : Color.Black;
            }
            

            //set the color
            texture.SetData(data);

            return texture;
        }

        public bool HasCollision(int x, int y)
        {
            x /= CellSize;
            y /= CellSize;

            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return true;
            return Map[x , y] == 1;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Rectangle(0, 0, Width * CellSize, Height * CellSize), Color.White);
        }
    }


    public class Actor
    {
        public Rectangle BoundingBox;

        public Vector2 Velocity;
        public Actor(Rectangle box)
        {
            Velocity = Vector2.Zero;
            BoundingBox = box;
        }



        public void Draw(SpriteBatch spriteBatch, Texture2D platformTexture)
        {
            spriteBatch.Draw(platformTexture, BoundingBox, Color.White);
        }

        public void SetX(float x, bool setRight)
        {
            BoundingBox.X = (int) (setRight ? x - BoundingBox.Width : x);
        }

        public int ForwardEdgeX
        {
            get { return Velocity.X > 0 ? BoundingBox.Right : BoundingBox.Left; }
        }


        public void ProcessInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.J))
            {
                Velocity.X = -1;
            }
            else if (keyState.IsKeyDown(Keys.L))
            {
                Velocity.X = 1;
            }
            else
            {
                Velocity.X = 0;
            }
            if (keyState.IsKeyDown(Keys.I))
            {
                Velocity.Y = -1;
            }
            else if (keyState.IsKeyDown(Keys.K))
            {
                Velocity.Y = 1;
            }
            else
            {
                Velocity.Y = 0;
            }
        }

        //public void IsRiding(Solid solid);
        //public void SetRiding(Solid solid, bool isRiding);

        //public void Move(float x, float y);

        //public void Move(Vector2 movement, Action onCollide);

    }
}
