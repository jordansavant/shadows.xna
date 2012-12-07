using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Utility
{
    public class FpsUtility
    {
        public FpsUtility(Game game)
        {
            Game = game;
        }

        private Game Game;

        public float fps;
        private float updateInterval = 1.0f; // seconds
        private float timeSinceLastUpdate = 0.0f;
        private float frameCount = 0;

        private int numFpsUpdates = 0;
        private float sumFpsSnapshots = 0;
        private float elapsedTimeSinceFpsAvgCleared = 0;

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                numFpsUpdates = 0;
                sumFpsSnapshots = 0;
                elapsedTimeSinceFpsAvgCleared = 0;
            }
        }

        public void Draw(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCount++;
            timeSinceLastUpdate += elapsed;

            if (timeSinceLastUpdate >= updateInterval)
            {
                fps = frameCount / timeSinceLastUpdate;

                sumFpsSnapshots += fps;
                numFpsUpdates += 1;
                elapsedTimeSinceFpsAvgCleared += timeSinceLastUpdate;

                frameCount = 0;
                timeSinceLastUpdate = 0;
            }
            Game.Window.Title = "FPS: " + fps.ToString();
            Game.Window.Title += " - Average FPS since " + (int)elapsedTimeSinceFpsAvgCleared + "s ago: " + (sumFpsSnapshots / numFpsUpdates);
        }
    }
}
