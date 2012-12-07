using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerTools;

namespace TDGameLibrary.Mobs
{
    public class FarseerPhysicalBody : PhysicalBody
    {
        public FarseerPhysicalBody(Body body)
        {
            Body = body;
        }

        public Body Body { get; private set; }

        public override Vector2 Position
        {
            get
            {
                return Body.Position;
            }
            set
            {
                Body.Position = value;
            }
        }
        public override Vector2 CenterPoint { get { return Body.WorldCenter; } }
        public override float Rotation { get { return Body.Rotation; } set { Body.Rotation = value; } }

        public override Category CollisionCategories
        {
            set
            {
                Body.CollisionCategories = value;
            }
        }
        public override Category CollidesWith
        {
            set
            {
                Body.CollidesWith = value;
            }
        }

        public override Vector2 LinearVelocity { get { return Body.LinearVelocity; } set { Body.LinearVelocity = value; } }
        public override float LinearDamping { get { return Body.LinearDamping; } set { Body.LinearDamping = value; } }
        public override float AngularDamping { get { return Body.AngularDamping; } set { Body.AngularDamping = value; } }
        public override float Friction { get { return Body.Friction; } set { Body.Friction = value; } }
        public override float Restitution { get { return Body.Restitution; } set { Body.Restitution = value; } }
        public override float Radius { get { return Body.FixtureList.FirstOrDefault().Shape.Radius; } set { Body.FixtureList.FirstOrDefault().Shape.Radius = value; } }
        public override float Density { get { return Body.FixtureList.FirstOrDefault().Shape.Density; } set { Body.FixtureList.FirstOrDefault().Shape.Density = value; } }
        public override float Mass { get { return Body.Mass; } }
        public override bool IsSensor { set { Body.IsSensor = value; } }
        public override bool IsBullet { set { Body.IsBullet = value; } }
        public override bool IsStatic { get { return Body.IsStatic; } set { Body.IsStatic = value; } }

        public override void ApplyForce(Vector2 force)
        {
            Body.ApplyForce(force);
        }

        public override void ApplyLinearImpulse(Vector2 force)
        {
            Body.ApplyLinearImpulse(force);
        }

        public override void ApplyLinearVelocityAdjustment(float factor)
        {
            Body.LinearVelocity *= factor;
        }

        public override void Collide(PhysicalBody other)
        {
            if (!(other is FarseerPhysicalBody))
            {
                return; // TO DO: Farseer needs to respond to Ghost body collisions.
            }
        }

        public override void Reset()
        {
            Body.ResetMassData();
            Body.ResetDynamics();
        }

        public override void Destruct()
        {
            Body.Dispose();
            base.Destruct();
        }
    }
}
