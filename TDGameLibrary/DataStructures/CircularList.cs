using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.DataStructures
{
    public class CircularList<T> where T : class
    {
        public CircularList()
        {
        }

        public EventHandler<EventArgs> OnChange;
        private LinkedListNode<T> _current;
        public LinkedListNode<T> Current
        {
            get
            {
                return _current;
            }
            private set
            {
                _current = value;

                if (OnChange != null && _current != null)
                {
                    OnChange.Invoke(_current.Value, EventArgs.Empty);
                }
            }
        }
        public int Count { get { return LinkedList.Count; } }
        protected LinkedList<T> LinkedList = new LinkedList<T>();
        protected Dictionary<Type, LinkedListNode<T>> Table = new Dictionary<Type, LinkedListNode<T>>();

        public virtual void SetTo(T item)
        {
            Add(item);
            Current = LinkedList.Find(item);
        }

        public virtual bool Add(T item)
        {
            if (Table.ContainsKey(item.GetType()))
            {
                return false;
            }

            LinkedListNode<T> c;
            if (LinkedList.Count == 0)
            {
                c = LinkedList.AddFirst(item);
                Current = c;
            }
            else
            {
                c = LinkedList.AddLast(item);
            }

            Table.Add(item.GetType(), c);

            return true;
        }

        public virtual void Remove(T item)
        {
            if(!Table.ContainsKey(item.GetType()))
            {
                return;
            }

            if (Current != null && Current.Next != null)
            {
                Current = Current.Next;
            }
            else if (Current != null && Current.Previous != null)
            {
                Current = Current.Previous;
            }
            else
            {
                Current = null;
            }

            Table.Remove(item.GetType());
            LinkedList.Remove(item);
        }

        public virtual bool Contains(T item)
        {
            return Table.ContainsKey(item.GetType());
        }

        public virtual void Clear()
        {
            Current = null;
            LinkedList.Clear();
            Table.Clear();
        }

        public virtual void CycleNext()
        {
            if (Current != null)
            {
                if (Current.Next != null)
                    Current = Current.Next;
                else if (LinkedList.First != null)
                    Current = LinkedList.First;
            }
        }

        public virtual void CyclePrevious()
        {
            if (Current != null)
            {
                if (Current.Previous != null)
                    Current = Current.Previous;
                else if (LinkedList.Last != null)
                    Current = LinkedList.Last;
            }
        }

        public virtual LinkedListNode<T> NextNode(LinkedListNode<T> n)
        {
            if (n.Next != null)
                return n.Next;
            else
                return LinkedList.First;
        }

        public virtual LinkedListNode<T> PreviousNode(LinkedListNode<T> n)
        {
            if (n.Previous != null)
                return n.Previous;
            else
                return LinkedList.Last;
        }
    }
}
