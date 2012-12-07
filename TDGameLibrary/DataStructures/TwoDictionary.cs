using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.DataStructures
{
    public class TwoDictionary<TKey, TKey2, TValue> : Dictionary<TKey, Dictionary<TKey2, TValue>>
    {
        public void AddKeysIfMissing(TKey key, TKey2 key2)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, new Dictionary<TKey2,TValue>());
            }
            if (!base[key].ContainsKey(key2))
            {
                base[key].Add(key2, default(TValue));
            }
        }

        public bool ContainsKeys(TKey outerKey, TKey2 innerKey)
        {
            return this.ContainsKey(outerKey) && base[outerKey].ContainsKey(innerKey);
        }
    }
}
