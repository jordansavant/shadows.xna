using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Mobs;
using TDGameLibrary.Utility;

namespace TDGameLibrary.Map
{

    public enum CameraLockModes
    {
        Point,
        Pan,
        Mob
    }

    /**
     * The responsibility of this class is to translate the Screen Rectangle to a Map's coordinates and track those coordinates
     * if ever the Screen Rectangle needs to move across the map.
     **/
    public class MapCamera
    {
        public MapCamera(ChunkedMap map)
        {
            Map = map;

            Origin = new Vector2(0, 0);

            PanDirection = Vector2.Zero;

            PanSpeed = 1.0f;

            IsLockedOnMob = false;
        }

        protected CameraLockModes LockMode = CameraLockModes.Pan;
        protected bool IsLockedOnMob;
        protected Mob MobLock { get; set; }
        public ChunkedMap Map { get; set; }
        public Vector2 Origin;
        public Vector2 Center;
        public Vector2 PanDirection { get; set; }
        public float PanSpeed { get; set; }
        public Vector2 TickOffset;
        public Vector2 OriginReset;
        protected Vector2 LockPoint;

        public virtual void Update(GameTime gameTime)
        {
            if (Origin != OriginReset)
            {
                Origin = OriginReset;
            }

            switch (LockMode)
            {
                case CameraLockModes.Mob:
                    UpdateMobLock();
                    break;
                case CameraLockModes.Pan:
                    if (PanDirection != Vector2.Zero)
                    {
                        Vector2 direction = PanDirection;
                        direction.Normalize();
                        PanDirection = direction;
                    }
                    Origin += (PanDirection * PanSpeed);
                    PanDirection = Vector2.Zero;
                    break;

                case CameraLockModes.Point:
                    Origin = LockPoint;
                    break;
            }

            Center.X = Origin.X + (GameEnvironment.ScreenRectangle.Width / 2);
            Center.Y = Origin.Y + (GameEnvironment.ScreenRectangle.Height / 2);

            if (TickOffset != Vector2.Zero)
            {
                OriginReset = Origin;
                Origin += TickOffset;
            }
        }

        public virtual void LockOnMob(Mob mob)
        {
            MobLock = mob;
            LockMode = CameraLockModes.Mob;
        }

        public virtual void LockOnPoint(Vector2 point)
        {
            LockPoint.X = point.X - (GameEnvironment.ScreenRectangle.Width / 2);
            LockPoint.Y = point.Y - (GameEnvironment.ScreenRectangle.Height / 2);
            LockMode = CameraLockModes.Point;
        }

        public virtual void UpdateMobLock()
        {
            Origin.X = MobLock.PositionInWorldByUpdate.X - (GameEnvironment.ScreenRectangle.Width / 2);
            Origin.Y = MobLock.PositionInWorldByUpdate.Y - (GameEnvironment.ScreenRectangle.Height / 2);
        }

        public Vector2 WorldToScreenPosition(Vector2 worldPosition)
        {
            return worldPosition - Origin; 
        }
    }
}
