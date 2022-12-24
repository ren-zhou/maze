using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace pinball.Physics
{
    public class Firework : Particle
    {
        public int Type;
        public float Age;
        public int PayloadType;
        public int PayloadCount;
        public Color Color;

        private Random _random = new Random();

        public Firework(FireworkRule rule, Color color) : base(1)
        {
            Type = rule.Type;
            Age = _random.NextSingle() * (rule.MaxAge - rule.MinAge) + rule.MinAge;
            Velocity.X = _random.NextSingle() * (rule.MaxVelocity.X - rule.MinVelocity.X) + rule.MinVelocity.X;
            Velocity.Y = _random.NextSingle() * (rule.MaxVelocity.Y - rule.MinVelocity.Y) + rule.MinVelocity.Y;
            Debug.WriteLine(Velocity.ToString());
            Damping = rule.Damping;
            Acceleration = new Vector2(0, 20);
            InvMass = 1;
            PayloadCount = rule.PayloadCount;
            Color = color;
        }

        public Firework(FireworkRule rule, Firework parent, Color color) : this(rule, color)
        {
            if (parent != null)
            {
            Velocity += parent.Velocity;
            Position = parent.Position;
            }
            else
            {
                Position = new Vector2(300, 300);
                Velocity.Y = -300;
            }
        }

        public bool Update(float duration)
        {
            Step(duration);
            Age -= duration;
            return (Age < 0) || (Position.Y < 0);
        }
    }

    public class Payload
    {
        public int Type;
        public int Count;
        public Payload(int type, int count)
        {
            Type = type;
            Count = count;
        }
    }

    public class FireworkRule
    {
        public float MinAge;
        public float MaxAge;
        public Vector2 MinVelocity;
        public Vector2 MaxVelocity;
        public float Damping;

        public int Type;

        public int PayloadCount;
        public Payload[] Payloads;

        public FireworkRule(float minAge, float maxAge, Vector2 minVelocity, Vector2 maxVelocity, float damping, int type, int payloadCount)
        {
            MinAge = minAge;
            MaxAge = maxAge;
            MinVelocity = minVelocity * 10;
            MaxVelocity = maxVelocity * 10;
            Damping = damping;
            Type = type;
            PayloadCount = payloadCount;
            Payloads = new Payload[payloadCount];
        }
    }

    public class FireworkDemo
    {

        private FireworkRule[] _fireworkRules;
        private Firework[] _fireworks;

        private Texture2D _sparkTexture;

        private int _nextFireworkInd;
        private Random _random = new Random();

        public static int MAXFIREWORKS = 1024;

        private Color[] _colors = { new Color(0, 140, 255) };
        public FireworkDemo()
        {
            _nextFireworkInd = 0;

            _fireworkRules = new FireworkRule[6];
            _fireworks = new Firework[MAXFIREWORKS];

            _fireworkRules[0] = new FireworkRule(0, 0, Vector2.Zero, Vector2.Zero, 0, 0, 0); // empty firework
            _fireworkRules[1] = new FireworkRule(0.5f, 1.4f, new Vector2(-5, 25), new Vector2(5, 28), 0.1f, 1, 2);
            _fireworkRules[1].Payloads[0] = new Payload(5, 10);
            _fireworkRules[1].Payloads[1] = new Payload(5, 10);

            _fireworkRules[2] = new FireworkRule(0.5f, 1.0f, new Vector2(-5, -20), new Vector2(5, 20), 0.8f, 2, 1);
            _fireworkRules[2].Payloads[0] = new Payload(4, 2);

            _fireworkRules[3] = new FireworkRule(0.5f, 1.5f, new Vector2(-5, -5), new Vector2(5, 5), 0.1f, 3, 0);

            _fireworkRules[4] = new FireworkRule(0.25f, 0.5f, new Vector2(-20, 5), new Vector2(20, 5), 0.2f, 4, 0);

            _fireworkRules[5] = new FireworkRule(0.5f, 1.0f, new Vector2(-12, -12), new Vector2(12, 12), 0.01f, 5, 1);
            _fireworkRules[5].Payloads[0] = new Payload(3, 5);
        }

        public void LoadContent(ContentManager content)
        {
            _sparkTexture = content.Load<Texture2D>("spark");
        }

        public void Update(float duration)
        {
            foreach (Firework f in _fireworks)
            {
                if (f != null && f.Type > 0)
                {
                    if (f.Update(duration))
                    {
                        FireworkRule rule = _fireworkRules[f.Type];
                        f.Type = 0;
                        foreach (Payload p in rule.Payloads)
                        {
                            Create(p.Type, p.Count, f);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Firework f in _fireworks)
            {
                if (f != null && f.Type > 0)
                {
                    spriteBatch.Draw(_sparkTexture, new Rectangle((int)f.Position.X, (int)f.Position.Y, 2, 2), f.Color);
                }
            }
        }

        public void Create(int type, Firework parent, Color color)
        {
            FireworkRule rule = _fireworkRules[type];
            _fireworks[_nextFireworkInd] = new Firework(rule, parent, color);
            _nextFireworkInd = (_nextFireworkInd + 1) % MAXFIREWORKS;

        }

        public void Create(int type, int amount, Firework parent)
        {
            Color color = _colors[_random.Next(_colors.Length)];
            for (int i = 0; i < amount; i++)
            {
                Create(type, parent, color);
            }
        }
    }
}
