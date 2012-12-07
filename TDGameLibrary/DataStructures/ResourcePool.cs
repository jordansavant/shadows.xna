using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.DataStructures
{
    public class ResourcePool<T>
    {
        public ResourcePool(Func<T> createResourceInstance)
        {
            if (createResourceInstance == null)
            {
                throw new ArgumentNullException("createResourceInstance");
            }

            CreateResourceInstance = createResourceInstance;
            StartingInstanceCount = 0;
            AvailableResources = new Stack<T>();

            for (int i = 0; i < StartingInstanceCount; i++)
            {
                Release(CreateResourceInstance.Invoke());
            }
        }

        public ResourcePool(Func<T> createResourceInstance, int startingInstanceCount)
            : this(createResourceInstance)
        {
            StartingInstanceCount = startingInstanceCount;
        }

        public ResourcePool(List<T> resources, Func<T> createResourceInstance, int startingInstanceCount)
            : this(createResourceInstance, startingInstanceCount)
        {
            AvailableResources = resources != null ? new Stack<T>(resources) : AvailableResources;
        }

        private Stack<T> AvailableResources;
        private Func<T> CreateResourceInstance;
        private int StartingInstanceCount;

        public T Fetch()
        {
            if (!AvailableResources.Any())
            {
                return CreateResourceInstance.Invoke();
            }
            return AvailableResources.Pop();
        }

        public void Release(T item)
        {
            if (item != null && !AvailableResources.Contains(item))
            {
                AvailableResources.Push(item);
            }
        }

    }
}
