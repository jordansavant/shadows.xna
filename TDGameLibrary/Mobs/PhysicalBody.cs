using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;

namespace TDGameLibrary.Mobs
{
    public class PhysicalBody
    {
        public PhysicalBody()
        {
        }

        public virtual Vector2 LastPostition { get; set; }

        public virtual Vector2 Position { get; set; }
        public virtual Vector2 CenterPoint { get { return new Vector2(Position.X + Radius, Position.Y + Radius); } }
        public virtual float Rotation { get; set; }

        public virtual Category CollisionCategories { get; set; }
        public virtual Category CollidesWith { get; set; }

        public virtual Vector2 LinearVelocity { get; set; }
        public virtual float LinearDamping { get; set; }
        public virtual float AngularDamping { get; set; }
        public virtual float Friction { get; set; }
        public virtual float Restitution { get; set; }
        public virtual float Radius { get; set; }
        public virtual float Density { get; set; }
        public virtual float Mass { get; private set; }
        public virtual bool IsSensor { get; set; }
        public virtual bool IsBullet { get; set; }
        public virtual bool IsStatic { get; set; }

        public virtual void ApplyForce(Vector2 force)
        {
        }

        public virtual void ApplyLinearImpulse(Vector2 force)
        {
        }

        public virtual void ApplyLinearVelocityAdjustment(float factor)
        {
        }

        public virtual void Collide(PhysicalBody other)
        {
            Vector2 perpendicular = DetectReflectedForce(other);
            ApplyForce(perpendicular);
        }

        protected virtual Vector2 DetectReflectedForce(PhysicalBody other)
        {
            Vector2 positionVector = other.Position - Position;
            float angle = (float)Math.Atan2(positionVector.Y, positionVector.X) - (float)Math.PI;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public virtual void Reset()
        {
        }

        public virtual void Destruct()
        {
        }
    }
}
