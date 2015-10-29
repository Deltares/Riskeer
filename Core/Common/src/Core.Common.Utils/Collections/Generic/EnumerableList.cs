using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Core.Common.Utils.Collections.Generic
{
    /// <summary>
    /// Implement the IEnumerableList&lt;T&gt; list and allows caching of the Count to speeds up performance.
    /// If _collectionChangeSource is not set, Count will always be processed by Enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerableList<T> : IEnumerableList<T>, INotifyPropertyChange
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
        private IList<T> cache;
        private INotifyCollectionChange collectionChangeSource;
        private INotifyPropertyChange propertyChangeSource;
        private bool countDirty = true;
        private int countCache;
        private IEnumerable<T> enumerable;
        private bool dirtyBackingField;

        public EnumerableList()
        {
            Dirty = true;
        }

        public T this[int index]
        {
            get
            {
                UpdateCache();
                return cache.ElementAt(index);
            }
            set
            {
                Editor.OnReplace(index, value);
                Dirty = true;
            }
        }

        public int Count
        {
            get
            {
                if (!countDirty)
                {
                    return countCache;
                }

                countCache = Dirty ? Enumerable.Count() : cache.Count;
                countDirty = false;

                return countCache;
            }
        }

        public object SyncRoot
        {
            get
            {
                return Enumerable;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                if (Enumerable is ICollection)
                {
                    return ((ICollection) Enumerable).IsSynchronized;
                }

                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return Editor == null;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return Editor == null;
            }
        }

        public IEnumerable<T> Enumerable
        {
            get
            {
                return enumerable;
            }
            set
            {
                enumerable = value;
                Dirty = true;
            }
        }

        public IEnumerableListEditor Editor { get; set; }

        public INotifyCollectionChange CollectionChangeSource
        {
            get
            {
                return collectionChangeSource;
            }
            set
            {
                if (collectionChangeSource != null)
                {
                    collectionChangeSource.CollectionChanged -= NotifyCollectionChangeCollectionChange;
                }

                collectionChangeSource = value;

                if (collectionChangeSource != null)
                {
                    collectionChangeSource.CollectionChanged += NotifyCollectionChangeCollectionChange;
                }
            }
        }

        public INotifyPropertyChange PropertyChangeSource
        {
            get
            {
                return propertyChangeSource;
            }
            set
            {
                if (propertyChangeSource != null)
                {
                    propertyChangeSource.PropertyChanged -= PropertyChangeSourcePropertyChanged;
                }
                propertyChangeSource = value;

                if (propertyChangeSource != null)
                {
                    propertyChangeSource.PropertyChanged += PropertyChangeSourcePropertyChanged;
                }
            }
        }

        public bool HasParent { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            UpdateCache();
            return Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T o)
        {
            Editor.OnAdd(o);
            Dirty = true;
        }

        public int Add(object value)
        {
            Editor.OnAdd((T) value);
            Dirty = true;
            return Count - 1;
        }

        public bool Contains(object value)
        {
            UpdateCache();
            return cache.Contains((T) value);
        }

        public void Clear()
        {
            Editor.OnClear();
            Dirty = true;
        }

        public int IndexOf(object value)
        {
            UpdateCache();
            return cache.IndexOf((T) value);
        }

        public void Insert(int index, object value)
        {
            Editor.OnInsert(index, value);
            Dirty = true;
        }

        public void Remove(object value)
        {
            Editor.OnRemove(value);
            Dirty = true;
        }

        public bool Contains(T item)
        {
            UpdateCache();
            return cache.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            UpdateCache();
            cache.CopyTo(array, arrayIndex);
        }

        public bool Remove(T o)
        {
            Editor.OnRemove(o);
            Dirty = true;
            return true;
        }

        public void CopyTo(Array array, int index)
        {
            UpdateCache();
            cache.ToArray().CopyTo(array, index);
        }

        public int IndexOf(T item)
        {
            UpdateCache();

            var i = 0;
            foreach (var o in cache)
            {
                if (Equals(o, item))
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            Editor.OnInsert(index, item);
            Dirty = true;
        }

        public void RemoveAt(int index)
        {
            Editor.OnRemoveAt(index);
            Dirty = true;
        }

        private bool Dirty
        {
            get
            {
                return dirtyBackingField;
            }
            set
            {
                dirtyBackingField = value;

                if (!value)
                {
                    return;
                }
                countDirty = true;
            }
        }

        private void PropertyChangeSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null && (sender.GetType() == typeof(T)))
            {
                PropertyChanged(sender, e);
            }
        }

        private void NotifyCollectionChangeCollectionChange(object sender, NotifyCollectionChangingEventArgs e)
        {
            Dirty = true;
        }

        private void UpdateCache()
        {
            if (!Dirty)
            {
                return;
            }

            cache = Enumerable.ToList();
            countCache = cache.Count;
            countDirty = false;
            Dirty = false;
        }

        object IList.this[int index]
        {
            get
            {
                UpdateCache();
                return cache.ElementAt(index);
            }

            set
            {
                Editor.OnReplace(index, value);
                Dirty = true;
            }
        }
    }
}