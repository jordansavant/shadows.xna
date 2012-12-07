using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Animation;
using FarseerTools;
using TDGameLibrary.Weapons;
using TDGameLibrary.Utility;
using FarseerPhysics.Common;
using TDGameLibrary.Components;
using System.Diagnostics;
using TDGameLibrary.DataStructures;


namespace TDGameLibrary.Mobs
{
    public class Mob : IDestroyable
    {
        public Mob(MobManager mobManager, PhysicalBody physicalBody)
        {
            IsDestroyed = false;

            MobManager = mobManager;
            PhysicalBody = physicalBody;
            if (PhysicalBody != null)
            {
                PositionInWorldByUpdate = PositionInWorld;
                PositionOnScreenByUpdate = PositionOnScreen;
                UpdateCollisionRectangle();
            }
            SetWorldLayer(3);
            MobManager.RegisterMob(this);

            IsDrawEnabled = true;
            IsUpdateEnabled = true;
            IsCollidable = true;
            
            Direction = Vector2.Zero;

            UseHitBox = false;
            NearbyMobs = new List<Mob>();
            StepsUntilRefresh = RefreshOffset++ % 20;
        }

        public MobManager MobManager { get; private set; }

        public bool IsDrawEnabled;
        public bool IsUpdateEnabled;
        public bool IsDestroyed { get; set; } // interface
        public IEnumerable<Mob> NearbyMobs;
        public int StepsUntilRefresh;
        private static int RefreshOffset;
        public bool IsCollidable;

        public event EventHandler OnDestroy;

        /// <summary>
        /// Gets or sets the direction toward which the mob moves spontaneously.
        /// </summary>
        public Vector2 Direction;

        /// <summary>
        /// Gets or sets the speed at which the mob moves spontaneously. (Meters per Second)
        /// </summary>
        public float Speed;

        /// <summary>
        /// Gets or sets the position of the mob relative to the world origin. (Pixels)
        /// </summary>
        public virtual Vector2 PositionInWorld { get { return ConvertUnits.ToDisplayUnits(PhysicalBody.Position); } }
        public Vector2 PositionInWorldByUpdate;

        /// <summary>
        /// Gets the point on the screen at which the mob's animated sprite will be drawn. (Pixels)
        /// </summary>
        public virtual Vector2 PositionOnScreen { get { return PositionInWorld - MobManager.WorldManager.ScreenPositionInWorld; } }
        public Vector2 PositionOnScreenByUpdate;

        public AnimatedSprite AnimatedSprite;

        /// <summary>
        /// Gets or sets the physics body for this mob.
        /// The design ensures the body is registered with the mob manager for collision detection.
        /// </summary>
        public PhysicalBody PhysicalBody;
        public void SetPhysicalBodyPosition(Vector2 newPosition)
        {
            PhysicalBody.Position = newPosition;
            UpdateCollisionRectangle();
        }
        // Used for when external events move the gunman drastically like when the game resets.
        public void SyncPositioning()
        {
            PositionInWorldByUpdate = PositionInWorld;
            PositionOnScreenByUpdate = PositionOnScreen;
        }

        /// <summary>
        /// Gets or sets a rectangle centered on the X and Y screen coordinates of the mob. (Pixels)
        /// </summary>
        public Rectangle CollisionRectangle;
        public Rectangle CollisionRectangleOnScreen
        {
            get
            {
                Vector2 s = CollisionRectangle.ToVector2();
                s = s - MobManager.WorldManager.ScreenPositionInWorld;
                Rectangle r = new Rectangle((int)s.X, (int)s.Y, CollisionRectangle.Width, CollisionRectangle.Height);
                return r;
            }
        }
        public bool UseHitBox;
        public Rectangle HitBox;
        public Rectangle HitBoxOnScreen
        {
            get
            {
                Vector2 s = HitBox.ToVector2();
                s = s - MobManager.WorldManager.ScreenPositionInWorld;
                Rectangle r = new Rectangle((int)s.X, (int)s.Y, HitBox.Width, HitBox.Height);
                return r;
            }
        }
        /// <summary>
        /// Gets or sets the layer (sub-world) to which the mob belongs. Mobs in different layers cannot collide, and mobs in higher layers
        /// are drawn above mobs in lower layers. Values outside the range of available layers (0-31) are automatically clamped.
        /// </summary>
        public int WorldLayer { get; private set; }
        protected void SetWorldLayer(int worldLayer)
        {
            WorldLayer = (int)MathHelper.Clamp(worldLayer, GameEnvironment.WorldLayerMinimum, GameEnvironment.WorldLayerMaximum);

            if (PhysicalBody != null)
            {
                Category cat = (Category)(int)Math.Pow(2, WorldLayer);
                PhysicalBody.CollisionCategories = cat;
                PhysicalBody.CollidesWith = cat;
            }
        }

