using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Utility;
using Microsoft.Xna.Framework.Audio;

namespace TDGameLibrary.Audio
{
    public static class SoundManager
    {
        public static int MAX_SOUNDS = 30;

        private static Dictionary<string, Queue<Sound>> SoundPool = new Dictionary<string, Queue<Sound>>();
        private static Dictionary<string, int> CountPlaying = new Dictionary<string, int>();
        private static Dictionary<string, int> MaxInstances = new Dictionary<string, int>();
        private static List<Sound> CurrentlyPlaying = new List<Sound>();
        private static List<Sound> FinishedPlaying = new List<Sound>();
        public static float TickPitch
        {
            get
            {
                return _tickPitch;
            }
            set
            {
                _tickPitch = value;
                UseTickPitch = true;
            }
        }
        public static bool UseTickPitch;
        private static float _tickPitch;

        public static Sound Play(string soundName, Vector2 screenPosition, float pitch, float volume)
        {
            if (CurrentlyPlaying.Count >= MAX_SOUNDS)
            {
                return null;
            }

            CountPlaying.AddKeyIfMissing(soundName);
            MaxInstances.AddKeyIfMissing(soundName);

            //Don't play sound if already playing the limit for this sound
            if (MaxInstances[soundName] > 0 && CountPlaying[soundName] >= MaxInstances[soundName])
            {
                return null;
            }

            //Attempt to play sound
            Sound sound = GetSound(soundName);
            if (sound != null)
            {
                float soundDistance = Vector2.Distance(screenPosition, GameEnvironment.ScreenRectangle.Center.ToVector2());
                float volumeByDistance = (1 - soundDistance / (GameEnvironment.ScreenRectangle.Width)) * volume;

                screenPosition.X = screenPosition.X - (TDGameLibrary.GameEnvironment.ScreenRectangle.Width / 2);
                screenPosition.Normalize();

                sound.Pan = screenPosition.X;
                sound.Pitch = pitch;
                sound.BasePitch = pitch;
                sound.Volume = volumeByDistance;
                sound.Play();

                sound.OnFinishedPlaying -= FinishedPlayingSound;
                sound.OnFinishedPlaying += FinishedPlayingSound;
                CountPlaying[soundName]++;
                CurrentlyPlaying.Add(sound);
            }

            return sound;
        }

        public static Sound Play(string soundName)
        {
            return Play(soundName, GameEnvironment.ScreenRectangle.Center.ToVector2());
        }

        public static Sound Play(string soundName, float volume)
        {
            return Play(soundName, GameEnvironment.ScreenRectangle.Center.ToVector2(), 0f, volume);
        }

        public static Sound Play(string soundName, float pitch, float volume)
        {
            return Play(soundName, GameEnvironment.ScreenRectangle.Center.ToVector2(), pitch, volume);
        }

        public static Sound Play(string soundName, Vector2 screenPosition)
        {
            return Play(soundName, screenPosition, 0f);
        }

        public static Sound Play(string soundName, Vector2 screenPosition, float pitch)
        {
            return Play(soundName, screenPosition, pitch, 1f);
        }

        private static Sound GetSound(string soundName)
        {
            //Get the sound from the pool if possible
            SoundPool.AddKeyIfMissing(soundName);
            if (SoundPool[soundName] == null)
            {
                SoundPool[soundName] = new Queue<Sound>();
            }

            if (SoundPool[soundName].Count > 0)
            {
                return SoundPool[soundName].Dequeue();
            }

            //Otherwise, create a new sound
            SoundEffectInstance soundEffectInstance = GameEnvironment.Game.Content.Load<SoundEffect>(soundName).CreateInstance();
            Sound sound = new Sound(soundEffectInstance, soundName);
            return new Sound(soundEffectInstance, soundName);
        }

        private static void FinishedPlayingSound(object sender, EventArgs e)
        {
            Sound sound = sender as Sound;
            if (sound != null)
            {
                CountPlaying[sound.Name]--;
            }
            FinishedPlaying.Add(sound);
        }

        public static void SetMaxInstanceCount(string soundName, int maxInstances)
        {
            MaxInstances.AddKeyIfMissing(soundName);
            MaxInstances[soundName] = maxInstances;
        }

        public static void PauseAll()
        {
            foreach (Sound sound in CurrentlyPlaying)
            {
                sound.Pause();
            }
        }

        public static void UnpauseAll()
        {
            foreach (Sound sound in CurrentlyPlaying)
            {
                sound.Play();
            }
        }

        public static void StopAll()
        {
            foreach (Sound sound in CurrentlyPlaying)
            {
                sound.Stop();
            }
        }

        public static void Update(GameTime gameTime)
        {
            foreach (Sound sound in CurrentlyPlaying)
            {
                sound.Update();
            }
            foreach (Sound sound in FinishedPlaying)
            {
                CurrentlyPlaying.Remove(sound);
                SoundPool.AddKeyIfMissing(sound.Name);
                SoundPool[sound.Name].Enqueue(sound);
            }
            FinishedPlaying.Clear();

            UseTickPitch = false;
        }

    }
}
