using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pinball.Physics
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float Damping;
        public float InvMass;
        public Texture2D Texture;

        public Particle(float _invMass)
        {
            InvMass = _invMass;
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Acceleration = new Vector2(0, 0);
            Damping = 0.999f;
        }

        public Particle(Vector2 position, Vector2 velocity, Vector2 acceleration, float dampening, float invMass, Texture2D texture)
        {
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Damping = dampening;
            InvMass = invMass;
            Texture = texture;
        }

        public void Step(float duration)
        {
            // should be called every game frame
            Position += Velocity * duration;
            Velocity = Damping * Velocity + Acceleration * duration;
        }

    }
}