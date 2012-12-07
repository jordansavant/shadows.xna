using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Mobs;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Components
{
    public class TriggerManager : TdglComponent
    {
        public TriggerManager()
        {
            watches = new Dictionary<Func<bool>, Action>();
            watchIds = new Dictionary<int, Func<bool>>();
        }

        private Dictionary<Func<bool>, Action> watches;
        private Dictionary<int, Func<bool>> watchIds;
        private int nextWatchId;

        public int AddWatch(Func<bool> cause, Action effect)
        {
            watches[cause] = effect;
            watchIds[nextWatchId] = cause;
            return nextWatchId++;
        }

        public void RemoveWatch(int watchId)
        {
            if (watchIds.ContainsKey(watchId))
            {
                watches.Remove(watchIds[watchId]);
                watchIds.Remove(watchId);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Func<bool> cause in watches.Keys)
            {
                if (cause.Invoke())
                {
                    watches[cause].Invoke();
                }
            }
        }

    }
}
