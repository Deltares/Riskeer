using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DelftTools.Utils.PropertyBag
{
    /// <summary>
    /// Represents the method that will handle the GetValue and SetValue events of the
    /// PropertyBag class.
    /// </summary>
    public delegate void PropertySpecEventHandler(object sender, PropertySpecEventArgs e);

    /// <summary>
    /// Represents a collection of custom properties that can be selected into a
    /// PropertyGrid to provide functionality beyond that of the simple reflection
    /// normally used to query an object's properties.
    /// </summary>
    public class PropertyBag : ICustomTypeDescriptor
    {
        /// <summary>
        /// Occurs when a PropertyGrid requests the value of a property.
        /// </summary>
        public event PropertySpecEventHandler GetValue;

        /// <summary>
        /// Occurs when the user changes the value of a property in a PropertyGrid.
        /// </summary>
        public event PropertySpecEventHandler SetValue;

        /// <summary>
        /// Initializes a new instance of the PropertyBag class.
        /// </summary>
        public PropertyBag()
        {
            DefaultProperty = null;
            Properties = new PropertySpecCollection();
        }

        /// <summary>
        /// Gets or sets the name of the default property in the collection.
        /// </summary>
        public string DefaultProperty { get; set; }

        /// <summary>
        /// Gets the collection of properties contained within this PropertyBag.
        /// </summary>
        public PropertySpecCollection Properties { get; private set; }

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        protected virtual void OnGetValue(PropertySpecEventArgs e)
        {
            if (GetValue != null)
            {
                GetValue(this, e);
            }
        }

        /// <summary>
        /// Raises the SetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        protected virtual void OnSetValue(PropertySpecEventArgs e)
        {
            if (SetValue != null)
            {
                SetValue(this, e);
            }
        }

        #region PropertySpecCollection class definition

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

        #endregion

        #region PropertySpecDescriptor class definition

        private class PropertySpecDescriptor : PropertyDescriptor
        {
            private readonly PropertyBag bag;
            private readonly PropertySpec item;

            public PropertySpecDescriptor(PropertySpec item, PropertyBag bag, string name, Attribute[] attrs)
                :
                    base(name, attrs)
            {
                this.bag = bag;
                this.item = item;
            }

            public override Type ComponentType
            {
                get
                {
                    return item.GetType();
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return (Attributes.Matches(ReadOnlyAttribute.Yes));
                }
            }

            public override bool IsBrowsable
            {
                get
                {
                    ReEvaluateAttributes();
                    return base.IsBrowsable;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return Type.GetType(item.TypeName);
                }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return ReEvaluateAttributes().Value;
            }

            public override void ResetValue(object component)
            {

            }

            public override void SetValue(object component, object value)
            {
                // Have the property bag raise an event to set the current value
                // of the property.

                PropertySpecEventArgs e = new PropertySpecEventArgs(item, value);
                bag.OnSetValue(e);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            private PropertySpecEventArgs ReEvaluateAttributes()
            {
                // Have the property bag raise an event to get the current value
                // of the property and evaluate the dynamic attributes
                var e = new PropertySpecEventArgs(item, null);
                bag.OnGetValue(e);
                AttributeArray = e.Property.Attributes;
                return e;
            }
        }

        #endregion

        #region ICustomTypeDescriptor explicit interface definitions

        // Most of the functions required by the ICustomTypeDescriptor are
        // merely pssed on to the default TypeDescriptor for this type,
        // which will do something appropriate.  The exceptions are noted
        // below.
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        System.ComponentModel.TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            // This function searches the property list for the property
            // with the same name as the DefaultProperty specified, and
            // returns a property descriptor for it.  If no property is
            // found that matches DefaultProperty, a null reference is
            // returned instead.

            if (DefaultProperty == null && Properties.Count != 0)
            {
                DefaultProperty = Properties[0].Name;
            }

            PropertySpec propertySpec = null;
            if (DefaultProperty != null)
            {
                int index = Properties.IndexOf(DefaultProperty);
                propertySpec = Properties[index];
            }

            if (propertySpec != null)
            {
                return new PropertySpecDescriptor(propertySpec, this, propertySpec.Name, null);
            }
            else
            {
                return null;
            }
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor) this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            // Rather than passing this function on to the default TypeDescriptor,
            // which would return the actual properties of PropertyBag, I construct
            // a list here that contains property descriptors for the elements of the
            // Properties list in the bag.
            var props = new List<PropertySpecDescriptor>();
            var propsToOrder = new List<Tuple<int, PropertySpecDescriptor>>();

            foreach (PropertySpec property in Properties)
            {
                var attrs = new ArrayList();

                // Additionally, append the custom attributes associated with the
                // PropertySpec, if any.
                if (property.Attributes != null)
                {
                    attrs.AddRange(property.Attributes);
                }

                Attribute[] attrArray = (Attribute[]) attrs.ToArray(typeof(Attribute));

                // Create a new property descriptor for the property item, and add
                // it to the list.
                var pd = new PropertySpecDescriptor(property, this, property.Name, attrArray);

                var propertyOrderAttribute = property.Attributes != null ? property.Attributes.OfType<PropertyOrderAttribute>().FirstOrDefault() : null;
                if (propertyOrderAttribute != null)
                {
                    propsToOrder.Add(new Tuple<int, PropertySpecDescriptor>(propertyOrderAttribute.Order, pd));
                }
                else
                {
                    props.Add(pd);
                }
            }

            var orderedProperties = propsToOrder.OrderBy(p => p.First).Select(p => p.Second).ToList();

            // Convert the list of PropertyDescriptors to a collection that the
            // ICustomTypeDescriptor can use, and return it.
            var browsableAttribute = attributes.OfType<BrowsableAttribute>().FirstOrDefault();

            var propertySpecDescriptors = (browsableAttribute != null)
                                              ? orderedProperties.Concat(props).Where(p => p.IsBrowsable == browsableAttribute.Browsable)
                                              : orderedProperties.Concat(props);

            return new PropertyDescriptorCollection(propertySpecDescriptors.ToArray());
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}