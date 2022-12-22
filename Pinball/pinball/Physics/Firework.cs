using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Data;

namespace pinball.Physics
{
    public class Firework : Particle
    {
        public int Type;
        public float Age;
        private Random _random = new Random();

        public Firework(FireworkRule rule, float invMass) : base(invMass)
        {
            LoadRule(rule);
            Acceleration = new Vector2(0, 10);
        }

        public bool Update(float duration)
        {
            Step(duration);
            Age -= duration;
            return (Age < 0) || (Position.Y < 0);
        }

        private void LoadRule(FireworkRule rule)
        {
            Type = rule.Type;
            Age = _random.NextSingle() * (rule.MaxAge - rule.MinAge) + rule.MinAge;
            Velocity = _random.NextSingle() * (rule.MaxVelocity - rule.MinVelocity) + rule.MinVelocity;
            Damping = rule.Damping;
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
        public int Count;

        public int PayloadCount;
    }
}
