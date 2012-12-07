using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Components;
using TDGameLibrary.Mobs;
using FarseerPhysics.Dynamics;
using FarseerTools;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Weapons;

namespace TDGameLibrary.Utility
{
    public class MobInfoUtility : DrawableGameComponent
    {
        public MobInfoUtility(MobManager mobManager, SpriteFont spriteFont, SpriteBatch spriteBatch)
            : base(GameEnvironment.Game)
        {
            MobManager = mobManager;
            SpriteFont = spriteFont;
            SpriteBatch = spriteBatch;

            DrawOrder = GameEnvironment.MobInfoUtilityDrawOrder;
        }

        private SpriteFont SpriteFont;
        private SpriteBatch SpriteBatch;
        private Dictionary<Fixture, Texture2D> MobDebugTextures = new Dictionary<Fixture, Texture2D>();
        private Dictionary<Fixture, FarseerSprite> MobDebugSprites = new Dictionary<Fixture, FarseerSprite>();
        public MobManager MobManager { get; private set; }

        //TODO: add debug sprite pruning logic to prevent slow memory leak
        //  (debug sprites can currently get added to the list, but the list is never pruned)

        //TODO: may also want to draw debug sprites more closely to mob draw depth. For now just draw them on top of everything in the game.

        public override void Draw(GameTime gameTime)
        {
            if (GameEnvironment.IsDebugModeActive)
            {
                SpriteBatch.Begin();

                foreach (Mob mob in MobManager.Mobs)
                {
                    //foreach (Fixture fixture in mob.PhysicalBody.Body.FixtureList)
                    //{
                    //    if (!(mob is Projectile))
                    //    {
                    //        if (!MobDebugTextures.ContainsKey(fixture))
                    //        {
                    //            MobDebugTextures.Add(fixture, AssetCreator.Instance.TextureFromShape(fixture.Shape, MaterialType.Squares, Color.Orange, 1f));
                    //        }

                    //        if (!MobDebugSprites.ContainsKey(fixture))
                    //        {
                    //            MobDebugSprites.Add(fixture, new FarseerSprite(MobDebugTextures[fixture]));
                    //        }

                    //        Vector2 scale = new Vector2(ConvertUnits.ToSimUnits(fixture.Shape.Radius * 2), ConvertUnits.ToSimUnits(fixture.Shape.Radius * 2));

                    //        SpriteBatch.Draw(
                    //            MobDebugTextures[fixture], mob.PositionInWorld - mob.MobManager.WorldManager.ScreenPositionInWorld, null, Color.White,
                    //            fixture.Body.Rotation, MobDebugSprites[fixture].Origin, scale, SpriteEffects.None, 0f);
                    //    }
                    //}

                    // World Position
                    int roundToNearest = 10;
                    double worldPositionX = Math.Round(mob.PositionInWorldByUpdate.X / roundToNearest, 0) * roundToNearest;
                    double worldPositionY = Math.Round(mob.PositionInWorldByUpdate.Y / roundToNearest, 0) * roundToNearest;
                    string worldPositionText = string.Format("({0},{1} {2})", worldPositionX, worldPositionY, mob.ToString());

                    float worldPositionTextPositionX = mob.PositionInWorldByUpdate.X - mob.MobManager.WorldManager.ScreenPositionInWorld.X;
                    float worldPositionTextPositionY = (mob.PositionInWorldByUpdate - mob.MobManager.WorldManager.ScreenPositionInWorld).Y - SpriteFont.MeasureString(worldPositionText).Y;

                    SpriteBatch.DrawString(SpriteFont, worldPositionText, new Vector2(worldPositionTextPositionX, worldPositionTextPositionY),
                        Color.Yellow, 0f, Vector2.Zero, new Vector2(0.8f, 0.8f), SpriteEffects.None, 0f);

                    // Animated Sprite Details
                    roundToNearest = 1;
                    double spritePositionX = Math.Round(mob.AnimatedSprite.Position.X / roundToNearest, 0) * roundToNearest;
                    double spritePositionY = Math.Round(mob.AnimatedSprite.Position.Y / roundToNearest, 0) * roundToNearest;
                    string spritePositionText = string.Format("({0},{1})", spritePositionX, spritePositionY);

                    float spritePositionTextPositionX = mob.AnimatedSprite.Position.X;
                    float spritePositionTextPositionY = mob.AnimatedSprite.Position.Y + SpriteFont.MeasureString(spritePositionText).Y;

                    // Collision Box
                    SpriteBatch.DrawString(SpriteFont, spritePositionText, new Vector2(spritePositionTextPositionX, spritePositionTextPositionY),
                        Color.Aquamarine, 0f, Vector2.Zero, new Vector2(0.8f, 0.8f), SpriteEffects.None, 0f);
                    DrawUtility.DrawRectangle(mob.CollisionRectangleOnScreen, Color.PeachPuff);

                    DrawUtility.DrawRectangle(mob.HitBoxOnScreen, Color.OrangeRed);
                }

                SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

    }
}
