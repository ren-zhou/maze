using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pinball.Physics
{
    public interface IForceGenerator
    {
        void UpdateForce(Particle body, float duration);
    }

    public class ParticleSubscription
    {
        private readonly IForceGenerator _generator;
        private readonly Particle _particle;

        public ParticleSubscription(Particle particle, IForceGenerator generator)
        {
            _particle = particle;
            _generator = generator;
        }
    }

    public class ForceRegistry
    {
        public List<ParticleSubscription> Subscriptions;
        public ForceRegistry()
        {
            Subscriptions = new List<ParticleSubscription>();
        }

        public void Clear()
        {
            Subscriptions.Clear();
        }

        public void Add(Particle particle, IForceGenerator generator)
        {
            Subscriptions.Add(new ParticleSubscription(particle, generator));
        }

        public void Update(float duration)
        {

        }
    }
}
