using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGameLibrary.Animation
{
    public class Animation
    {
        private Rectangle[] Frames;
        private TimeSpan FrameLength;
        private TimeSpan FrameTimer;
        private int minFps;
        private int maxFps;
        public int FrameWidth;
        public int FrameHeight;
        public int FramesPerSecond;
        public int CurrentFrame;

        public Rectangle CurrentFrameRect
        {
            get { return Frames[CurrentFrame]; }
        }


        public Animation(int frameCount, int frameWidth, int frameHeight, int xOffset, int yOffset, int fps, Boolean horizontal)
        {
            minFps = 1;
            maxFps = 60;
            Frames = new Rectangle[frameCount];
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FramesPerSecond = fps;

            if (FramesPerSecond < 1)
                FramesPerSecond = 1;
            else if (FramesPerSecond > 60)
                FramesPerSecond = 60;
            else
                FramesPerSecond = (int)MathHelper.Clamp(FramesPerSecond, minFps, maxFps);

            FrameLength = TimeSpan.FromSeconds(1 / (double)FramesPerSecond);


            for (int i = 0; i < frameCount; i++)
            {
                if (horizontal)
                {
                    Frames[i] = new Rectangle(
                            xOffset + (frameWidth * i),
                            yOffset,
                            frameWidth,
                            frameHeight);
                }
                else
                {
                    Frames[i] = new Rectangle(
                            xOffset,
                            yOffset + (frameHeight * i),
                            frameWidth,
                            frameHeight);
                }
            }
            CurrentFrame = 0;
            FrameTimer = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            FrameTimer += gameTime.ElapsedGameTime;

            if (FrameTimer >= FrameLength)
            {
                FrameTimer = TimeSpan.Zero;
                CurrentFrame = (CurrentFrame + 1) % Frames.Length;
            }
        }

        public void SetFPS(int fps)
        {
            FramesPerSecond = fps;
        }

        public void Reset()
        {
            CurrentFrame = 0;
        }
    }
}
