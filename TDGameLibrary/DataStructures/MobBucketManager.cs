using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Mobs;
using TDGameLibrary.Components;
using Microsoft.Xna.Framework;
using TDGameLibrary.Weapons;

namespace TDGameLibrary.DataStructures
{
    public class MobBucketManager
    {
            public MobBucketManager(int bucketSizeInPixels, WorldManager worldManager)
            {
                WorldManager = worldManager;
                BucketSize = bucketSizeInPixels;

                //Distance (in buckets) past the edge of the screen to continue tracking mobs.
                //Once mobs are outside of this range, they no longer appear in search results.
                BucketPadding = 1;

                NextCollisionRectangle = new Dictionary<Mob, Rectangle>();
                LastCollisionRectangle = new Dictionary<Mob, Rectangle?>();

                InitializeBuckets();
            }


            private void InitializeBuckets()
            {
                Rectangle bucketRectangle = GameEnvironment.ScreenRectangle;

                bucketRectangle.X -= BucketSize * BucketPadding;
                bucketRectangle.Y -= BucketSize * BucketPadding;
                bucketRectangle.Width = AlignToBucketEdge(GameEnvironment.ScreenRectangle.Width + BucketSize * (BucketPadding + 1));
                bucketRectangle.Height = AlignToBucketEdge(GameEnvironment.ScreenRectangle.Height + BucketSize * (BucketPadding + 1));

                MobBuckets = new EventHandler[bucketRectangle.Width / BucketSize, bucketRectangle.Height / BucketSize];
                //for (int x = 0; x < MobBuckets.GetLength(0); x++)
                //{
                //    for (int y = 0; y < MobBuckets.GetLength(1); y++)
                //    {
                //        MobBuckets[x, y] = new CollisionDelegate();
                //    }
                //}
            }

            private Rectangle LastScreenRectangle;
            private Rectangle NextScreenRectangle;
            private Dictionary<Mob, Rectangle?> LastCollisionRectangle { get; set; }
            private Dictionary<Mob, Rectangle> NextCollisionRectangle { get; set; }

            private WorldManager WorldManager;
            private EventHandler[,] MobBuckets;
            private int BucketPadding;
            public int BucketSize { get; private set; }


            public IEnumerable<Mob> GetMobsInCircle(Vector2 origin, int radius)
            {
                IEnumerable<Mob> mobsInRectangle = GetMobsInRectangle(
                    new Rectangle((int)origin.X - radius, (int)origin.Y - radius, radius * 2, radius * 2));

                HashedList<Mob> results = new HashedList<Mob>();

                foreach (Mob mob in mobsInRectangle)
                {
                    if (Vector2.Distance(mob.PositionInWorldByUpdate, origin) <= Math.Abs(mob.CollisionRectangle.Width / 2 + radius))
                    {
                        results.Add(mob);
                    }
                }

                return results;
            }


            //Runs in O(nm) time where n = number of buckets searched and m is total number of mob references from said buckets
            public IEnumerable<Mob> GetMobsInRectangle(Rectangle rectangle)
            {
                DistinctSet<Mob> results = new DistinctSet<Mob>();

                int left = AlignToBucketEdge(rectangle.Left - WorldManager.ScreenRectangle.Left) / BucketSize;
                int top = AlignToBucketEdge(rectangle.Top - WorldManager.ScreenRectangle.Top) / BucketSize;
                int right = AlignToBucketEdge(rectangle.Right - WorldManager.ScreenRectangle.Left) / BucketSize;
                int bottom = AlignToBucketEdge(rectangle.Bottom - WorldManager.ScreenRectangle.Top) / BucketSize;

                for (int x = left; x <= right; x++)
                {
                    for (int y = top; y <= bottom; y++)
                    {
                        if (IndexInBounds(x, y))
                        {
                            //foreach (Mob mob in MobBuckets[x, y])
                            //{
                            //    if (mob.CollisionRectangle.Intersects(rectangle) && !results.Contains(mob))
                            //    {
                            //        results.Add(mob);
                            //        if (results.Count == 1)
                            //            return results;
                            //    }
                            //}
                        }
                    }
                }

                return results;
            }


            public void Update()
            {
                LastScreenRectangle = NextScreenRectangle;
                NextScreenRectangle = WorldManager.ScreenRectangle;
            }


