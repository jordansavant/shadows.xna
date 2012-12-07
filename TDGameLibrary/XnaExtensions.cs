using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary
{
    public static class XnaExtensions
    {
        /**
         * Dictionaries
         */
        public static void AddKeyIfMissing<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, default(TValue));
            }
        }

        public static bool ContainsKeyAndValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return dictionary.ContainsKey(key) && dictionary[key].Equals(value);
        }




        /**
         * Randoms
         */
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }


        /// <summary>
        /// Returns a random float between 0 and 1, clamped to min and max. Note: doesn't support Max greater than 1!
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float NextFloat(this Random random, float min, float max)
        {
            return MathHelper.Clamp((float)random.NextDouble(), min, max);
        }

        public static bool WithChance(this Random random, double lessThanValue)
        {
            return GameEnvironment.Random.NextDouble() <= lessThanValue;
        }



        /**
         * Rectangles
         */
        public static Vector2 RandomEdgeVector(this Rectangle rectangle)
        {
            switch (GameEnvironment.Random.Next(3))
            {
                case 0:
                    return new Vector2( GameEnvironment.Random.Next(rectangle.X, rectangle.Right), rectangle.Y);
                case 1:
                    return new Vector2(GameEnvironment.Random.Next(rectangle.X, rectangle.Right), rectangle.Bottom);
                case 2:
                    return new Vector2(rectangle.X, GameEnvironment.Random.Next(rectangle.Y, rectangle.Bottom));
                case 3:
                    return new Vector2(rectangle.Right, GameEnvironment.Random.Next(rectangle.Y, rectangle.Bottom));
            }

            throw new InvalidOperationException();
        }

        public static Rectangle? Intersection(this Rectangle self, Rectangle rectangle)
        {
            int width = Math.Min(self.Right, rectangle.Right) - Math.Max(self.Left, rectangle.Left);
            int height = Math.Min(self.Bottom, rectangle.Bottom) - Math.Max(self.Top, rectangle.Top);

            if (width == 0 || height == 0)
            {
                return null;
            }

            return new Rectangle
            {
                X = Math.Max(self.X, rectangle.X),
                Y = Math.Max(self.Y, rectangle.Y),
                Width = width,
                Height = height
            };
        }

        public static bool Intersects(this Rectangle rectangle, Vector2 startPoint, Vector2 endPoint)
        {
            int xMin = (int)Math.Min(startPoint.X, endPoint.X);
            int xMax = (int)Math.Max(startPoint.X, endPoint.X);
            bool xCross = (xMin <= rectangle.Left && xMax >= rectangle.Left) || (xMin <= rectangle.Right && xMax >= rectangle.Right) || (xMin >= rectangle.Left && xMax <= rectangle.Right);
            
            int yMin = (int)Math.Min(startPoint.Y, endPoint.Y);
            int yMax = (int)Math.Max(startPoint.Y, endPoint.Y);
            bool yCross = (yMin <= rectangle.Top && yMax > rectangle.Top) || (yMin <= rectangle.Bottom && yMax > rectangle.Bottom) || (yMin >= rectangle.Top && yMax <= rectangle.Bottom);

            return xCross && yCross;
        }

        public static Vector2 GetInteriorVectorByPercentage(this Rectangle rectangle, float percentWidth, float percentHeight)
        {
            return new Vector2(rectangle.Width * MathHelper.Clamp(percentWidth, 0f, 1f), rectangle.Height * MathHelper.Clamp(percentHeight, 0f, 1f));
        }

        public static Vector2 RandomInternalVector(this Rectangle rectangle)
        {
            return new Vector2(GameEnvironment.Random.Next(rectangle.X, rectangle.X + rectangle.Width), GameEnvironment.Random.Next(rectangle.Y, rectangle.Y + rectangle.Height));
        }




        /**
         * Vector 2s
         */
        //Determines whether the given point lies along (returns x = 0), left (returns x < 0), or right (returns x > 0) of a line evenly bisecting space.
        //The line dividing space is defined using the start and end points, and it is directed away from the start point and toward the end point.
        //
        // Example (P = test point, v1 = line segment start, v2 = line segment end):
        //
        //      [P: x<0]
        //                                                               [P: x<0]
        //   >>>>>>>>>>>> [v1] >>>>>>>> [v2] >>>>>>>> [P: x=0] >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //    [P: x>0]
        //                                 [P: x>0]
        //
        public static float GetWhichSideOfLine(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            return (lineEnd.X - lineStart.X) * (point.Y - lineStart.Y) - (lineEnd.Y - lineStart.Y) * (point.X - lineStart.X);
        }

        
        //Finds the angle (radians) between two vectors by considering two lines drawn from each vector to a shared pivot point.
        //Rotation is calculated clockwise starting at the first vector.
        //The end result x always satisfies 0 <= x < TwoPi.
        //Note: the length of each line drawn to the pivot matters unless the angle is an exact multiple of PiOver2 (90 degrees).
        //
        // Examples (P = pivot point, v1 = first vector, v2 = second vector):
        //
        //   |[v1]                                       \[v2]  =(rotated 1/3 left into quadrant 2)           \[v1]  =(rotated 1/3 left into quadrant
        //   |                                            \                                                    \
        //   |                                             \                                                    \
        //   |                                              \                                                    \
        //   | x=PiOver2=90degrees                           \ x=(5/3)Pi=240degrees                               \ x=(4/3)Pi=120degrees
        //   P---------------------------------[v2]           P-------------[v1]  =(along x axis)                  P-------------[v2]  =(along x axis)
        //
        public static float GetAngleBetweenVectors(this Vector2 point, Vector2 other, Vector2 pivot)
        {
            float angle = point.GetRotation(pivot) - other.GetRotation(pivot);
            return angle < 0 ? angle + MathHelper.TwoPi : angle;
        }

        
        public static Vector2 ScaleToLength(this Vector2 point, float length)
        {
            throw new NotImplementedException();
        }

        public static Vector2 ScaleToLength(this Vector2 point, float length, Vector2 pivot)
        {
            throw new NotImplementedException();
        }


        public static Vector2 Scale(this Vector2 point, float scaleFactor)
        {
            throw new NotImplementedException();
        }

        public static Vector2 Scale(this Vector2 point, float scaleFactor, Vector2 pivot)
        {
            throw new NotImplementedException();
        }


        //http ://stackoverflow.com/questions/3756920/whats-wrong-with-this-xna-rotatevector2-function
        public static Vector2 RotateVector(this Vector2 point, float radians)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            return new Vector2(
                point.X * cosRadians - point.Y * sinRadians,
                point.X * sinRadians + point.Y * cosRadians);
        }
        
        //http ://stackoverflow.com/questions/3756920/whats-wrong-with-this-xna-rotatevector2-function
        public static Vector2 RotateVector(this Vector2 point, float radians, Vector2 pivot)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            Vector2 translatedPoint = new Vector2();
            translatedPoint.X = point.X - pivot.X;
            translatedPoint.Y = point.Y - pivot.Y;

            Vector2 rotatedPoint = new Vector2();
            rotatedPoint.X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + pivot.X;
            rotatedPoint.Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + pivot.Y;

            return rotatedPoint;
        }

        public static Vector2 ToVector2(this Rectangle rectangle)
        {
            return new Vector2(rectangle.X, rectangle.Y);
        }

        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point((int)vector2.X, (int)vector2.Y);
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static float GetRotation(this Vector2 vector2)
        {
            if (vector2 != Vector2.Zero)
                vector2.Normalize();
            return (float)Math.Atan2(vector2.Y, vector2.X);
        }

        public static Vector2 ToVector2(this float rads)
        {
            return new Vector2((float)Math.Cos(rads - (float)Math.PI / 2), (float)Math.Sin(rads - (float)Math.PI / 2));
        }

        public static float GetRotation(this Vector2 vector2, Vector2 pivot)
        {
            return (float)Math.Atan2(vector2.Y - pivot.Y, vector2.X - pivot.X);
        }

        public static Color ToOpacity(this Color color, float Opacity)
        {
            Vector4 v = color.ToVector4();
            return new Color(v.X * Opacity, v.Y * Opacity, v.Z * Opacity, v.W * Opacity);
        }

    }
}
