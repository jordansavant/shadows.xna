using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.DataStructures
{
    /// <summary>
    /// Wraps a generic list and supports efficient access to basic operations.
    /// Add, Remove, Count, Contains, IEnumerable Items are supported approaching O(1) time.
    /// Clear is supported in O(n) time.
    /// This structure ignores null items and allows duplicate items.
    /// A consequence of the design is that GetEnumerator() enumerates duplicate items in contiguous groups.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashedList<T> : IEnumerable<T>
    {
        public HashedList()
        {
            _items = new Dictionary<T, int>();
            _count = 0;
        }

        private int _count;
        public virtual int Count
        {
            get
            {
                return _count;
            }
        }

        private Dictionary<T, int> _items;
        public virtual IEnumerable<T> Items
        {
            get
            {
                return _items.Keys;
            }
        }

        public virtual void Add(T item)
        {
            if (item != null)
            {
                if (!_items.ContainsKey(item))
                {
                    _items.Add(item, 0);
                }
                _items[item]++;
                _count++;
            }
        }

        public virtual void Remove(T item)
        {
            if (item != null && _items.ContainsKey(item))
            {
                if (_items[item] > 1)
                {
                    _items[item]--;
                }
                else
                {
                    _items.Remove(item);
                }
                _count--;
            }
        }

        public virtual bool Contains(T item)
        {
            return item != null && _items.ContainsKey(item);
        }

        public virtual void Clear()
        {
            _items.Clear();
            _count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int count = 0;
            foreach (T item in _items.Keys)
            {
                while (count++ < _items[item])
                {
                    yield return item;
                }
                count = 0;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
