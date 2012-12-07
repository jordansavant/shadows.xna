using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TDGameLibrary.Utility
{
    public class FrameRateCounter : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        float frameRate = 0.0f;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Color Color;

        public FrameRateCounter(Game game, SpriteFont sf, SpriteBatch sb)
            : base(game)
        {
            content = new ContentManager(game.Services);
            spriteFont = sf;
            spriteBatch = sb;

            DrawOrder = GameEnvironment.FrameRateCounterDrawOrder;
        }



        public override void Update(GameTime gameTime)
        {
#if DEBUG
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update(gameTime);
#endif
        }


        public override void Draw(GameTime gameTime)
        {
#if DEBUG
            frameCounter++;

            if (frameRate >= 60)
                Color = Color.Lime;
            else if (frameRate >= 55)
                Color = Color.Yellow;
            else if (frameRate >= 50)
                Color = Color.Orange;
            else
                Color = Color.Red;

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, frameRate.ToString(), new Vector2(9, 6), Color);
            spriteBatch.End();

            base.Draw(gameTime);
#endif
        }
    }
}