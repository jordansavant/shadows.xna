using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Utility;

namespace TDGameLibrary.Animation
{
    public enum AnimationEnum
    {
        Default
    }

    public class AnimatedSprite
    {
        private AnimatedSprite()
        {
            Color = Color.White;
            Animations = new Dictionary<Enum, Animation>();
            SpriteEffects = SpriteEffects.None;
            Scale = 1f;
            IsVisible = true;
            Origin = new Vector2(9999, 9999); //always start sprites offscreen so they don't flicker when created
        }


        public AnimatedSprite(Texture2D sprite, Animation animation)
            : this()
        {
            Texture = sprite;
            Animations.Add(AnimationEnum.Default, animation);
            CurrentAnimation = AnimationEnum.Default;
        }

        public AnimatedSprite(Texture2D sprite)
            : this(sprite, new Animation(1, sprite.Width, sprite.Height, 0, 0, 1, true))
        {
        }

        public AnimatedSprite(Texture2D sprite, Dictionary<Enum, Animation> animations)
            : this()
        {
            CurrentAnimation = animations.Keys.FirstOrDefault();
            Texture = sprite;
            Animations = animations;
        }

        public AnimatedSprite(String spritePath)
            : this(GameEnvironment.Game.Content.Load<Texture2D>(spritePath))
        {
        }

        public AnimatedSprite(String spritePath, Dictionary<Enum, Animation> animations)
            : this(GameEnvironment.Game.Content.Load<Texture2D>(spritePath), animations)
        {
        }

        public AnimatedSprite(String spritePath, Animation animation)
            : this(GameEnvironment.Game.Content.Load<Texture2D>(spritePath), animation)
        {
        }



        public Dictionary<Enum, Animation> Animations;
        private Enum currentAnimation;
        public Texture2D Texture { get; private set; }
        public float Rotation = 0f;
        public SpriteEffects SpriteEffects;
        public Vector2 RelativeOrigin = new Vector2(.5f, .5f);
        public float Scale;
        public Vector2 Origin;
        
        public Enum CurrentAnimation
        {
            get { return currentAnimation; }
            set
            {
                LastAnimation = currentAnimation;
                currentAnimation = value;

                if (LastAnimation == null)
                    LastAnimation = value;
            }
        }

        public Enum LastAnimation;
        public Enum DefinedLastAnimation;
        public bool IsVisible;
        public Rectangle DrawRectangle; // Do not edit anywhere.
        public Color Color;
        public Color TickColor;
        public int TickColorCount = 3;

        public int Width
        {
            get { return Animations[currentAnimation].FrameWidth; }
        }

        public int Height
        {
            get { return Animations[currentAnimation].FrameHeight; }
        }

        private float speed = 1.0f;
        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(value, 0.1f, 16.0f); }
        }

        public Vector2 Position;
        public Vector2 PositionOffset = Vector2.Zero; // Used to adjust the draw position for a single update
        public bool IsFrozen = false;

        private Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (velocity != Vector2.Zero)
                    velocity.Normalize();
            }
        }


        public void Update(GameTime gameTime)
        {
            // Update Rectangle
            DrawRectangle.Width = (int)((float)Width * Scale * 2); // x2 for rotation on sprites
            DrawRectangle.Height = (int)((float)Height * Scale * 2);
            DrawRectangle.X = (int)Position.X - DrawRectangle.Width / 2;
            DrawRectangle.Y = (int)Position.Y - DrawRectangle.Height / 2;

            if (IsVisible && !IsFrozen)
            {
               Animations[currentAnimation].Update(gameTime);
            }
        }

        int tickColorCounter = 0;
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float layerDepth)
        {
            // Update Rectangle
            DrawRectangle.Width = (int)((float)Width * Scale * 2); // x2 for rotation on sprites
            DrawRectangle.Height = (int)((float)Height * Scale * 2);
            DrawRectangle.X = (int)Position.X - DrawRectangle.Width / 2;
            DrawRectangle.Y = (int)Position.Y - DrawRectangle.Height / 2;

            if (IsVisible)
            {
                if (DrawRectangle.Intersects(GameEnvironment.ScreenRectangle))
                {
                    // Moved here from update because we had sync issues.
                    Position += PositionOffset;
                    Origin.X = RelativeOrigin.X * Animations[currentAnimation].CurrentFrameRect.Width;
                    Origin.Y = RelativeOrigin.Y * Animations[currentAnimation].CurrentFrameRect.Height;
                    // --

                    spriteBatch.Draw(
                        Texture,
                        Position,
                        Animations[currentAnimation].CurrentFrameRect,
                        (TickColor != Color.White ? TickColor : Color),
                        Rotation,
                        Origin,
                        Scale,
                        SpriteEffects,
                        layerDepth);

                    if (TickColor != Color.White)
                    {
                        tickColorCounter++;
                    }

                    if(tickColorCounter >= TickColorCount)
                    {
                        TickColor = Color.White;
                        tickColorCounter = 0;
                    }
                }
            }
        }

        public void LockToMap(Rectangle ScreenRectangle)
        {
            Position.X = MathHelper.Clamp(Position.X, 0, ScreenRectangle.Width - Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, ScreenRectangle.Height - Height);
        }

        public void ResetAnimation(Enum key)
        {
            Animations[key].Reset();
        }


        public void Reset()
        {
            Color = Color.White;
            Scale = 1f;
            Rotation = 0f;
            PositionOffset = Vector2.Zero;
            IsFrozen = false;
            IsVisible = true;
        }

        public void Sync(AnimatedSprite sprite)
        {
            Rotation = sprite.Rotation;
            Scale = sprite.Scale;
            Origin = sprite.Origin;
            Position = sprite.Position;
        }


        public static AnimatedSprite CreateEmptySprite()
        {
            return new AnimatedSprite(DrawUtility.GetEmptyTexture());
        }
    }
}