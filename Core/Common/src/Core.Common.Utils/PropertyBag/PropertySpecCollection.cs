using System;
using System.Collections;

namespace Core.Common.Utils.PropertyBag
{
    /// <summary>
    /// Encapsulates a collection of PropertySpec objects.
    /// </summary>
    [Serializable]
    public class PropertySpecCollection : IList
    {
        private readonly ArrayList innerArray;

        /// <summary>
        /// Initializes a new instance of the PropertySpecCollection class.
        /// </summary>
        public PropertySpecCollection()
        {
            innerArray = new ArrayList();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// In C#, this property is the indexer for the PropertySpecCollection class.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>
        /// The element at the specified index.
        /// </value>
        public PropertySpec this[int index]
        {
            get
            {
                return (PropertySpec) innerArray[index];
            }
            set
            {
                innerArray[index] = value;
            }
        }

        /// <summary>
        /// Gets the number of elements in the PropertySpecCollection.
        /// </summary>
        /// <value>
        /// The number of elements contained in the PropertySpecCollection.
        /// </value>
        public int Count
        {
            get
            {
                return innerArray.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the PropertySpecCollection has a fixed size.
        /// </summary>
        /// <value>
        /// true if the PropertySpecCollection has a fixed size; otherwise, false.
        /// </value>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the PropertySpecCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread-safe).
        /// </summary>
        /// <value>
        /// true if access to the PropertySpecCollection is synchronized (thread-safe); otherwise, false.
        /// </value>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the collection.
        /// </value>
        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a PropertySpec to the end of the PropertySpecCollection.
        /// </summary>
        /// <param name="value">The PropertySpec to be added to the end of the PropertySpecCollection.</param>
        /// <returns>The PropertySpecCollection index at which the value has been added.</returns>
        public int Add(PropertySpec value)
        {
            int index = innerArray.Add(value);

            return index;
        }

        /// <summary>
        /// Adds the elements of an array of PropertySpec objects to the end of the PropertySpecCollection.
        /// </summary>
        /// <param name="array">The PropertySpec array whose elements should be added to the end of the
        /// PropertySpecCollection.</param>
        public void AddRange(PropertySpec[] array)
        {
            innerArray.AddRange(array);
        }

        /// <summary>
        /// Determines whether a PropertySpec is in the PropertySpecCollection.
        /// </summary>
        /// <param name="item">The PropertySpec to locate in the PropertySpecCollection. The element to locate
        /// can be a null reference (Nothing in Visual Basic).</param>
        /// <returns>true if item is found in the PropertySpecCollection; otherwise, false.</returns>
        public bool Contains(PropertySpec item)
        {
            return innerArray.Contains(item);
        }

        /// <summary>
        /// Determines whether a PropertySpec with the specified name is in the PropertySpecCollection.
        /// </summary>
        /// <param name="name">The name of the PropertySpec to locate in the PropertySpecCollection.</param>
        /// <returns>true if item is found in the PropertySpecCollection; otherwise, false.</returns>
        public bool Contains(string name)
        {
            foreach (PropertySpec spec in innerArray)
            {
                if (spec.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the entire PropertySpecCollection to a compatible one-dimensional Array, starting at the
        /// beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied
        /// fromPropertySpecCollection. The Array must have zero-based indexing.</param>
        public void CopyTo(PropertySpec[] array)
        {
            innerArray.CopyTo(array);
        }

        /// <summary>
        /// Copies the PropertySpecCollection or a portion of it to a one-dimensional array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied
        /// from the collection.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(PropertySpec[] array, int index)
        {
            innerArray.CopyTo(array, index);
        }

        /// <summary>
        /// Searches for the specified PropertySpec and returns the zero-based index of the first
        /// occurrence within the entire PropertySpecCollection.
        /// </summary>
        /// <param name="value">The PropertySpec to locate in the PropertySpecCollection.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire PropertySpecCollection,
        /// if found; otherwise, -1.</returns>
        public int IndexOf(PropertySpec value)
        {
            return innerArray.IndexOf(value);
        }

        /// <summary>
        /// Searches for the PropertySpec with the specified name and returns the zero-based index of
        /// the first occurrence within the entire PropertySpecCollection.
        /// </summary>
        /// <param name="name">The name of the PropertySpec to locate in the PropertySpecCollection.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire PropertySpecCollection,
        /// if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            int i = 0;

            foreach (PropertySpec spec in innerArray)
            {
                if (spec.Name == name)
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Inserts a PropertySpec object into the PropertySpecCollection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The PropertySpec to insert.</param>
        public void Insert(int index, PropertySpec value)
        {
            innerArray.Insert(index, value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the PropertySpecCollection.
        /// </summary>
        /// <param name="obj">The PropertySpec to remove from the PropertySpecCollection.</param>
        public void Remove(PropertySpec obj)
        {
            innerArray.Remove(obj);
        }

        /// <summary>
        /// Removes the property with the specified name from the PropertySpecCollection.
        /// </summary>
        /// <param name="name">The name of the PropertySpec to remove from the PropertySpecCollection.</param>
        public void Remove(string name)
        {
            int index = IndexOf(name);
            RemoveAt(index);
        }

        /// <summary>
        /// Copies the elements of the PropertySpecCollection to a new PropertySpec array.
        /// </summary>
        /// <returns>A PropertySpec array containing copies of the elements of the PropertySpecCollection.</returns>
        public PropertySpec[] ToArray()
        {
            return (PropertySpec[]) innerArray.ToArray(typeof(PropertySpec));
        }

        /// <summary>
        /// Removes all elements from the PropertySpecCollection.
        /// </summary>
        public void Clear()
        {
            innerArray.Clear();
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the PropertySpecCollection.
        /// </summary>
        /// <returns>An IEnumerator for the entire PropertySpecCollection.</returns>
        public IEnumerator GetEnumerator()
        {
            return innerArray.GetEnumerator();
        }

        /// <summary>
        /// Removes the object at the specified index of the PropertySpecCollection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            innerArray.RemoveAt(index);
        }

        #region Explicit interface implementations for ICollection and IList

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((PropertySpec[]) array, index);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        int IList.Add(object value)
        {
            return Add((PropertySpec) value);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        bool IList.Contains(object obj)
        {
            return Contains((PropertySpec) obj);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        object IList.this[int index]
        {
            get
            {
                return ((PropertySpecCollection) this)[index];
            }
            set
            {
                ((PropertySpecCollection) this)[index] = (PropertySpec) value;
            }
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        int IList.IndexOf(object obj)
        {
            return IndexOf((PropertySpec) obj);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        void IList.Insert(int index, object value)
        {
            Insert(index, (PropertySpec) value);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        void IList.Remove(object value)
        {
            Remove((PropertySpec) value);
        }

        #endregion
    }
}