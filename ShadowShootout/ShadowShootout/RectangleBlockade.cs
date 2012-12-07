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
    class RectangleBlockade : Mob
    {
        public RectangleBlockade(float width, float height, Vector2 positionInWorld)
            : base(GamePlayManager.MobManager, BuildBody(width, height))
        {
            SetPhysicalBodyPosition(ConvertUnits.ToSimUnits(positionInWorld));
            AnimatedSprite = new AnimatedSprite(@"Blank");
            Hull = BuildHull(width, height);
        }

        public ShadowHull Hull;

        public override void Update(GameTime gameTime)
        {
            Hull.Position = PositionOnScreen;

            base.Update(gameTime);
        }

        public static PhysicalBody BuildBody(float width, float height)
        {
            Body body = BodyFactory.CreateRectangle(GamePlayManager.WorldManager.World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1);
            body.BodyType = BodyType.Static;
            return new FarseerPhysicalBody(body);
        }

        public static ShadowHull BuildHull(float width, float height)
        {
            return ShadowHull.CreateRectangle(new Vector2(width, height));
        }
    }
}
