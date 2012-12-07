using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Utility
{
    public class GameTimer
    {
        public GameTimer(float seconds)
        {
            SetNewDuration(seconds);
        }

        public GameTimer(float seconds, bool startMaxed)
            :this(seconds)
        {
            if(startMaxed)
                Counter = TimeSpan.FromSeconds(Seconds);
        }

        private float Seconds;
        private TimeSpan WaitTime, Counter;
        public EventHandler<EventArgs> OnFire;

        public bool Update(GameTime gameTime)
        {
            Counter += gameTime.ElapsedGameTime;

            if (Counter >= WaitTime)
            {
                Counter = TimeSpan.FromSeconds(0);

                if (OnFire != null)
                {
                    OnFire.Invoke(this, EventArgs.Empty);
                }

                return true;
            }

            return false;
        }

        public void Reset()
        {
            Counter = TimeSpan.FromSeconds(0);
        }

        public void Max()
        {
            Counter = WaitTime;
        }

        public void AdjustBy(float seconds)
        {
            WaitTime = TimeSpan.FromSeconds(Seconds + seconds);
        }

        public void SetNewDuration(float seconds)
        {
            Seconds = seconds;
            Counter = TimeSpan.FromSeconds(0);
            WaitTime = TimeSpan.FromSeconds(Seconds);
        }

        public float GetCompletionRatio()
        {
            return (float)(Counter.TotalMilliseconds / WaitTime.TotalMilliseconds);
        }

    }
}
