using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Sound;

namespace TDGameLibrary
{
    public static class TDGLFactoryExtensions
    {
        public static SoundManager SoundManager(this Object host, string soundName)
        {
            return new SoundManager(host, soundName);
        }
    }
}
