using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Utility;
using TDGameLibrary.Animation;

namespace TDGameLibrary.Ui
{
    /**
     * A UiContainer's responsibility is positioning controls, and applying actions across its child controls and control managers
     */
    public class UiContainer
    {
        public UiContainer(Rectangle displayRectangle, UiControlManager controlManager)
        {
            DisplayRectangle = displayRectangle;
            UiControls = new List<UiControl>();
            UiControlPositions = new Dictionary<UiControl, Vector2>();
            ChildContainers = new List<UiContainer>();
            ChildContainerPositions = new Dictionary<UiContainer, Vector2>();

            // each container has its own control manager
            if (controlManager == null)
            {
                ControlManager = new UiControlManager();
            }
            else
            {
                ControlManager = controlManager;
            }

            IsEnabled = true;
            IsVisible = true;
            Opacity = 1f;
            BackgroundColor = Color.White;

            FadeInTime = TimeSpan.FromSeconds(0);
            FadeOutTime = TimeSpan.FromSeconds(0);
            CurrentFade = TimeSpan.FromSeconds(0);

        }


        protected UiContainer Parent;
        public UiControlManager ControlManager;
        protected Vector2 _displayPosition;
        public Vector2 DisplayPosition
        {
            get
            {
                return _displayPosition;
            }
            set
            {
                _displayPosition = value;
                _displayRectangle.X = (int)_displayPosition.X;
                _displayRectangle.Y = (int)_displayPosition.Y;
            }
        }
        protected Rectangle _displayRectangle;
        public Rectangle DisplayRectangle
        {
            get
            {
                return _displayRectangle;
            }
            set
            {
                _displayRectangle = value;
                _displayPosition.X = _displayRectangle.X;
                _displayPosition.Y = _displayRectangle.Y;
            }
        }
        public List<UiControl> UiControls { get; set; }
        protected Dictionary<UiControl, Vector2> UiControlPositions { get; set; }
        public List<UiContainer> ChildContainers { get; set; }
        public Dictionary<UiContainer, Vector2> ChildContainerPositions { get; set; }
        protected bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;