            public void UpdateMobBucket(Mob mob)
            {
                //Register mob in ALL buckets it touches. Thus, a mob mostly inside one bucket but partially inside another
                //  bucket will get registered in BOTH buckets.

                Rectangle r = NextCollisionRectangle[mob] = mob.CollisionRectangle;

                int left = AlignToBucketEdge(r.Left - NextScreenRectangle.Left) / BucketSize;
                int top = AlignToBucketEdge(r.Top - NextScreenRectangle.Top) / BucketSize;
                int right = AlignToBucketEdge(r.Right - NextScreenRectangle.Left) / BucketSize;
                int bottom = AlignToBucketEdge(r.Bottom - NextScreenRectangle.Top) / BucketSize;

                int preleft = 0;
                int pretop = 0;
                int preright = 0;
                int prebottom = 0;
                bool firstRegister = false;

                if (LastCollisionRectangle.ContainsKey(mob) && LastCollisionRectangle[mob].HasValue)
                {
                    preleft = AlignToBucketEdge(LastCollisionRectangle[mob].Value.Left - LastScreenRectangle.Left) / BucketSize;
                    pretop = AlignToBucketEdge(LastCollisionRectangle[mob].Value.Top - LastScreenRectangle.Top) / BucketSize;
                    preright = AlignToBucketEdge(LastCollisionRectangle[mob].Value.Right - LastScreenRectangle.Left) / BucketSize;
                    prebottom = AlignToBucketEdge(LastCollisionRectangle[mob].Value.Bottom - LastScreenRectangle.Top) / BucketSize;
                }
                else
                {
                    firstRegister = true;
                }


                // Add mob to new buckets
                for (int x = left; x <= right; x++)
                {
                    for (int y = top; y <= bottom; y++)
                    {
                        if (!mob.IsDestroyed && IndexInBounds(x, y)) //!MobBuckets[x, y].Contains(mob)) //the boundary check below is faster!!
                        {
                            if (firstRegister || x < preleft || x > preright || y < pretop || y > prebottom) //if bucket does not already have the mob
                            {
                                if (MobBuckets[x, y] != null)
                                {
                                    MobBuckets[x, y].Invoke(mob, EventArgs.Empty);
                                }

                                // I know you cant remember what this was for, DONT TOUCH THIS! IT FIXES THE FUCKING XBOX!!! ROAOOAOAOOAAARR!!1!
                                if (!mob.IsDestroyed && //here, mob can get destroyed in the Invoke call above, so DON'T REMOVE THIS or it will get left in the bucket forever!
                                    (MobBuckets[x, y] == null ||
                                        (mob is Projectile && (MobBuckets[x, y] != null && MobBuckets[x, y].GetInvocationList().Count() < 3)) ||
                                        (MobBuckets[x, y] != null && MobBuckets[x, y].GetInvocationList().Count() < 1)))
                                //if (MobBuckets[x, y] == null)
                                {
                                    MobBuckets[x, y] += mob.CheckCollide;
                                }
                            }
                            /// Jordan added this else.
                            /// The issue was that the bucket only invoked the collision checks
                            /// when a new mob entered the bucket.
                            /// As a result, internal collisions were never detected until a
                            /// new mob entered.
                            else
                            {
                                if (!mob.IsDestroyed && //<-- freaking sannity check cause I dont want to test this exhaustively to see if it MIGHT really happen :P
                                    MobBuckets[x, y] != null)
                                {
                                    MobBuckets[x, y].Invoke(mob, EventArgs.Empty);
                                }
                            }
                        }
                        else if (IndexInBounds(x, y) && MobBuckets[x, y] != null)
                        {
                            MobBuckets[x, y] -= mob.CheckCollide; //UBER IMPORTANT sanity check for preventing destroyed mobs from hanging around in buckets!!
                        }
                    }
                }

                // Remove mob from old buckets
                for (int x = preleft; x <= preright; x++)
                {
                    for (int y = pretop; y <= prebottom; y++)
                    {
                        if (!firstRegister && IndexInBounds(x, y))
                        {
                            if (mob.IsDestroyed || x < left || x > right || y < top || y > bottom) //if bucket should no longer have the mob
                            {
                                MobBuckets[x, y] -= mob.CheckCollide;
                            }
                        }
                    }
                }

                if (!mob.IsDestroyed)
                {
                    LastCollisionRectangle[mob] = NextCollisionRectangle[mob];
                }
                else
                {
                    LastCollisionRectangle.Remove(mob);
                    NextCollisionRectangle.Remove(mob);
                }
            }


