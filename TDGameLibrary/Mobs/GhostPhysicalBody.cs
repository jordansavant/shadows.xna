using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using TDGameLibrary.Components;
using FarseerTools;

namespace TDGameLibrary.Mobs
{
    public class GhostPhysicalBody : PhysicalBody
    {
        public GhostPhysicalBody(GhostBodyManager ghostBodyManager)
        {
            GhostBodyManager = ghostBodyManager;
            GhostBodyManager.RegisterGhost(this);
            NetForces = Vector2.Zero;
        }

        public GhostBodyManager GhostBodyManager { get; private set; }
        private TimeSpan ElapsedGameTime;
        public Vector2 NetForces;
        private Vector2 _linearVelocity = Vector2.Zero;
        public override Vector2 LinearVelocity
        {
            get
            {
                return _linearVelocity;
            }
            set
            {
                _linearVelocity = value;
            }
        }

        public override float Mass
        {
            get
            {
                return Density * (float)(Math.PI * Math.Pow(Radius, 2));
            }
        }

        public override void ApplyForce(Vector2 force)
        {
            NetForces += force;
        }

        public override void ApplyLinearImpulse(Vector2 force)
        {
            NetForces += force * 2; // hehe - wannabe farseer
        }

        private float _linearVelocityAdjustment = 1f;
        public override void ApplyLinearVelocityAdjustment(float factor)
        {
            _linearVelocityAdjustment = factor * .5f;
        }

        public virtual void Update(GameTime gameTime)
        {
            ElapsedGameTime = gameTime.ElapsedGameTime;
            float step = (float)ElapsedGameTime.TotalSeconds;

            LastPostition = Position;

            // Apply Damping
            NetForces *= MathHelper.Clamp(1.0f - step * LinearDamping, 0.0f, 1.0f);

            // Apply Force
            _linearVelocity = step * (1 / Mass) * NetForces;

            // Update Position
            Position += _linearVelocity * _linearVelocityAdjustment;

            // Reset
            _linearVelocityAdjustment = 1f;
        }

        public override void Collide(PhysicalBody other)
        {
            // Get the reflected force.
            //Vector2 reflection = DetectReflectedForce(other);

            // Multiply the reflected force by the current linear velocity
            //Vector2 reflectedForce = Vector2.Zero;
            //reflectedForce.X = Math.Abs(NetForces.X);
            //reflectedForce.Y = Math.Abs(NetForces.Y);
            //reflectedForce *= reflection;

            //NetForces = Vector2.Zero;
            //ApplyForce(reflectedForce);
        }

        public override void Reset()
        {
            NetForces = Vector2.Zero;
            LinearVelocity = Vector2.Zero;
            LinearDamping = 0f;
        }

        public override void Destruct()
        {
            GhostBodyManager.UnregisterGhost(this);
            base.Destruct();
        }
    }
}
