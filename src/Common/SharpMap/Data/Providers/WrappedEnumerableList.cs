using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// todo: figure out where to put this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WrappedEnumerableList<T> : IList
    {
        private IEnumerable<T> Source { get; set; }
        private IList<T> EditableList { get; set; }

        public WrappedEnumerableList(IEnumerable<T> source, IList<T> editableList)
        {
            Source = source;
            EditableList = editableList;
        }

        public IEnumerator GetEnumerator()
        {
            return Source.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count { get { return Source.Count(); } }
        public object SyncRoot { get; private set; }
        public bool IsSynchronized { get; private set; }
        public int Add(object value)
        {
            EditableList.Add((T) value);
            return EditableList.Count;
        }

        public bool Contains(object value)
        {
            return Source.Contains((T) value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            var i = 0;
            foreach(var elem in Source)
            {
                if (ReferenceEquals(elem, value))
                    return i;
                i++;
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get { return Source.ElementAt(index); }
            set { throw new NotImplementedException(); }
        }

        public bool IsReadOnly { get; private set; }
        public bool IsFixedSize { get; private set; }
    }
}