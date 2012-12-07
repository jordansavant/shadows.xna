using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Audio
{
    public class Sound : IDisposable
    {
        public Sound(SoundEffectInstance soundEffectInstance, string soundName)
        {
            SoundEffectInstance = soundEffectInstance;
            Name = soundName;
        }

        public event EventHandler OnFinishedPlaying;
        public SoundEffectInstance SoundEffectInstance;
        public int RemainingLoops;
        public string Name;

        public float Pan { get { return SoundEffectInstance.Pan; } set { SoundEffectInstance.Pan = MathHelper.Clamp(value, -1.0f, 1.0f); } }
        public float Pitch { get { return SoundEffectInstance.Pitch; } set { SoundEffectInstance.Pitch = MathHelper.Clamp(value, -1.0f, 1.0f); } }
        public float Volume { get { return SoundEffectInstance.Volume; } set { SoundEffectInstance.Volume = MathHelper.Clamp(value, 0.0f, 1.0f); } }
        public float BasePitch;

        public bool IsPaused
        {
            get
            {
                return SoundEffectInstance.State == SoundState.Paused;
            }
        }

        public void Pause()
        {
            SoundEffectInstance.Pause();
        }

        private void Unpause()
        {
            if (SoundEffectInstance.State == SoundState.Paused)
            {
                SoundEffectInstance.Resume();
            }
        }

        public bool IsStopped
        {
            get
            {
                return SoundEffectInstance.State == SoundState.Stopped;
            }
        }

        public bool HasFiniteRemainingLoops
        {
            get
            {
                return RemainingLoops > 0;
            }
        }

        public void Play()
        {
            if (IsPaused)
            {
                Unpause();
            }
            else if (RemainingLoops == 0)
            {
                RemainingLoops = 1;
            }
        }

        public void Loop()
        {
            Loop(-1);
        }

        public void Loop(int numberOfRepeats)
        {
            Unpause();
            RemainingLoops = numberOfRepeats;
        }

        public void Finish()
        {
            Stop(true);
        }

        public void Stop()
        {
            Stop(false);
        }

        public void Stop(bool finishCurrentLoop)
        {
            RemainingLoops = 0;
            SoundEffectInstance.Stop(!finishCurrentLoop);
        }

        public void Update()
        {
            Pitch = BasePitch;
            if (SoundManager.UseTickPitch)
            {
                Pitch = SoundManager.TickPitch;
            }

            if (!IsPaused && RemainingLoops != 0 && SoundEffectInstance.State != SoundState.Playing)
            {
                SoundEffectInstance.Play();
                if (HasFiniteRemainingLoops)
                {
                    RemainingLoops--;
                }
            }
            else if (IsStopped && !HasFiniteRemainingLoops)
            {
                if (OnFinishedPlaying != null)
                {
                    OnFinishedPlaying(this, EventArgs.Empty);
                }
            }
        }


        public void Dispose()
        {
            if (SoundEffectInstance != null)
            {
                SoundEffectInstance.Dispose();
            }
        }

    }
}
