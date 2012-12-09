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
using Microsoft.Xna.Framework.Input;

namespace ShadowShootout
{
    class Player : Mob
    {
        public Player(Vector2 positionInWorld)
            : base(GamePlayManager.MobManager, BuildBody())
        {
            Light = new Light2D()
            {
                Texture = Game1.LightTexture,
                Range = 120,
                Color = Color.LightBlue,
                Intensity = .7f,
                Angle = MathHelper.TwoPi
            };

            SetPhysicalBodyPosition(ConvertUnits.ToSimUnits(positionInWorld));
            AnimatedSprite = new AnimatedSprite(@"Photon");
            Speed = 5;
        }

        public Light2D Light;

        public override void Update(GameTime gameTime)
        {
            Direction = InputManager.GetCurrentControlVector(VectorControl.Movement);

            Light.Position = PositionOnScreen;

            if (InputManager.IsButtonPressed(Button.ActivatePointLight))
            {
                Light.Fov = MathHelper.PiOver2 * (float)(0.5);
                Light.Color = Color.GhostWhite;
                Light.Range = 300f;
                Light.Intensity = .8f;
            }
            if (InputManager.IsButtonPressed(Button.ActivateSpotLight))
            {
                Light.Fov = (float)Math.PI * 2;
                Light.Color = Color.LightBlue;
                Light.Range = 120f;
                Light.Intensity = .7f;
            }

            Light.Angle = GetRotationFromVector(InputManager.GetCurrentControlVector(VectorControl.Aiming));

            base.Update(gameTime);
        }

        public static PhysicalBody BuildBody()
        {
            //return new GhostPhysicalBody(GamePlayManager.GhostBodyManager)
            //{
            //    Radius = ConvertUnits.ToSimUnits(10),
            //    Density = 1
            //};
            Body body = BodyFactory.CreateCircle(GamePlayManager.WorldManager.World, ConvertUnits.ToSimUnits(10), 1);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = .2f;
            return new FarseerPhysicalBody(body);
        }
    }
}