        public void UpdateCollisionRectangle()
        {
            CollisionRectangle.X = (int)ConvertUnits.ToDisplayUnits(PhysicalBody.Position.X - PhysicalBody.Radius);
            CollisionRectangle.Y = (int)ConvertUnits.ToDisplayUnits(PhysicalBody.Position.Y - PhysicalBody.Radius);
            CollisionRectangle.Width = (int)(ConvertUnits.ToDisplayUnits(PhysicalBody.Radius * 2));
            CollisionRectangle.Height = (int)(ConvertUnits.ToDisplayUnits(PhysicalBody.Radius * 2));

            UpdateHitBox();
        }

        public virtual void UpdateHitBox()
        {
            HitBox = CollisionRectangle;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsDestroyed)
            {
                return;
            }

            PositionInWorldByUpdate = PositionInWorld;
            PositionOnScreenByUpdate = PositionOnScreen;

            UpdateCollisionRectangle();

            ApplyMotilityForce();
        }


        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, float layerDepth)
        {
            AnimatedSprite.Draw(gameTime, spriteBatch, layerDepth);
        }


        public virtual void Enable()
        {
            IsDrawEnabled = true;
            IsUpdateEnabled = true;
        }


        public virtual void Disable()
        {
            IsDrawEnabled = false;
            IsUpdateEnabled = false;
        }


        /// <summary>
        /// Apply force needed to move mob spontaneously in its current Direction at its current Speed.
        /// </summary>
        public virtual void ApplyMotilityForce()
        {
            if (Direction != Vector2.Zero)
                Direction.Normalize();

            PhysicalBody.ApplyForce(Direction * Speed);
        }

        public DistinctSet<Mob> AlreadyCollided = new DistinctSet<Mob>();
        bool thisCollidesWithOther = false;
        bool otherCollidesWithThis = false;
        bool thisApplyCollisionForces = false;
        bool otherApplyCollisionForces = false;
        Vector2 thisPositionOffset = Vector2.Zero;
        Vector2 otherPositionOffset = Vector2.Zero;
        Vector2 thisNewVelocity = Vector2.Zero;
        Vector2 otherNewVelocity = Vector2.Zero;
        Vector2 thisNewNetForces = Vector2.Zero;
        Vector2 otherNewNetForces = Vector2.Zero;
        bool ProjectileHasCollidedWithBoss;
        protected bool IsFixForBigColliding = true;
        public virtual void CheckCollide(object sender, EventArgs e)
        {
            if (sender == this)
                return;

            //hack to prevent projectiles spinning around inside of mobs
            if ((this is Projectile && HasCollided.ContainsKey(sender.GetHashCode())) || (sender is Projectile && HasCollided.ContainsKey(GetHashCode())))
            {
                return;
            }

            Mob other = sender as Mob;
            if (IsCollidable && other != null && other.IsCollidable && !AlreadyCollided.Contains(other))
            {
                if (IsPhysicallyIntersecting(other))
                {
                    GhostPhysicalBody ghost = PhysicalBody as GhostPhysicalBody;
                    GhostPhysicalBody otherGhost = other.PhysicalBody as GhostPhysicalBody;

                    if (ghost == null && otherGhost == null)
                    {
                        return; //two farseer bodies. Collision is handled elsewhere
                    }

                    thisCollidesWithOther = false;
                    otherCollidesWithThis = false;
                    VectorHelper.Zero(ref thisPositionOffset);
                    VectorHelper.Zero(ref otherPositionOffset);
                    VectorHelper.Zero(ref thisNewVelocity);
                    VectorHelper.Zero(ref otherNewVelocity);
                    VectorHelper.Zero(ref thisNewNetForces);
                    VectorHelper.Zero(ref otherNewNetForces);

                    //only apply collision forces if not a projectile or planet
                    thisApplyCollisionForces = !(other is Projectile) && !PhysicalBody.IsStatic;
                    otherApplyCollisionForces = !(this is Projectile) && !other.PhysicalBody.IsStatic;

                    if (Collide(other) || (IsFixForBigColliding && other.IsFixForBigColliding && CollisionRectangle.Width > 100 && other.CollisionRectangle.Width > 100))
                    {
                        thisCollidesWithOther = true;
                        if (thisApplyCollisionForces)
                        {
                            CalculateCollisionForces(other, this, ghost, ref thisPositionOffset, ref thisNewVelocity, ref thisNewNetForces); //, true);
                        }

                        if (this is Projectile)
                        {
                            HasCollided.AddKeyIfMissing(other.GetHashCode());
                        }
                    }

                    if (other.Collide(this) || (IsFixForBigColliding && other.IsFixForBigColliding && CollisionRectangle.Width > 100 && other.CollisionRectangle.Width > 100))
                    {
                        otherCollidesWithThis = true;
                        if (otherApplyCollisionForces)
                        {
                            CalculateCollisionForces(this, other, otherGhost, ref otherPositionOffset, ref otherNewVelocity, ref otherNewNetForces); //, false);
                        }

                        if (other is Projectile)
                        {
                            HasCollided.AddKeyIfMissing(GetHashCode());
                        }
                    }

                    if (thisCollidesWithOther && thisApplyCollisionForces)
                    {
                        if (ghost != null) //This is null if the instance is a planet or the Gunman (the only Farseer bodies)
                        {
                            PhysicalBody.LinearVelocity = thisNewVelocity;
                            PhysicalBody.Position += thisPositionOffset;
                            UpdateCollisionRectangle();
                            
                            //normally, would do me.NetForces = newNetForces but need to hack so projectiles don't recollide with large bosses
                            if (!PhysicalBody.IsStatic && !other.PhysicalBody.IsStatic &&
                                (CollisionRectangle.Width >= 150 || other.CollisionRectangle.Width >= 150))
                            {
                                if (!ProjectileHasCollidedWithBoss)
                                {
                                    ghost.NetForces = thisNewNetForces;
                                }
                                //to prevent recollision, if colliding with 'boss' (i.e. large nonstatic mob), disable all further
                                //collisions WITH BOSSES ONLY (don't hack collision algorithm for cases involving small mobs)
                                ProjectileHasCollidedWithBoss = true;
                            }
                            else
                            {
                                ghost.NetForces = thisNewNetForces;
                            }
                        }
                    }

                    if (otherCollidesWithThis && otherApplyCollisionForces)
                    {
                        if (otherGhost != null) //This is null if the instance is a planet or the Gunman (the only Farseer bodies)
                        {
                            other.PhysicalBody.LinearVelocity = otherNewVelocity;
                            other.PhysicalBody.Position += otherPositionOffset;
                            other.UpdateCollisionRectangle();
                            
                            //normally, would do me.NetForces = newNetForces but need to hack so projectiles don't recollide with large bosses
                            if (!PhysicalBody.IsStatic && !other.PhysicalBody.IsStatic &&
                                (CollisionRectangle.Width >= 150 || other.CollisionRectangle.Width >= 150))
                            {
                                if (!other.ProjectileHasCollidedWithBoss)
                                {
                                    otherGhost.NetForces = otherNewNetForces;
                                }
                                //to prevent recollision, if colliding with 'boss' (i.e. large nonstatic mob), disable all further
                                //collisions WITH BOSSES ONLY (don't hack collision algorithm for cases involving small mobs)
                                other.ProjectileHasCollidedWithBoss = true;
                            }
                            else
                            {
                                otherGhost.NetForces = otherNewNetForces;
                            }
                        }
                    }

                    //Once the first mob handles the collision, we do not try again with the other mob
                    AlreadyCollided.Add(other); 
                    //other.AlreadyCollided.Add(this); <- for whatever reason, this breaks collisions with ShardSkimmers. Handle with care
                }
            }
        }

        //Made this static so we don't accidentally reference properties in the current mob. Want this method to work the same for any two mobs!! 092512ss
        private static void CalculateCollisionForces(Mob other, Mob self, GhostPhysicalBody selfGhost, ref Vector2 selfPositionOffset, ref Vector2 selfNewVelocity, ref Vector2 selfNewNetForces) //bool invertVelocity)
        {
            bool selfIsProjectile = self is Projectile;

            //Calculate collision force
            Vector2 lineToOther = self.CollisionRectangle.Center.ToVector2() - other.CollisionRectangle.Center.ToVector2();
            Vector2 n = lineToOther; //the collision normal is the direction along the line between the two bodies
            if (n == Vector2.Zero)
            {
                return;
            }
            n.Normalize();
            Vector2 v = self.PhysicalBody.LinearVelocity;// + other.PhysicalBody.LinearVelocity;
            if (v == Vector2.Zero) //this actually happens quite often for some reason. Way more often than the other such sanity checks in this method
            {
                return;
            }
            //v = invertVelocity ? -v : v;
            Vector2 u = n * Vector2.Dot(v, n); //need this if not normalized:   n * (Vector2.Dot(v, n) / Vector2.Dot(n, n));
            Vector2 w = v - u; //v'=wf-ur, or v'=w-u in a perfectly elastic, frictionless collision (w * Friction - u * Restitution)
            selfNewVelocity = w - u;

            //Redirect net forces (ghost projectile bodies only)
            if (selfGhost != null && selfIsProjectile)
            {
                Vector2 newVelocityDirection = selfNewVelocity;
                if (newVelocityDirection == Vector2.Zero)
                {
                    return;
                }
                newVelocityDirection.Normalize();
                Vector2 oldVelocityDirection = self.PhysicalBody.LinearVelocity;
                if (oldVelocityDirection == Vector2.Zero)
                {
                    return;
                }
                oldVelocityDirection.Normalize();
                float angleBetween = oldVelocityDirection.GetAngleBetweenVectors(newVelocityDirection, Vector2.Zero);
                selfNewNetForces = selfGhost.NetForces.RotateVector(-angleBetween);

                //DEBUG
                //DrawUtility.DrawLine(self.PositionOnScreen,
                //        self.PositionOnScreen + (newVelocityDirection * 100), Color.Red, 2);
            }

            //To prevent a recollision, move along the collision normal away from the other mob
            Vector2 collisionRectangleDiagonal = new Vector2(self.CollisionRectangle.Width / 2f, self.CollisionRectangle.Height / 2f);
            Vector2 otherCollisionRectangleDiagonal = new Vector2(other.CollisionRectangle.Width / 2f, 0);

            if (lineToOther.Length() < collisionRectangleDiagonal.Length() + otherCollisionRectangleDiagonal.Length())
            {
                if (other.PhysicalBody.IsStatic)
                {
                    selfPositionOffset =
                        (n * ConvertUnits.ToSimUnits(collisionRectangleDiagonal.Length() + otherCollisionRectangleDiagonal.Length())) +
                        ConvertUnits.ToSimUnits(lineToOther.RotateVector(MathHelper.Pi, Vector2.Zero));
                }
                else if (selfGhost != null && !selfIsProjectile)
                {
                    if (self.CollisionRectangle.Width > 100 && other.CollisionRectangle.Width > 100)
                    {
                        Vector2 test = (n * 30 * ConvertUnits.ToSimUnits(collisionRectangleDiagonal.Length() + otherCollisionRectangleDiagonal.Length())) +
                        ConvertUnits.ToSimUnits(lineToOther.RotateVector(MathHelper.Pi, Vector2.Zero));
                        selfGhost.ApplyForce(test);
                    }
                    selfNewNetForces += n * 50; //arbitrary number. The idea is the two bodies slowly push each other apart, instead of just jumping apart using position
                }
            }

        }

        private bool IsPhysicallyIntersecting(Mob other)
        {
            if (this is Projectile && other.UseHitBox)
            {
                return this.CollisionRectangle.Intersects(other.HitBox);
            }

            return Vector2.Distance(this.PhysicalBody.Position, other.PhysicalBody.Position) - this.PhysicalBody.Radius - other.PhysicalBody.Radius <= 0;
        }

        //total hack to keep projectiles from bouncing around inside of mobs
        //if mobs are ever pooled, this will cause all kinds of havoc
        private Dictionary<int, bool> HasCollided = new Dictionary<int,bool>(8);

        public virtual bool Collide(Mob other)
        {
            return true;
        }


        public virtual void Destruct()
        {
            if (!IsDestroyed)
            {
                IsDestroyed = true;
                MobManager.UnregisterMob(this);
                PhysicalBody.Destruct();
                DestructEvent(EventArgs.Empty);
            }
        }

        protected virtual void DestructEvent(EventArgs e)
        {
            EventHandler handler = this.OnDestroy;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public float GetRotationToMob(Mob mob)
        {
            return ConstrainPi((float)Math.Atan2((mob.PhysicalBody.Position - PhysicalBody.Position).Y, (mob.PhysicalBody.Position - PhysicalBody.Position).X));
        }


        /// <summary>
        /// This method will calculate a vector that is the direction of rotation of the animated sprite.
        /// This is useful for being able to position objects along the center line of the sprite regardless of rotation.
        /// </summary>
        public virtual Vector2 GetVectorBasedOnRotation(float rotation)
        {
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }


        // Constrain radians to PI
        public float ConstrainPi(float radians)
        {
            while (radians < (float)Math.PI)
            {
                radians += (float)Math.PI * 2;
            }
            while (radians > (float)Math.PI)
            {
                radians -= (float)Math.PI * 2;
            }
            return radians;
        }


        public Vector2 GetRandomVector()
        {
            float angle = (float)(GameEnvironment.Random.NextDouble() * MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public float GetRotationFromVector(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
    }

}