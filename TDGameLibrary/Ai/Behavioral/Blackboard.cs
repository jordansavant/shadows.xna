using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Ai.Behavioral
{
    public class Blackboard
    {
        public Blackboard(Blackboard parent)
        {
            Parent = parent;
            Data = new Dictionary<string, object>();
        }

        private Blackboard Parent;
        private Dictionary<string, object> Data;

        public T Get<T>(string key)
        {
            object value;

            if(Data.ContainsKey(key) && Data[key] != null)
            {
                value = Data[key];

                if (value is T)
                {
                    return (T)value;
                }
            }
            
            if (Parent != null)
            {
                return Parent.Get<T>(key);
            }

            return default(T);
        }


        public void Set<T>(string key, T value)
        {
            Data[key] = value;
        }

    }
}
