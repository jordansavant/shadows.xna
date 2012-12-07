using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.DataStructures
{
    /// <summary>
    /// Inventory is a list of items (implemented as an array) that enforces the following rules:
    ///   1) The same item may not appear in the list more than once.
    ///   2) The exact ordering of items in the list is always preserved.
    ///   3) Both null and non-null items may be added to the list.
    ///   4) The capacity of the list can be changed, but not automatically.
    ///   5) Items can be automatically consolidated into the lowest indexes of the list, eliminating gaps
    ///      between items, but this behavior is optional.
    ///   6) No external entity may directly modify the list of items (but the items themselves may be modified).
    /// </summary>
    /// 
    public class Inventory<T>
        where T : class
    {
        public Inventory(int capacity)
        {
            Items = new T[capacity];
            ItemIndexTable = new Dictionary<T, int>(capacity);
        }

        public int Count { get; private set; } //ItemCount
        private T[] Items;
        private Dictionary<T, int> ItemIndexTable; //Allows FirstIndexOf() to find a match for non-null items in O(1) time
        
        public int Capacity
        {
            get
            {
                return Items.Length;
            }
        }

        public bool IsFull
        {
            get
            {
                return Count >= Items.Length;
            }
        }

        /// <summary>
        /// If true, items are automatically grouped into the lowest indexes of the inventory with no gaps.
        /// </summary>
        public bool ConsolidateItems { get; set; }

        /// <summary>
        /// Gets item at requested index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Item (possibly null) at given index, or null if given index was out of bounds.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Items.Length)
                {
                    return null;
                }
                return Items[index];
            }
        }

        /// <summary>
        /// Finds the first index of an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Index as value if a matching item was found, otherwise null.</returns>
        public int? this[T item]
        {
            get
            {
                return FirstIndexOf(item);
            }
        }

        /// <summary>
        /// Changes the inventory capacity. Negative values for newCapacity are treated as 0. If the new
        /// capacity is smaller than the old capacity, items at the highest indexes of the original inventory
        /// are discarded.
        /// </summary>
        /// <param name="newCapacity"></param>
        /// <returns>List of items from original inventory that were discard due to new capacity.</returns>
        public List<T> ChangeCapacity(int newCapacity)
        {
            return ChangeCapacity(newCapacity, false);
        }

        /// <summary>
        /// Changes the inventory capacity. Negative values for newCapacity are treated as 0. If the new
        /// capacity is smaller than the old capacity, items at the highest indexes of the original inventory
        /// are discarded. If consolidateFirst is true, items will be grouped into the lowest indexes prior
        /// to changing the inventory's capacity to minimize the number of items discarded.
        /// </summary>
        /// <param name="newCapacity"></param>
        /// <param name="consolidateFirst"></param>
        /// <returns>List of items from original inventory that were discard due to new capacity.</returns>
        public List<T> ChangeCapacity(int newCapacity, bool consolidateFirst)
        {
            if (consolidateFirst || ConsolidateItems)
            {
                Consolidate();
            }

            newCapacity = Math.Max(newCapacity, 0);
            T[] oldItems = Items;
            Items = new T[newCapacity];
            ItemIndexTable.Clear();

            int i = 0;
            while (i < Math.Min(oldItems.Length, newCapacity))
            {
                Items[i] = oldItems[i];
                if (Items[i] != null) //Dictionary doesn't support null keys
                {
                    ItemIndexTable.Add(Items[i], i);
                }
                i++;
            }

            List<T> discardedItems = new List<T>();
            while (i < Math.Max(oldItems.Length, newCapacity))
            {
                discardedItems.Add(oldItems[i]);
                i++;
            }

            return discardedItems;
        }

        /// <summary>
        /// Gets list of all items (including nulls) stored in the inventory. Modifying this list does not
        /// affect the inventory's own data structure, but modifications to items in either list apply
        /// universally.
        /// </summary>
        /// <returns>New list containing all items in the inventory.</returns>
        public List<T> GetItems()
        {
            return new List<T>(Items);
        }

        /// <summary>
        /// Adds item at first available index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if item was added or already existed, false if inventory was full.</returns>
        public virtual bool Add(T item)
        {
            int? index;
            if (!Items.Contains(item))
            {
                index = FirstIndexOf(null);
                if (!index.HasValue)
                {
                    return false;
                }
                Items[index.Value] = item;
                UpdateItemIndexTable(item, index.Value);
                Count++;
                TryConsolidate();
            }
            return true;
        }

        /// <summary>
        /// Adds item to given index. Moves item if it already exists at another index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns>True if item was added or moved, false if another item was already at requested index or
        /// requested index was out of bounds.</returns>
        public virtual bool Add(int index, T item)
        {
            if (index < 0 || index >= Items.Length || (Items[index] != null && Items[index] != item))
            {
                return false;
            }

            int? existingIndex = FirstIndexOf(item);
            if (existingIndex.HasValue)
            {
                Items[existingIndex.Value] = null;
            }
            else
            {
                Count++;
            }
            Items[index] = item;
            UpdateItemIndexTable(item, index);
            TryConsolidate();
            return true;
        }

        /// <summary>
        /// Removes given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if item was removed, false if it was not found in the inventory.</returns>
        public virtual bool Remove(T item)
        {
            int? index = FirstIndexOf(item);
            if (index.HasValue)
            {
                return Remove(index.Value);
            }
            return false;
        }

        /// <summary>
        /// Removes item at given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if item was removed or no item existed, false if requested index was out of bounds.</returns>
        public virtual bool Remove(int index)
        {
            if (index < 0 || index >= Items.Length)
            {
                return false;
            }
            T item = Items[index];
            Items[index] = null;
            if (item != null)
            {
                Count--;
                TryConsolidate();
            }
            UpdateItemIndexTable(item, index);
            return true;
        }

        /// <summary>
        /// Swaps two items.
        /// </summary>
        /// <param name="indexOne"></param>
        /// <param name="indexTwo"></param>
        /// <returns>True if items were swapped, false if either index was out of bounds.</returns>
        public virtual bool Swap(int indexOne, int indexTwo)
        {
            if (indexOne < 0 || indexOne >= Items.Length || indexTwo < 0 || indexTwo >= Items.Length)
            {
                return false;
            }

            T tmp = Items[indexOne];
            Items[indexOne] = Items[indexTwo];
            Items[indexTwo] = tmp;
            UpdateItemIndexTable(Items[indexOne], indexOne);
            UpdateItemIndexTable(Items[indexTwo], indexTwo);
            TryConsolidate();
            return true;
        }


        /// <summary>
        /// Replaces oldItem with newItem and removes oldItem, or swaps the items if both are
        /// already in the inventory.
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        /// <returns>True if oldItem existed and was replaced by newItem, false if oldItem did not exist.</returns>
        public virtual bool Replace(T oldItem, T newItem)
        {
            int? indexOfOld = FirstIndexOf(oldItem);
            int? indexOfNew = FirstIndexOf(newItem);
            
            if (indexOfOld.HasValue)
            {
                if (indexOfNew.HasValue)
                {
                    return Swap(indexOfOld.Value, indexOfNew.Value);
                }
                Items[indexOfOld.Value] = newItem;
                UpdateItemIndexTable(Items[indexOfOld.Value], indexOfOld.Value);
                UpdateItemIndexTable(Items[indexOfNew.Value], indexOfNew.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Replaces item at given index with newItem and removes the original item, or swaps the items if
        /// both are already in the inventory.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        /// <returns>True if item at given index was replaced by newItem, false if given index was out of bounds.</returns>
        public virtual bool Replace(int index, T newItem)
        {
            if (index < 0 || index >= Items.Length)
            {
                return false;
            }

            int? indexOfNew = FirstIndexOf(newItem);
            if (indexOfNew.HasValue)
            {
                return Swap(index, indexOfNew.Value);
            }

            Items[index] = newItem;
            UpdateItemIndexTable(newItem, index);
            TryConsolidate();
            return true;
        }

        /// <summary>
        /// Finds the first index of an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Index as value if a matching item was found, otherwise null.</returns>
        protected int? FirstIndexOf(T item)
        {
            if (item != null && ItemIndexTable.ContainsKey(item))
            {
                return ItemIndexTable[item]; //Optimization for approaching-O(1) searching of non-null items
            }

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == item)
                {
                    UpdateItemIndexTable(Items[i], i);
                    return i;
                }
            }
            return null;
        }

        private void TryConsolidate()
        {
            if (ConsolidateItems)
            {
                Consolidate();
            }
        }

        /// <summary>
        /// Groups items into the lowest indexes of the inventory so there are no gaps between items.
        /// Preserves existing item ordering. Runs in one pass at O(n) time.
        /// </summary>
        public void Consolidate()
        {
            ItemIndexTable.Clear();

            Queue<int> emptyCells = new Queue<int>();
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null)
                {
                    emptyCells.Enqueue(i);
                }
                else
                {
                    if (emptyCells.Any())
                    {
                        Items[emptyCells.Dequeue()] = Items[i];
                        Items[i] = null;
                        emptyCells.Enqueue(i);
                    }
                    UpdateItemIndexTable(Items[i], i);
                }
            }
        }

        private void UpdateItemIndexTable(T item, int newIndex)
        {
            if (item != null) //Dictionary doesn't support null keys
            {
                //Remove any existing key
                if (Items[newIndex] != null && ItemIndexTable.ContainsKey(Items[newIndex]))
                {
                    ItemIndexTable.Remove(Items[newIndex]);
                }

                //Update table, adding new key if necessary
                if (!ItemIndexTable.ContainsKey(item))
                {
                    ItemIndexTable.Add(item, newIndex);
                }
                ItemIndexTable[item] = newIndex;
            }
        }

    }
}
