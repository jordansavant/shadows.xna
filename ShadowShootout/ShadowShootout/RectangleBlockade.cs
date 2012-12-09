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
using Microsoft.Xna.Framework.Graphics;

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
            Texture = GameEnvironment.Game.Content.Load<Texture2D>(@"Black");
            Rectangle.Width = (int)width;
            Rectangle.Height = (int)height;
        }

        public ShadowHull Hull;
        private Texture2D Texture;
        private Rectangle Rectangle;

        public override void Update(GameTime gameTime)
        {
            Hull.Position = PositionOnScreen;
            Rectangle.X = (int)PositionOnScreen.X - Rectangle.Width /2;
            Rectangle.Y = (int)PositionOnScreen.Y - Rectangle.Height / 2;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float layerDepth)
        {
            spriteBatch.Draw(Texture, Rectangle, Color.White);

            base.Draw(gameTime, spriteBatch, layerDepth);
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
