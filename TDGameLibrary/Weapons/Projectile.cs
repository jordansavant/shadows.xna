using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using TDGameLibrary.Components;
using TDGameLibrary.Mobs;

namespace TDGameLibrary.Weapons
{
    public class Projectile : Mob
    {
        public Projectile(WeaponManager weaponManager, PhysicalBody physicalBody)
            : base(weaponManager.MobManager, physicalBody)
        {
            WeaponManager = weaponManager;
            WeaponManager.RegisterProjectile(this);

            IsComplete = false;
            TimeAlive = TimeSpan.FromSeconds(0);
            TimeDie = TimeSpan.FromSeconds(10);

            CancelCollisionMobs = new List<Mob>();
        }


        public WeaponManager WeaponManager;
        public bool IsComplete;
        protected TimeSpan TimeAlive, TimeDie;
        public List<Mob> CancelCollisionMobs;


        public virtual void ApplyCollisionEffects()
        {
            IsComplete = true;
        }


        public virtual void ApplyTimeOfDeathEffects()
        {
            IsComplete = true;
        }


        public override void Update(GameTime gameTime)
        {
            // Update Mob
            base.Update(gameTime);

            if (TimeDie.TotalSeconds > 0)
            {
                // If I have outlived my life, apply my death effects
                TimeAlive += gameTime.ElapsedGameTime;
                if (TimeAlive >= TimeDie)
                {
                    ApplyTimeOfDeathEffects();
                }
            }

            // If I am complete, end me
            if (IsComplete)
            {
                Destruct();
            }
        }


        public override bool Collide(Mob other)
        {
            ApplyCollisionEffects();

            return true;
        }


        public override void Destruct()
        {
            if (!IsDestroyed)
            {
                WeaponManager.UnregisterProjectile(this);

                base.Destruct();
            }
        }


    }
}
