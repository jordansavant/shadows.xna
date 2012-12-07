using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDGameLibrary.Components;
using TDGameLibrary.Map;

namespace TDGameLibrary.Utility
{
    public class DrawUtility : TdglComponent
    {
        public DrawUtility(SpriteBatch spriteBatch)
        {
            FpsUtility = new FpsUtility(GameEnvironment.Game);
            Texture = GetEmptyTexture();
            SpriteBatch = spriteBatch;
        }

        public Texture2D Texture;
        private SpriteBatch SpriteBatch;
        public FpsUtility FpsUtility;

        private static int PointCount;
        private static Dictionary<int, Vector2> Points = new Dictionary<int, Vector2>();
        private static Dictionary<int, Color> PointColors = new Dictionary<int, Color>();
        private static Dictionary<int, int> PointWidths = new Dictionary<int, int>();

        private static int LineCount;
        private static Dictionary<int, Vector2[]> LinePoints = new Dictionary<int, Vector2[]>();
        private static Dictionary<int, Color> LineColors = new Dictionary<int, Color>();
        private static Dictionary<int, int> LineWidths = new Dictionary<int, int>();

        private static List<WorldText> WorldTexts = new List<WorldText>();
        public static bool ShowFrameRate;


        public override void Update(GameTime gameTime)
        {
            FpsUtility.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // FRAME RATE
            if (ShowFrameRate)
            {
                FpsUtility.Draw(gameTime);
            }

            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null,
                null,
                null,
                Matrix.Identity);

            // POINTS
            int p = 0;
            foreach (Vector2 point in Points.Values)
            {
                SpriteBatch.Draw(Texture, point, null, PointColors[p],
                           0, Vector2.Zero, new Vector2(PointWidths[p], PointWidths[p]),
                           SpriteEffects.None, 0);
                p++;
            }

            // LINES
            int c = 0;
            foreach (Vector2[] pointPair in LinePoints.Values)
            {
                DrawLine(SpriteBatch, pointPair[0], pointPair[1], Texture, LineColors[c], LineWidths[c], 1, 0);
                c++;
            }

            // WORLD TEXTS
            foreach (WorldText worldText in WorldTexts.ToList())
            {
                if (worldText.TimeSince <= worldText.Duration)
                {
                    worldText.TimeSince += gameTime.ElapsedGameTime;

                    if (worldText.DrawShadow)
                        SpriteBatch.DrawString(worldText.SpriteFont, worldText.Text, worldText.Position + new Vector2(1, 1), Color.Black);
                    SpriteBatch.DrawString(worldText.SpriteFont, worldText.Text, worldText.Position, worldText.Color);

                    worldText.PositionInWorld += worldText.Trail;
                }
                else
                {
                    WorldTexts.Remove(worldText);
                }
            }
            

            SpriteBatch.End();

            if (Points.Any())
                Points.Clear();
            if (PointColors.Any())
                PointColors.Clear();
            if (PointWidths.Any())
                PointWidths.Clear();
            PointCount = 0;

            if (LinePoints.Any())
                LinePoints.Clear();
            if (LineColors.Any())
                LineColors.Clear();
            if (LineWidths.Any())
                LineWidths.Clear();
            LineCount = 0;

            base.Draw(gameTime);
        }






        public static void DrawPoints(List<Vector2> list)
        {
            foreach(Vector2 v in list)
            {
                DrawPoint(v);
            }
        }

        public static void DrawPoint(Vector2 point)
        {
            DrawPoint(point, Color.White, 1);
        }

        public static void DrawPoint(Vector2 point, Color color, int width)
        {
            Points[PointCount] = point;
            PointColors[PointCount] = color;
            PointWidths[PointCount] = width;

            PointCount++;
        }





        public static void DrawLine(Vector2 start, Vector2 end)
        {
            DrawLine(start, end, Color.White);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            DrawLine(start, end, color, 1);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color, int width)
        {
            LinePoints[LineCount] = new Vector2[2] { start, end };
            LineColors[LineCount] = color;
            LineWidths[LineCount] = width;
            LineCount++;
        }





        public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Texture2D texture, Color color, int width, float percentage, float drawDepth)
        {
            int distance = (int)(Vector2.Distance(start, end) * percentage);
            distance++;

            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            sb.Draw(texture, new Rectangle((int)start.X, (int)start.Y, distance, width),
                    new Rectangle(0, 0, (int)(texture.Width * percentage), texture.Height), color, angle, new Vector2(0, texture.Height * 0.5f), SpriteEffects.None, drawDepth);
        }


        public void DrawLine(Vector2 start, Vector2 end, Color color, int width, SpriteBatch sb, float drawDepth)
        {
            DrawLine(sb, start, end, Texture, color, width, 1, drawDepth);
        }


        public void DrawPoint(SpriteBatch sb, Vector2 point, Texture2D texture, Color color, int width, float drawDepth)
        {
            sb.Draw(texture, point, null, color,
                           0, Vector2.Zero, new Vector2(width, width),
                           SpriteEffects.None, 0);
        }

        public void DrawPoint(SpriteBatch sb, Vector2 point, Color color, int width, float drawDepth)
        {
            DrawPoint(sb, point, Texture, color, width, drawDepth);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            DrawRectangle(rectangle, color, 1);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color, int lineWidth)
        {
            DrawUtility.DrawLine(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X, rectangle.Y + rectangle.Height), color, lineWidth);
            DrawUtility.DrawLine(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y), color, lineWidth);
            DrawUtility.DrawLine(new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color, lineWidth);
            DrawUtility.DrawLine(new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color, lineWidth);
        }



        public static void DrawWorldText(MapCamera mapCamera, Vector2 positionInWorld, string font, Color color, string text, TimeSpan duration, Vector2 trail, bool drawShadow)
        {
            WorldTexts.Add(new WorldText(mapCamera, positionInWorld, GameEnvironment.Game.Content.Load<SpriteFont>(font), color, text, duration, trail, drawShadow));
        }

        public static void DrawScreenText(Vector2 positionOnScreen, string font, Color color, string text, TimeSpan duration, Vector2 trail, bool drawShadow)
        {
            WorldTexts.Add(new WorldText(null, positionOnScreen, GameEnvironment.Game.Content.Load<SpriteFont>(font), color, text, duration, trail, drawShadow));
        }



        public static Texture2D EmptyTexture = GetEmptyTexture();
        public static Texture2D GetEmptyTexture()
        {
            Texture2D texture = new Texture2D(TDGameLibrary.GameEnvironment.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData(new[] { Color.White });

            return texture;
        }








        private class WorldText
        {
            public WorldText(MapCamera mapCamera, Vector2 positionInWorld, SpriteFont spriteFont, Color color, string text, TimeSpan duration, Vector2 trail, bool drawShadow)
            {
                MapCamera = mapCamera;
                PositionInWorld = positionInWorld;
                SpriteFont = spriteFont;
                Color = color;
                Text = text;
                Duration = duration;
                TimeSince = TimeSpan.FromSeconds(0);
                Trail = trail;
                DrawShadow = drawShadow;
            }

            public MapCamera MapCamera;
            public Vector2 PositionInWorld;
            public SpriteFont SpriteFont;
            public Color Color;
            public string Text;
            public TimeSpan Duration, TimeSince;
            public Vector2 Trail;
            public bool DrawShadow;
            public Vector2 Position
            {
                get
                {
                    if(MapCamera != null)
                        return MapCamera.WorldToScreenPosition(PositionInWorld);
                    return PositionInWorld;
                }
            }
        }
    }

}
