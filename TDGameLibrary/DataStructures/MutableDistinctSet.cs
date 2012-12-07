using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.DataStructures
{
    /// <summary>
    /// Wraps a generic list in a data structure that can be safely modified during iteration.
    /// Add and Remove are supported approaching O(1) time.
    /// To commit pending updates, consumers must call the Update() method.
    /// This structure ignores null items and does not allow duplicate items.
    /// A consequence of the design is that items pending an Add are recognized by Contains().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MutableDistinctSet<T> : IEnumerable<T>
    {
        public MutableDistinctSet()
        {
            _values = new HashedList<T>();
            _valuesToAdd = new HashedList<T>();
            _valuesToRemove = new HashedList<T>();
        }

        private HashedList<T> _values;
        private HashedList<T> _valuesToAdd;
        private HashedList<T> _valuesToRemove;

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        public IEnumerable<T> Items
        {
            get
            {
                return _values;
            }
        }

        public void Commit()
        {
            foreach (T item in _valuesToAdd)
            {
                _values.Add(item);
            }
            _valuesToAdd.Clear();

            foreach (T item in _valuesToRemove)
            {
                _values.Remove(item);
            }
            _valuesToRemove.Clear();
        }

        public void Undo()
        {
            _valuesToAdd.Clear();
            _valuesToRemove.Clear();
        }

        public void Add(T item)
        {
            if (item != null && !_valuesToAdd.Contains(item))
            {
                if (_valuesToRemove.Contains(item))
                {
                    _valuesToRemove.Remove(item);
                }
                else if (!_values.Contains(item))
                {
                    _valuesToAdd.Add(item);
                }
            }
        }

        public void Remove(T item)
        {
            if (item != null && !_valuesToRemove.Contains(item))
            {
                if (_valuesToAdd.Contains(item))
                {
                    _valuesToAdd.Remove(item);
                }
                else if (_values.Contains(item))
                {
                    _valuesToRemove.Add(item);
                }
            }
        }

        public bool Contains(T item)
        {
            return _values.Contains(item) || _valuesToAdd.Contains(item);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

    }
}
