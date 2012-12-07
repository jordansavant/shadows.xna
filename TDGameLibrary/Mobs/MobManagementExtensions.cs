using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Components;

namespace TDGameLibrary.Mobs
{
    public static class MobManagementExtensions
    {
        public static List<Mob> GetMobsInRectangle(this MobManager mobManager, Rectangle rectangle)
        {
            List<Mob> workingMobList = new List<Mob>();
            foreach (Mob mob in mobManager.Mobs)
            {
                if (rectangle.Intersects(mob.CollisionRectangle))
                {
                    workingMobList.Add(mob);
                }
            }

            return workingMobList;
        }


        public static List<Mob> GetMobsInCircle(this MobManager mobManager, Vector2 mapCenterCoordinates, float pixelRadius)
        {
            List<Mob> workingMobList = new List<Mob>();
            foreach (Mob mob in mobManager.Mobs)
            {
                if (Vector2.Distance(mob.PositionInWorldByUpdate, mapCenterCoordinates) < pixelRadius)
                {
                    workingMobList.Add(mob);
                }
            }

            return workingMobList;
        }


        public static Mob FindRandomMob(this MobManager mobManager, List<Mob> exludedMobs)
        {
            return mobManager.FindRandomMobOfType<Mob>(exludedMobs);
        }


        public static T FindRandomMobOfType<T>(this MobManager mobManager, List<Mob> exludedMobs)
            where T : Mob
        {
            List<Mob> mobsFound = new List<Mob>();

            foreach (Mob mob in mobManager.Mobs)
            {
                if (!exludedMobs.Contains(mob) && mob is T)
                {
                    mobsFound.Add(mob);
                }
            }

            if (mobsFound.Any())
            {
                return (T)mobsFound[GameEnvironment.Random.Next(mobsFound.Count)];
            }

            return null;
        }
    }
}