            private bool IndexInBounds(int x, int y)
            {
                return x >= 0 && x < MobBuckets.GetLength(0) && y >= 0 && y < MobBuckets.GetLength(1);
            }


            private int AlignToBucketEdge(int value)
            {
                float f = value;
                int r = 0;
                r = (int)(f / BucketSize);
                r = value - (r * BucketSize);

                //Shift up and left to account for off-screen buckets used as padding
                r -= BucketSize * BucketPadding;

                //Expand left or up to fill bucket
                if (r != 0)
                {
                    if (value < 0)
                    {
                        return value - (BucketSize + r); //subtracts remainder
                    }
                    return value - (r);
                }
                return value;
            }




            //Useful for debugging. Use in the immediate window with the following syntax:
            //
            //      ?VisualizeRectangleInBucketField(mob.CollisionRectangle),nq
            //
            private string VisualizeRectangleInBucketField(Rectangle rectangle)
            {
                bool[,] work = GetBucketsInRectangle(rectangle);
                string result = "  ";

                for (int x = 0; x < work.GetLength(0); x++)
                {
                    result += string.Format(" {0:00}", x);
                }

                for (int y = 0; y < work.GetLength(1); y++)
                {
                    result += string.Format("\r\n{0:00}", y);
                    for (int x = 0; x < work.GetLength(0); x++)
                    {
                        if (work[x, y])
                        {
                            result += "  x";
                        }
                        else
                        {
                            result += "  -";
                        }
                    }
                }

                return result;
            }


            //Useful for debugging
            private bool[,] GetBucketsInRectangle(Rectangle rectangle)
            {
                bool[,] result = new bool[MobBuckets.GetLength(0), MobBuckets.GetLength(1)];

                int left = AlignToBucketEdge(rectangle.Left - NextScreenRectangle.Left) / BucketSize;
                int top = AlignToBucketEdge(rectangle.Top - NextScreenRectangle.Top) / BucketSize;
                int right = AlignToBucketEdge(rectangle.Right - NextScreenRectangle.Left) / BucketSize;
                int bottom = AlignToBucketEdge(rectangle.Bottom - NextScreenRectangle.Top) / BucketSize;

                result = new bool[MobBuckets.GetLength(0), MobBuckets.GetLength(1)];
                for (int x = 0; x < MobBuckets.GetLength(0); x++)
                {
                    for (int y = 0; y < MobBuckets.GetLength(1); y++)
                    {
                        if (x >= left && x <= right && y >= top && y <= bottom)
                        {
                            result[x, y] = true;
                        }
                        else
                        {
                            result[x, y] = false;
                        }
                    }
                }

                return result;
            }


            //Useful for debugging
            private Dictionary<Type, List<Mob>> GetDistinctMobsByType()
            {
                Dictionary<Type, List<Mob>> distinctTypes = new Dictionary<Type, List<Mob>>();
                for (int x = 0; x < MobBuckets.GetLength(0); x++)
                {
                    for (int y = 0; y < MobBuckets.GetLength(1); y++)
                    {
                        if (MobBuckets[x, y] != null)
                        {
#if WINDOWS
                            foreach (Delegate collisionHandler in MobBuckets[x, y].GetInvocationList())
                            {
                                Type key = collisionHandler.Target.GetType();
                                if (!distinctTypes.ContainsKey(key))
                                {
                                    distinctTypes.Add(key, new List<Mob>());
                                }
                                if (!distinctTypes[key].Contains(collisionHandler.Target))
                                {
                                    distinctTypes[key].Add((Mob)collisionHandler.Target);
                                }
                            }
#endif
                        }
                    }
                }

                //if you ever want to look inside one of the lists by type, do something like this below
                //Mob test = distinctTypes.ToList()[0].Value[0];

                return distinctTypes;
            }


            //Useful for debugging
            private List<Vector2> GetBucketsForMob(Mob mob)
            {
                List<Vector2> resultList = new List<Vector2>();

                for (int x = 0; x < MobBuckets.GetLength(0); x++)
                {
                    for (int y = 0; y < MobBuckets.GetLength(1); y++)
                    {
                        //foreach (Mob mob2 in MobBuckets[x, y])
                        //{
                        //    if (mob2 == mob)
                        //    {
                        //        resultList.Add(new Vector2(x, y));
                        //    }
                        //}
                    }
                }

                return resultList;
            }

    }
}
