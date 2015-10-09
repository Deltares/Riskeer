using System;
using System.Collections;
using System.Collections.Generic;

namespace DelftTools.Utils
{
    /// <summary>
    /// Inherit this class to have an enumerable with [] and Count functionality. (but no other IList<T> functionality)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Selector<T> : IList<T>
    {
        public T this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public abstract int Count { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return new DefaultListEnumerator<T>(this);
        }

        protected abstract T Get(int index);

        #region Undesireds

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}