                if (ControlManager != null)
                {
                    ControlManager.IsEnabled = value;
                }
            }
        }
        protected bool isVisible;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;

                if (ControlManager != null)
                {
                    ControlManager.IsVisible = value;
                }

                foreach (UiContainer childContainer in ChildContainers)
                {
                    if(!childContainer.IndependentlyManaged)
                        childContainer.IsVisible = value;
                }
            }
        }
        public Texture2D BackgroundTexture;
        public Rectangle BackgroundTextureDestinationRectangle;
        public Color BackgroundColor;
        public float Opacity;
        public TimeSpan FadeInTime;
        public TimeSpan FadeOutTime;
        private TimeSpan CurrentFade;
        private bool FadeIn, FadeOut;
        public event EventHandler OnShow;
        public event EventHandler OnHide;
        public bool IndependentlyManaged;

        // Support percentage positioning
        public void AddChildControlAtControl(UiControl newControl, UiControl existingControl)
        {
            AddChildControlAtControl(newControl, existingControl, 0, 0, 0, 0, Vector2.Zero);
        }
        public void AddChildControlAtControl(UiControl newControl, UiControl existingControl, float childX, float atParentX, float childY, float atParentY)
        {
            AddChildControlAtControl(newControl, existingControl, childX, atParentX, childY, atParentY, Vector2.Zero);
        }
        public void AddChildControlAtControl(UiControl newControl, UiControl existingControl, float childX, float atParentX, float childY, float atParentY, Vector2 offset)
        {
            if (!UiControls.Contains(newControl))
                UiControls.Add(newControl);

            if (!UiControlPositions.ContainsKey(newControl))
            {
                Vector2 position = new Vector2();
                position.X = ToRelativeX(newControl.DisplayRectangle, existingControl.DisplayRectangle, childX, atParentX) + offset.X;
                position.Y = ToRelativeY(newControl.DisplayRectangle, existingControl.DisplayRectangle, childY, atParentY) + offset.Y;

                UiControlPositions.Add(newControl, position + UiControlPositions[existingControl]);

                newControl.Position = existingControl.Position + position;
            }

            ControlManager.Add(newControl);
        }

        public void AddChildControl(UiControl control, float childX, float atParentX, float childY, float atParentY, Vector2 offset)
        {
            AddChildControl(control, ToRelativePositionAboutContainer(control.DisplayRectangle, childX, atParentX, childY, atParentY) + offset);
        }
        public void AddChildControl(UiControl control, float childX, float atParentX, float childY, float atParentY)
        {
            AddChildControl(control, ToRelativePositionAboutContainer(control.DisplayRectangle, childX, atParentX, childY, atParentY));
        }

        // Support pixel positioning
        public virtual void AddChildControl(UiControl control)
        {
            AddChildControl(control, Vector2.Zero);
        }

        public void AddChildControl(UiControl control, Vector2 position)
        {
            if (!UiControls.Contains(control))
                UiControls.Add(control);

            if (!UiControlPositions.ContainsKey(control))
            {
                UiControlPositions.Add(control, position);

                Vector2 controlPosition = control.Position;
                controlPosition.X = DisplayRectangle.X + position.X;
                controlPosition.Y = DisplayRectangle.Y + position.Y;
                control.Position = controlPosition;
            }

            ControlManager.Add(control);
        }





        public void OffsetChildControlPosition(UiControl control, Vector2 position)
        {
            if (UiControlPositions.ContainsKey(control))
            {
                UiControlPositions[control] += position;
            }
        }

        public void UpdateChildControlPosition(UiControl control, Vector2 position)
        {
            if (UiControlPositions.ContainsKey(control))
            {
                UiControlPositions[control] = position;

                Vector2 controlPosition = control.Position;
                controlPosition.X = DisplayRectangle.X + UiControlPositions[control].X;
                controlPosition.Y = DisplayRectangle.Y + UiControlPositions[control].Y;
                control.Position = controlPosition;
            }
        }




        // Realignment helpers for text control
        public void RealignLabelCenter(UiLabel label, string newText)
        {
            int preWidth = (int)label.TextSize.X;
            label.Text = newText;
            int postWidth = (int)label.TextSize.X;
            if (preWidth != postWidth)
                OffsetChildControlPosition(label, new Vector2(preWidth / 2 - postWidth / 2, 0)); // keep center
        }
        public void RealignLabelRight(UiLabel label, string newText)
        {
            int preWidth = (int)label.TextSize.X;
            label.Text = newText;
            int postWidth = (int)label.TextSize.X;
            if (preWidth != postWidth)
                OffsetChildControlPosition(label, new Vector2(preWidth - postWidth, 0)); // keep center
        }




        // Support percentage positioning
        public void AddChildContainer(UiContainer container, float childX, float atParentX, float childY, float atParentY, Vector2 offset)
        {
            AddChildContainer(container, ToRelativePositionAboutContainer(container.DisplayRectangle, childX, atParentX, childY, atParentY) + offset);
        }
        public void AddChildContainer(UiContainer container, float childX, float atParentX, float childY, float atParentY)
        {
            AddChildContainer(container, ToRelativePositionAboutContainer(container.DisplayRectangle, childX, atParentX, childY, atParentY));
        }

        // Support pixel positioning
        public virtual void AddChildContainer(UiContainer container)
        {
            AddChildContainer(container, new Vector2(container.DisplayRectangle.X, container.DisplayRectangle.Y));
        }
        public void AddChildContainer(UiContainer container, Vector2 relativePosition)
        {
            container.Parent = this;
            if (!ChildContainerPositions.ContainsKey(container))
                ChildContainerPositions.Add(container, relativePosition);

            if (!ChildContainers.Contains(container))
                ChildContainers.Add(container);
        }

        public void OffsetChildContainerPosition(UiContainer container, Vector2 position)
        {
            if (ChildContainerPositions.ContainsKey(container))
            {
                ChildContainerPositions[container] += position;
            }
        }

        public void UpdateChildContainerPosition(UiContainer container, Vector2 position)
        {
            if (ChildContainerPositions.ContainsKey(container))
            {
                ChildContainerPositions[container] = position;
            }
        }


        public virtual void Show()
        {
            IsEnabled = true;
            FadeIn = true;
            FadeOut = false;

            if (OnShow != null)
            {
                OnShow(this, EventArgs.Empty);
            }
        }

        public virtual void Hide()
        {
            IsEnabled = false;
            FadeOut = true;
            FadeIn = false;

            if (OnHide != null)
            {
                OnHide(this, EventArgs.Empty);
            }
        }




        public virtual void Update(GameTime gameTime)
        {
            ControlManager.IsEnabled = IsEnabled;
            ControlManager.Update(gameTime);

            // Fade Effects
            if (FadeIn)
            {
                IsVisible = true;
                CurrentFade += gameTime.ElapsedGameTime;
                Opacity = MathHelper.Clamp((float)(CurrentFade.TotalMilliseconds / FadeInTime.TotalMilliseconds), 0f, 1f);

                if (Opacity == 1f)
                {
                    FadeIn = false;
                    CurrentFade = TimeSpan.FromSeconds(0);
                }
            }

            if (FadeOut && Opacity > 0)
            {
                CurrentFade += gameTime.ElapsedGameTime;
                Opacity = 1 - MathHelper.Clamp((float)(CurrentFade.TotalMilliseconds / FadeOutTime.TotalMilliseconds), 0f, 1f);

                if (Opacity == 0f)
                {
                    FadeOut = false;
                    CurrentFade = TimeSpan.FromSeconds(0);
                    IsVisible = false;
                }
            }

            foreach (UiControl control in UiControls)
            {
                Vector2 controlPosition = control.Position;

                controlPosition.X = DisplayRectangle.X + UiControlPositions[control].X;
                controlPosition.Y = DisplayRectangle.Y + UiControlPositions[control].Y;
                control.Position = controlPosition;

                control.Opacity = Opacity;
                control.IsEnabled = IsEnabled;
                control.IsVisible = IsVisible;

                // Control Manager calls update on controls already
            }

            foreach (UiContainer childContainer in ChildContainers)
            {
                Vector2 containerPosition = childContainer.DisplayPosition;
                containerPosition.X = DisplayRectangle.X + ChildContainerPositions[childContainer].X;
                containerPosition.Y = DisplayRectangle.Y + ChildContainerPositions[childContainer].Y;
                childContainer.DisplayPosition = containerPosition;

                childContainer.Update(gameTime);
            }
        }


        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible && TDGameLibrary.GameEnvironment.IsDebugModeActive)
                DrawUtility.DrawRectangle(DisplayRectangle, Color.Turquoise);

            if (BackgroundTexture != null && IsVisible)
            {
                Rectangle destination = (BackgroundTextureDestinationRectangle.Width == 0 ? DisplayRectangle : BackgroundTextureDestinationRectangle);
                Vector4 v = BackgroundColor.ToVector4();
                spriteBatch.Draw(BackgroundTexture, destination, BackgroundColor.ToOpacity(v.W * Opacity));
            }

            ControlManager.Draw(spriteBatch);

            // Control Manager calls Draw on controls already

            foreach (UiContainer childContainer in ChildContainers)
            {
                childContainer.Draw(gameTime, spriteBatch);
            }
        }


        public void SubscribeFunctionToControlSelectEvents(EventHandler function)
        {
            foreach (UiControl control in UiControls)
            {
                control.OnSelect += function;
            }

            foreach (UiContainer childContainer in ChildContainers)
            {
                childContainer.SubscribeFunctionToControlSelectEvents(function);
            }
        }


        public void SubscribeFunctionToControlFocusEvents(EventHandler function)
        {
            ControlManager.OnFocus += function;

            foreach (UiContainer childContainer in ChildContainers)
            {
                childContainer.SubscribeFunctionToControlFocusEvents(function);
            }
        }


        protected Vector2 ToRelativePositionAboutContainer(Rectangle childRectangle, float childX, float atParentX, float childY, float atParentY)
        {
            float x = ToRelativeX(childRectangle, DisplayRectangle, childX, atParentX);
            float y = ToRelativeY(childRectangle, DisplayRectangle, childY, atParentY);
            return new Vector2(x, y);
        }
        protected float ToRelativeX(Rectangle childRectangle, Rectangle parentRectangle, float childX, float atParentX)
        {
            return parentRectangle.Width * atParentX - childRectangle.Width * childX;
        }
        protected float ToRelativeY(Rectangle childRectangle, Rectangle parentRectangle, float childY, float atParentY)
        {
            return parentRectangle.Height * atParentY - childRectangle.Height * childY;
        }


        public void SetBackgroundColor(Color color)
        {
            BackgroundTexture = GameEnvironment.Game.Content.Load<Texture2D>(@"Sprites\White");
            BackgroundColor = color;
        }
    }
}
