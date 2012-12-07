using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.DataStructures
{
    public class DistinctSet<T> : HashedList<T>
    {
        public DistinctSet()
        {
        }

        public override void Add(T item)
        {
            if (!Items.Contains(item))
            {
                base.Add(item);
            }
        }

    }
}
