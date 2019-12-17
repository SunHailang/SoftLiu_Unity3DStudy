using System;
using System.Collections.Generic;
namespace SoftLiu.Utilities
{
    /// <summary>
    ///     Simple object pooler factory.
    ///     Unpools if possible, otherwise uses the func to spawn.
    /// </summary>
    public class PoolFactory<T>
    {
        private readonly Func<T> m_constructAction;
        private readonly Action<T> m_destroyAction;
        private readonly List<T> m_pooledItems;
        public PoolFactory(Func<T> constructAction, Action<T> destroyAction, int initialAllocation)
        {
            m_constructAction = constructAction;
            m_destroyAction = destroyAction;
            m_pooledItems = new List<T>(initialAllocation);
            GrowTo(initialAllocation);
        }
        public T Unpool()
        {
            var lastIndex = m_pooledItems.Count - 1;
            if (lastIndex >= 0)
            {
                var itemFromPool = m_pooledItems[lastIndex];
                m_pooledItems.RemoveAt(lastIndex);
                return itemFromPool;
            }
            return m_constructAction();
        }
        public void Pool(T item)
        {
            m_pooledItems.Add(item);
        }
        public void GrowTo(int capacity)
        {
            for (int i = m_pooledItems.Count; i < capacity; i++)
            {
                var newItem = m_constructAction();
                m_pooledItems.Add(newItem);
            }
        }
        public void ShrinkTo(int capacity)
        {
            for (int i = m_pooledItems.Count - 1; i >= capacity; i--)
            {
                m_destroyAction(m_pooledItems[i]);
                m_pooledItems.RemoveAt(i);
            }
            m_pooledItems.TrimExcess();
        }
    }
}