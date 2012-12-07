using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Ui.Modifiers
{
    public class BlinkModifier : UiControlModifier
    {
        public BlinkModifier(UiControl control, TimeSpan blinkRate)
            : this(control, blinkRate, Color.White.ToOpacity(0f))
        {
        }

        public BlinkModifier(UiControl control, TimeSpan blinkRate, Color endColor)
            : base(control)
        {
            TimeForFullFade = TimeSpan.FromMilliseconds(blinkRate.TotalMilliseconds / 2f);
            EndColor = endColor;
            StartColor = control.Color;
        }

        private bool IsFadingToEnd;
        private TimeSpan TimeForFullFade, TimeSinceLastFade;
        private Color EndColor, StartColor;

        public void Reset()
        {
            IsFadingToEnd = false;
            TimeSinceLastFade = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (TimeSinceLastFade.TotalMilliseconds > TimeForFullFade.TotalMilliseconds)
            {
                if (IsFadingToEnd)
                {
                    FinishEffect();
                }

                IsFadingToEnd = !IsFadingToEnd;
                TimeSinceLastFade = TimeSpan.Zero;
            }

            float percentFaded = (float)TimeSinceLastFade.TotalMilliseconds / (float)TimeForFullFade.TotalMilliseconds;
            if (IsFadingToEnd)
            {
                Control.Color = Color.Lerp(StartColor, EndColor, percentFaded);
            }
            else
            {
                Control.Color = Color.Lerp(EndColor, StartColor, percentFaded);
            }

            TimeSinceLastFade += gameTime.ElapsedGameTime;
        }

    }
}
