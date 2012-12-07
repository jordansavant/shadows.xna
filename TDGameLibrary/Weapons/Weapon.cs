using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Mobs;
using TDGameLibrary.Components;
using FarseerPhysics.Dynamics;
using TDGameLibrary.Audio;
using TDGameLibrary.Utility;

namespace TDGameLibrary.Weapons
{
    public class Weapon : MobAttachment
    {
        public Weapon(WeaponManager weaponManager, Vector2 positionInWorld, float parallax, int worldLayer)
            : base(weaponManager.MobManager, positionInWorld, parallax, worldLayer)
        {
            WeaponManager = weaponManager;
            WeaponManager.RegisterWeapon(this);

            RateOfFire = TimeSpan.FromSeconds(1);
            TickRateOfFireSubtraction = TimeSpan.FromSeconds(0);
            TimeSinceLastFire = RateOfFire; // Initialize equal to rate of fire so first shot is instantaneous

            FirePosition = new Vector2();

            AimDirection = new Vector2(0f, 0f);

            IsTriggered = false;

            FireRadius = 0f;

            RemainingRounds = ClipSize = 0;

            InfiniteAmmo = false;

            BurstTimer = new GameTimer(0f);
            BurstCount = 0;
            BurstMax = 1;
            IsBurstActive = false;
        }


        public WeaponManager WeaponManager { get; private set; }
        public Vector2 FirePosition;
        public float EffectiveRange;
        public TimeSpan RateOfFire;
        public TimeSpan TickRateOfFireSubtraction;
        public TimeSpan TimeSinceLastFire;
        public Vector2 AimDirection;
        private float AimDirectionRadians;
        protected bool IsTriggered, IsTriggerRequested, LastIsTriggeredRequested;
        public float FireRadius;
        public Projectile Projectile;
        public int RemainingRounds, ClipSize;
        public bool InfiniteAmmo;
        public string ReloadSound;
        public string Title;
        public Texture2D Icon;
        public GameTimer BurstTimer;
        protected int BurstCount, BurstMax;
        protected bool IsBurstActive;
        public virtual bool IsReady
        {
            get
            {
                return TimeSinceLastFire >= RateOfFire;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (RemainingRounds > 0)
            {
                // Update Muzzle Position
                if (AimDirection != Vector2.Zero)
                    AimDirection.Normalize();
                AimDirectionRadians = (float)Math.Atan2(AimDirection.Y, AimDirection.X);
                FirePosition.X = (float)(Math.Cos(AimDirectionRadians) * FireRadius + PositionInWorldByUpdate.X);
                FirePosition.Y = (float)(Math.Sin(AimDirectionRadians) * FireRadius + PositionInWorldByUpdate.Y);


                // Update Trigger
                TimeSinceLastFire += gameTime.ElapsedGameTime;

                if (LastIsTriggeredRequested == false && IsTriggerRequested == true)
                {
                    OnTriggerPress();
                }

                if (LastIsTriggeredRequested == true && IsTriggerRequested == false)
                {
                    OnTriggerRelease();
                }

                LastIsTriggeredRequested = IsTriggerRequested;
                IsTriggerRequested = false;

                if (IsTriggered)
                {
                    TimeSinceLastFire = TimeSpan.FromSeconds(0);
                    IsTriggered = false;
                    Burst();
                }

                // Burst
                if (IsBurstActive && BurstCount < BurstMax)
                {
                    if (BurstTimer.Update(gameTime))
                    {
                        Fire();
                        BurstCount++;
                    }

                    if (BurstCount >= BurstMax)
                    {
                        IsBurstActive = false;
                        BurstTimer.Reset();
                    }
                }

            }

            // Update sprite to rotate towards AimDirection
            AnimatedSprite.Rotation = AimDirectionRadians;

            // Reset rate of fire
            base.Update(gameTime);
        }


        public virtual void Trigger()
        {
            IsTriggerRequested = true;

            if (TimeSinceLastFire >= (RateOfFire - TickRateOfFireSubtraction))
            {
                IsTriggered = true;
            }

            TickRateOfFireSubtraction = TimeSpan.FromSeconds(0);
        }


        public virtual void Burst()
        {
            IsBurstActive = true;
            BurstCount = 0;
            BurstTimer.Reset();
        }


        public virtual void Fire()
        {
            if (!InfiniteAmmo)
            {
                RemainingRounds--;
            }

            if (Projectile != null)
            {
                Projectile.OnDestroy -= OnProjectileDestruct;
                Projectile.OnDestroy += OnProjectileDestruct;
            }
        }


        public virtual void OnTriggerPress()
        {
            // placeholder
        }


        public virtual void OnTriggerRelease()
        {
            // placeholder
        }


        public override void Destruct()
        {
            if (!IsDestroyed)
            {
                WeaponManager.UnregisterWeapon(this);
                base.Destruct();
            }
        }


        public void OnProjectileDestruct(object sender, EventArgs e)
        {
            Projectile = null;
        }


        public virtual void Reload()
        {
            if (ReloadSound != null || ReloadSound != "")
            {
                SoundManager.Play(ReloadSound);
            }

            RemainingRounds = ClipSize;
        }

        public virtual void Load()
        {
        }
    }
}
