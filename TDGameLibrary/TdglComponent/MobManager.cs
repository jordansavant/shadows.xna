using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TDGameLibrary.Mobs;
using System;
using TDGameLibrary.Map;
using TDGameLibrary.Weapons;
using TDGameLibrary.DataStructures;

namespace TDGameLibrary.Components
{
    public class MobManager : TdglComponent
    {
        //TODO: all list.Contains() run in O(n), so switch to a dictionary/hashtable or something to get O(1)
        public MobManager(WorldManager worldManager)
        {
            WorldManager = worldManager;
            
            _mobs = new MutableDistinctSet<Mob>();
            MobBodies = new Dictionary<PhysicalBody, Mob>();
            MobFixtures = new Dictionary<Fixture, Mob>();
            MobBucketManager = new MobBucketManager(80, WorldManager);

            //MobSpriteBatches = new Dictionary<int, SpriteBatch>();
            //for (int z = GameEnvironment.WorldLayerMinimum; z < GameEnvironment.WorldLayerMaximum; z++)
            //{
            //    MobSpriteBatches[z] = new SpriteBatch(GameEnvironment.Game.GraphicsDevice);
            //}

            MobSpriteBatch = new SpriteBatch(GameEnvironment.Game.GraphicsDevice);
        }

        protected Dictionary<int, SpriteBatch> MobSpriteBatches;
        protected SpriteBatch MobSpriteBatch;
        protected Dictionary<PhysicalBody, Mob> MobBodies;
        protected Dictionary<Fixture, Mob> MobFixtures;
        protected Dictionary<Vector2, List<Mob>> MobBuckets;
        protected MobBucketManager MobBucketManager;
        
        public WorldManager WorldManager { get; private set; }

        protected MutableDistinctSet<Mob> _mobs;
        public IEnumerable<Mob> Mobs
        {
            get
            {
                return _mobs;
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Mob mob in Mobs)
            {
                mob.AlreadyCollided.Clear();
            }

            foreach (Mob mob in Mobs)
            {
                if (mob.IsUpdateEnabled)
                {
                    InspectMob(mob);

                    mob.Update(gameTime);

                    if (mob.IsDrawEnabled)
                    {
                        mob.AnimatedSprite.Update(gameTime);
                    }
                }

                if (!(mob is DrawOnlyMob))
                {
                    MobBucketManager.UpdateMobBucket(mob);
                }
            }

            MobBucketManager.Update();
            _mobs.Commit();
        }

        public override void Draw(GameTime gameTime)
        {
            //for (int z = GameEnvironment.WorldLayerMinimum; z < GameEnvironment.WorldLayerMaximum; z++)
            {
                MobSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                    SamplerState.LinearWrap, null, null, null, Matrix.Identity);
            }

            foreach (Mob mob in Mobs)
            {
                if (mob.IsDrawEnabled && !mob.IsDestroyed)
                {
                    // Moved here from update because we had sync issues.
                    mob.AnimatedSprite.Position = mob.PositionOnScreen;
                    // --

                    mob.Draw(gameTime, MobSpriteBatch, CalculateMobDrawDepth(mob));
                }
            }

            //for (int z = GameEnvironment.WorldLayerMinimum; z < GameEnvironment.WorldLayerMaximum; z++)
            {
                MobSpriteBatch.End();
            }
        }


        public virtual void RegisterMob(Mob mob)
        {
            if (mob != null && !Mobs.Contains(mob))
            {
                _mobs.Add(mob);

                if (!(mob is DrawOnlyMob))
                {
                    if (mob.PhysicalBody is FarseerPhysicalBody)
                    {
                        FarseerPhysicalBody physicalBody = (FarseerPhysicalBody)mob.PhysicalBody;

                        foreach (Fixture fixture in physicalBody.Body.FixtureList)
                        {
                            MobFixtures[fixture] = mob;
                            fixture.OnCollision -= OnMobCollision; //Unregister in case event is already hooked (no exception is thrown if absent)
                            fixture.OnCollision += OnMobCollision;
                        }
                    }
                    else
                    {
                        MobBodies[mob.PhysicalBody] = mob;
                    }
                }
            }
        }


        public IEnumerable<Mob> GetMobsInCircle(Vector2 origin, int radius)
        {
            return MobBucketManager.GetMobsInCircle(origin, radius);
            //return GetMobsInCircle<Mob>(origin, radius);
        }

        public IEnumerable<Mob> GetMobsInCircle<T>(Vector2 origin, int radius)
            where T : Mob
        {
            return GetMobsInCircle(origin, radius, new Type[] { typeof(T) });
        }

