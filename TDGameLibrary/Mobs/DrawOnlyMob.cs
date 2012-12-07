using Microsoft.Xna.Framework;
using TDGameLibrary.Mobs;
using TDGameLibrary.Weapons;
using FarseerPhysics.Dynamics;
using TDGameLibrary.Animation;
using TDGameLibrary;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Components;
using System;
using FarseerTools;

namespace TDGameLibrary.Mobs
{
    public class DrawOnlyMob : Mob
    {
        public DrawOnlyMob(MobManager mobManager, Vector2 positionInWorld, float parallaxFactor, int worldLayer)
            : base(mobManager, null)
        {
            PositionInWorldByUpdate = positionInWorld;
            SetWorldPosition(ref positionInWorld);
            ParallaxFactor = parallaxFactor;
            SetWorldLayer(worldLayer);
        }

        public float ParallaxFactor;
        public override Vector2 PositionInWorld { get { return PositionInWorldByUpdate; } }
        public override Vector2 PositionOnScreen
        {
            get
            {
                return (PositionInWorld - MobManager.WorldManager.ScreenPositionInWorld) * ParallaxFactor;
            }
        }


        public override void Update(GameTime gameTime)
        {
            PositionInWorldByUpdate = PositionInWorld;
            PositionOnScreenByUpdate = PositionOnScreen;

            int width = (int)((float)AnimatedSprite.Width * AnimatedSprite.Scale);
            int height = (int)((float)AnimatedSprite.Width * AnimatedSprite.Scale);

            CollisionRectangle.X = (int)PositionInWorld.X - width / 2;
            CollisionRectangle.Y = (int)PositionInWorld.Y - height / 2;
            CollisionRectangle.Width = width;
            CollisionRectangle.Height = height;

            HitBox = CollisionRectangle;
        }

        public void SetWorldPosition(ref Vector2 position)
        {
            PositionInWorldByUpdate.X = position.X;
            PositionInWorldByUpdate.Y = position.Y;
        }

        public override void Destruct()
        {
            if (!IsDestroyed)
            {
                IsDestroyed = true;
                MobManager.UnregisterMob(this);
                DestructEvent(EventArgs.Empty);
            }
        }
    }
}
