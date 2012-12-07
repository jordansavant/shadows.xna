using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Mobs;
using FarseerTools;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using TDGameLibrary.Animation;
using TDGameLibrary.Input;
using ShadowShootout.Input;
using Krypton.Lights;
using TDGameLibrary;
using Krypton;

namespace ShadowShootout
{
    class CircleBlockade : Mob
    {
        public CircleBlockade(float width, Vector2 positionInWorld)
            : base(GamePlayManager.MobManager, BuildBody(width))
        {
            SetPhysicalBodyPosition(ConvertUnits.ToSimUnits(positionInWorld));
            AnimatedSprite = new AnimatedSprite(@"Blank");
            Hull = BuildHull(width);
        }

        public ShadowHull Hull;

        public override void Update(GameTime gameTime)
        {
            Hull.Position = PositionOnScreen;

            base.Update(gameTime);
        }

        public static PhysicalBody BuildBody(float size)
        {
            Body body = BodyFactory.CreateCircle(GamePlayManager.WorldManager.World, ConvertUnits.ToSimUnits(size), 1);
            body.BodyType = BodyType.Static;
            return new FarseerPhysicalBody(body);
        }

        public static ShadowHull BuildHull(float size)
        {
            return ShadowHull.CreateCircle(size, 50);
        }
    }
}
