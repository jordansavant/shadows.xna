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

namespace ShadowShootout
{
    class Player : Mob
    {
        public Player(Light2D light, Vector2 positionInWorld)
            : base(GamePlayManager.MobManager, BuildBody())
        {
            Light = light;

            SetPhysicalBodyPosition(ConvertUnits.ToSimUnits(positionInWorld));
            AnimatedSprite = new AnimatedSprite(@"Photon");
            Speed = 5;
        }

        public Light2D Light;

        public override void Update(GameTime gameTime)
        {
            Direction = InputManager.GetCurrentControlVector(VectorControl.Movement);

            Light.Position = PositionOnScreen;

            base.Update(gameTime);
        }

        public static PhysicalBody BuildBody()
        {
            Body body = BodyFactory.CreateCircle(GamePlayManager.WorldManager.World, ConvertUnits.ToSimUnits(10), 1);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = .2f;
            return new FarseerPhysicalBody(body);
        }
    }
}