        public IEnumerable<Mob> GetMobsInCircle(Vector2 origin, int radius, IEnumerable<Type> typeList)
        {
            return FilterMobListByType(MobBucketManager.GetMobsInCircle(origin, radius), typeList);
        }

        public IEnumerable<Mob> GetMobsInRectangle(Rectangle rectangle)
        {
            return GetMobsInRectangle<Mob>(rectangle);
        }

        public IEnumerable<Mob> GetMobsInRectangle<T>(Rectangle rectangle)
            where T : Mob
        {
            return GetMobsInRectangle(rectangle, new Type[] { typeof(T) });
        }

        public IEnumerable<Mob> GetMobsInRectangle(Rectangle rectangle, IEnumerable<Type> typeList)
        {
            return FilterMobListByType(MobBucketManager.GetMobsInRectangle(rectangle), typeList);
        }

        private IEnumerable<Mob> FilterMobListByType(IEnumerable<Mob> mobList, IEnumerable<Type> typeList)
        {
            List<Mob> resultList = new List<Mob>();

            foreach (Mob mob in mobList)
            {
                foreach (Type type in typeList)
                {
                    if (type.IsInstanceOfType(mob))
                    {
                        resultList.Add(mob);
                    }
                }
            }

            return resultList;
        }


        public virtual void UnregisterMob(Mob mob)
        {
            if (mob != null && Mobs.Contains(mob))
            {
                _mobs.Remove(mob);

                if (!(mob is DrawOnlyMob))
                {
                    // fixtures
                    List<Fixture> fixturesToUnregister = new List<Fixture>();
                    foreach (Fixture key in MobFixtures.Keys)
                    {
                        if (MobFixtures[key] == mob)
                        {
                            fixturesToUnregister.Add(key);
                        }
                    }
                    foreach (Fixture fixture in fixturesToUnregister)
                    {
                        fixture.OnCollision -= OnMobCollision;
                        MobFixtures.Remove(fixture);
                    }

                    // physical bodies
                    List<PhysicalBody> bodiesToUnregister = new List<PhysicalBody>();
                    foreach (PhysicalBody key in MobBodies.Keys)
                    {
                        if (MobBodies[key] == mob)
                        {
                            bodiesToUnregister.Add(key);
                        }
                    }
                    foreach (PhysicalBody p in bodiesToUnregister)
                    {
                        MobBodies.Remove(p);
                    }
                
                    MobBucketManager.UpdateMobBucket(mob);
                }
            }
        }


        protected virtual float CalculateMobDrawDepth(Mob mob)
        {
            // With a spritebatch ordering BackToFront, the lower the drawDepth, the higher the rendering
            float screenPositionY = (float)(int)mob.PositionOnScreen.Y;
            float drawDepth = screenPositionY;
            float worldLayerCount = (GameEnvironment.WorldLayerMaximum + 1);

            if(screenPositionY == 0)
            {
                drawDepth = 1;
            }
            else if (screenPositionY > 0)
            {
                drawDepth = 1 / screenPositionY;
            }
            else
            {
                drawDepth = 1 + 1 / screenPositionY;
            }

            if (drawDepth == 0)
            {
                drawDepth = 1;
            }

            float lessImportantDrawDepth = drawDepth / 10f;

            float onePointFourStepThingy = 1f / worldLayerCount;
            float slot = onePointFourStepThingy * mob.WorldLayer;
            slot = 1 - slot;

            // Add Z factor
            drawDepth = slot - lessImportantDrawDepth;

            return drawDepth;
        }


        protected virtual bool OnMobCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (!MobFixtures.ContainsKey(f1) || !MobFixtures.ContainsKey(f2))
                return false;

            Mob f1Mob = MobFixtures[f1];
            Mob f2Mob = MobFixtures[f2];


            if (f1Mob != null && f2Mob != null)
            {
                if (f1Mob.WorldLayer == f2Mob.WorldLayer)
                {
                    return f1Mob.Collide(f2Mob);
                }
            }

            return false;
        }


        protected virtual bool OnMobCollision(PhysicalBody f1, PhysicalBody f2)
        {
            Mob f1Mob = MobBodies[f1];
            Mob f2Mob = MobBodies[f2];

            if (f1Mob != null && f2Mob != null)
            {
                if (f1Mob.WorldLayer == f2Mob.WorldLayer)
                {
                    return f1Mob.Collide(f2Mob);
                }
            }

            return false;
        }

        /*
         * Lets extending classes inspect each mob during primary update iteration.
         */
        protected virtual void InspectMob(Mob mob)
        {
            // Empty on purpose.
        }
    }

}
