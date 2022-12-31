using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pinball.Physics
{

    public class Level
    {
        public List<Platform> Platforms;
        public List<Actor> Actors;
        private Texture2D _platformTexture;
        private LevelMap _layout;
        private Texture2D _playerSprite;
        public Level(GraphicsDevice device)
        {
            Platforms = new List<Platform>();
            Actors = new List<Actor>();
            _layout = new LevelMap(device);
            //Platforms.Add(new Platform(new Rectangle(10, 10, 20, 60)));
            Actors.Add(new Actor(new Rectangle(0, 0, 32, 32)));
        }

        public void LoadContent(ContentManager content)
        {
            _platformTexture = content.Load<Texture2D>("spark");
            _playerSprite = content.Load<Texture2D>("player");
        }
        public void AddSolid(Platform platform)
        {
            Platforms.Add(platform);
        }

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);
        }

        public void Update(float duration)
        {

            foreach (Actor actor in Actors)
            {
                actor.ProcessInput(Keyboard.GetState());
                actor.MoveX(duration);
                int bound = _layout.SeekWallX(actor.BoundingBox.Center.X, new int[] { actor.BoundingBox.Center.Y }, actor.Velocity.X > 0);
                int dir = actor.Velocity.X > 0 ? 1 : -1;
                if (actor.Velocity.X != 0 && dir*bound <= dir*actor.FFEdgeX())
                {
                    Debug.WriteLine("bound: " + bound.ToString());
                    actor.SetPositionX(bound, actor.Velocity.X > 0);
                    actor.Velocity.X = 0;
                }
                actor.Step(duration);
                actor.Update();

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _layout.Draw(spriteBatch);
            foreach (Actor actor in Actors)
            {
                actor.Draw(spriteBatch, _playerSprite);
            }
            foreach (Platform platform in Platforms)
            {
                platform.Draw(spriteBatch, _platformTexture);
            }
        }
    }
    public interface Solid
    {

    }

    public class LevelMap
    {
        public enum Direction
        {
            Left,
            Right,
        }

        public int CellSize;
        public int Width;  // in cells
        public int Height;  // in cells
        public int[,] Map;
        private Texture2D _texture;

        public LevelMap(GraphicsDevice device)
        {
            Map = new int[4, 4]
            {
                { 0, 0, 0, 1},
                { 1, 1, 0, 1},
                { 1, 0, 0, 1},
                { 1, 1, 0, 0}, 
            };
            Width = 4;
            Height = 4;
            CellSize = 32;
            _texture = CreateTexture(device);
        }

        public int SeekWallX(int worldX, int[] worldY, bool facingRight)
        {
            //return CellSize * 3;
            int step = facingRight ? 1 : -1;
            int gridX = worldX / CellSize;
            int[] gridY = worldY.Select((index, el) => el / CellSize).ToArray();
            //Debug.WriteLine("----");
            //Debug.WriteLine(worldX.ToString());
            //Debug.WriteLine(gridX.ToString());
            //Debug.WriteLine(gridY[0].ToString());

            while (gridX >= 0 && gridX < Width)
            {
                foreach (int y in gridY)
                {
                    if (Map[y, gridX] == 1)
                    {
                        Debug.WriteLine("grid pos: " + gridX.ToString() + ", "+ y.ToString());
                        goto ConvertToWorld;
                    }
                }
                gridX += step;
            }
        ConvertToWorld:
            gridX = facingRight ? gridX : gridX + 1;
            return gridX * CellSize;

        }

        //public int SeekWallY(int worldX)


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

    public class Platform
    {
        public Rectangle BoundingBox;

        public Platform(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        public bool IsColliding(Actor actor)
        {
            return BoundingBox.Intersects(actor.BoundingBox);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D platformTexture)
        {
            spriteBatch.Draw(platformTexture, BoundingBox, Color.BurlyWood);
        }
    }

    public class Actor : Particle
    {
        public Rectangle BoundingBox;
        public bool FacingRight;
        public Actor(Rectangle box) : base(1)
        {
            BoundingBox = box;
            Position = new Vector2(box.X, box.Y);
            FacingRight = true;
        }



        public void Update()
        {
            BoundingBox.X = (int) Position.X;
            BoundingBox.Y = (int) Position.Y;
            
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D platformTexture)
        {
            spriteBatch.Draw(platformTexture, BoundingBox, Color.White);
        }


        public int FFEdgeX()  // Forward-facing Edge
        {
            return Velocity.X > 0 ? BoundingBox.Right : BoundingBox.Left;
        }

        public int FFEdgeY()
        {
            return Velocity.Y > 0 ? BoundingBox.Bottom : BoundingBox.Top;
        }

        public void ProcessInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.J))
            {
                Velocity.X = -10;
            }
            else if (keyState.IsKeyDown(Keys.L))
            {
                Velocity.X = 10;
            }
            else
            {
                Velocity.X = 0;
            }
            if (keyState.IsKeyDown(Keys.I))
            {
                Velocity.Y = -10;
            }
            else if (keyState.IsKeyDown(Keys.K))
            {
                Velocity.Y = 10;
            }
            else
            {
                Velocity.Y = 0;
            }
        }

        public void SetPositionX(float x, bool setRight)
        {
            if (setRight) { Position.X = x - BoundingBox.Width; }
            else { Position.X = x; }
        }

        public new void Step(float duration)
        {
            Velocity = MathF.Pow(Damping, duration) * Velocity + Acceleration * duration;
        }

        public void MoveX(float duration)
        {
            Position.X += Velocity.X * duration;
        }

        public void MoveY(float duration)
        {
            Position.Y += Velocity.Y * duration;
        }
        //public void IsRiding(Solid solid);
        //public void SetRiding(Solid solid, bool isRiding);

        //public void Move(float x, float y);

        //public void Move(Vector2 movement, Action onCollide);

    }
}
