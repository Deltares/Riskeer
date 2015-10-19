using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils.ComponentModel;
using DelftTools.Utils.PropertyBag;
using DelftTools.Utils.PropertyBag.Dynamic;
using NUnit.Framework;
using SharpTestsEx;
using CategoryComponentModelAttribute = System.ComponentModel.CategoryAttribute;

namespace DelftTools.Utils.Tests.PropertyBag
{
    [TestFixture]
    public class DynamicPropertyBagTest
    {
        [Test]
        public void DynamicPropertyBagReturnsCorrectProperties()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestProperties());

            dynamicPropertyBag.Properties.Count
                              .Should("Expected property count wrong").Be.EqualTo(4);
        }

        [Test]
        public void DynamicPropertyBagCopiesStaticAttributes()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestProperties());

            PropertySpec nameProperty = null;
            PropertySpec descriptionProperty = null;

            foreach (PropertySpec spec in dynamicPropertyBag.Properties)
            {
                if (spec.Name.Equals("Description"))
                {
                    descriptionProperty = spec;
                }
                else if (spec.Name.Equals("Name"))
                {
                    nameProperty = spec;
                }
            }

            // asserts
            nameProperty.Attributes.Any(x => x.GetType() == typeof(CategoryComponentModelAttribute))
                        .Should("Static Category attribute not copied!").Be.True();

            descriptionProperty.Attributes
                               .Should("Static ReadOnly attribute not copied!").Contain(ReadOnlyAttribute.Yes);
        }

        [Test]
        public void DynamicPropertyBagResolvesDynamicAttributesToNothing()
        {
            var testProperties = new TestProperties
            {
                IsNameReadOnly = false
            };

            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.GetValue(dynamicPropertyBag);

            // asserts
            namePropertyDescriptor.Attributes.Matches(new DynamicReadOnlyAttribute())
                                  .Should("Dynamic ReadOnly attribute was not added").Be.True();

            namePropertyDescriptor.Attributes.Matches(new ReadOnlyAttribute(true))
                                  .Should("Inactive dynamic ReadOnly attribute was resolved to static attribute: wrong.").Be.False();
        }

        [Test]
        public void DynamicPropertyBagResolvesDynamicAttributes()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestProperties());

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.GetValue(dynamicPropertyBag);

            // asserts
            namePropertyDescriptor.Attributes.Matches(new DynamicReadOnlyAttribute())
                                  .Should("Dynamic ReadOnly attribute was not added").Be.True();

            namePropertyDescriptor.Attributes.Matches(new ReadOnlyAttribute(true))
                                  .Should("Dynamic ReadOnly attribute was not resolved to static attribute: wrong.").Be.True();
        }

        [Test]
        public void DynamicPropertyBagResolvesDynamicVisibleAttributes()
        {
            var propertyObject = new TestProperties
            {
                Visible = true
            };
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties(new[]
            {
                new BrowsableAttribute(true)
            });

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);
            Assert.IsTrue(namePropertyDescriptor.IsBrowsable, "Name should be visible");

            propertyObject.Visible = false;

            propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties(new[]
            {
                new BrowsableAttribute(true)
            });

            namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);
            Assert.IsNull(namePropertyDescriptor, "Name should not be visible anymore");
        }

        [Test]
        [ExpectedException(typeof(MissingMethodException), ExpectedMessage = "DynamicReadOnlyValidationMethod not found (or not public), class: DelftTools.Utils.Tests.PropertyBag.DynamicPropertyBagTest+TestWithoutValidationMethodClassProperties")]
        public void ThrowsExceptionOnTypoInDynamicAttributeFunction()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestWithoutValidationMethodClassProperties());
            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();
            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.GetValue(dynamicPropertyBag);
        }

        [Test]
        public void DynamicPropertyBagMaintainsDesiredOrdering()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestOrderedProperties());

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();

            Assert.AreEqual("Name", propertyDescriptorCollection[3].DisplayName);
            Assert.AreEqual("Description", propertyDescriptorCollection[2].DisplayName);
            Assert.AreEqual("PropOne", propertyDescriptorCollection[1].DisplayName);
            Assert.AreEqual("PropTwo", propertyDescriptorCollection[0].DisplayName);
        }

        [Test]
        public void DynamicPropertyBagWrapsNestedPropertyObjects()
        {
            var subProperties = new TestProperties()
            {
                Name = "test"
            };
            var testProperties = new TestWithNestedPropertiesClassProperties()
            {
                SubProperties = subProperties
            };
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            var propertiesCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();
            var wrappedValue = propertiesCollection[0].GetValue(dynamicPropertyBag);

            // check the object properties are wrapped in a dynamic property bag
            wrappedValue.Should("Object properties wrapped in dynamic property bag").Be.OfType<DynamicPropertyBag>();
        }

        [Test]
        public void DynamicPropertyBagPropagatesValueSetter()
        {
            var propertyGrid = new PropertyGrid();

            var testProperties = new TestProperties
            {
                Name = "name"
            };
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            propertyGrid.SelectedObject = dynamicPropertyBag;

            var expected = "newName";
            propertyGrid.SelectedGridItem.PropertyDescriptor.SetValue(null, expected);

            testProperties.Name
                          .Should("Name not correctly set").Be.EqualTo(expected);
        }

        [Test]
        [ExpectedException(typeof(MissingMethodException), ExpectedMessage = "DynamicReadOnlyValidationMethod has incorrect number of arguments, should be 1 of type string, class: DelftTools.Utils.Tests.PropertyBag.DynamicPropertyBagTest+TestInvalidValidationMethodClassProperties")]
        public void ThrowsExceptionOnInvalidValidationMethod()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestInvalidValidationMethodClassProperties());
            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();
            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.GetValue(dynamicPropertyBag);
        }

        [Test]
        public void PropertyWithNoSetterAreReadOnly()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestWithoutSetterPropertyClassProperties());
            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();

            //check both properties have the readonly attribute 
            Assert.IsTrue(propertyDescriptorCollection[0].Attributes.Matches(new ReadOnlyAttribute(true)));
            Assert.IsTrue(propertyDescriptorCollection[1].Attributes.Matches(new ReadOnlyAttribute(true)));
        }

        [Test]
        [ExpectedException(typeof(MissingMethodException), ExpectedMessage = "Only one DynamicReadOnlyValidationMethod is allowed per class: DelftTools.Utils.Tests.PropertyBag.DynamicPropertyBagTest+TestWithTwoValidationMethodsClassProperties")]
        public void OnlySingleValidationMethodIsAllowed()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestWithTwoValidationMethodsClassProperties());

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();
            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.GetValue(dynamicPropertyBag);
        }

        #region Test Classes

        private class TestWithNestedPropertiesClassProperties
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public TestProperties SubProperties { get; set; }
        }

        private class TestInvalidValidationMethodClassProperties
        {
            [DynamicReadOnly]
            public string Name { get; set; }

            [DynamicReadOnlyValidationMethod]
            public bool InvalidMethod() //method is invalid because it does not accept a string
            {
                return false;
            }
        }

        private class TestWithoutValidationMethodClassProperties
        {
            [DynamicReadOnly]
            public string Name { get; set; }
        }

        private class TestWithTwoValidationMethodsClassProperties
        {
            [DynamicReadOnly]
            public string Name { get; set; }

            [DynamicReadOnlyValidationMethod]
            public bool Method1(string property)
            {
                return false;
            }

            [DynamicReadOnlyValidationMethod]
            public bool Method2(string property)
            {
                return false;
            }
        }

        private class TestOrderedProperties
        {
            [PropertyOrder(3)]
            public string Name { get; set; }

            [PropertyOrder(2)]
            public string Description { get; set; }

            [PropertyOrder(1)]
            public string PropOne { get; set; }

            [PropertyOrder(0)]
            public string PropTwo { get; set; }
        }

        private class TestProperties : INotifyPropertyChange
        {
            public event PropertyChangedEventHandler PropertyChanged;
// Required by interface, but not used (yet)
#pragma warning disable 67
            public event PropertyChangingEventHandler PropertyChanging;
#pragma warning restore 67
            private bool isNameReadOnly;

            public TestProperties()
            {
                Name = "my name";
                Description = "short description";
                Visible = true;
                IsNameReadOnly = true;
            }

            /// <summary>
            /// Dynamic property. ReadOnly when IsNameReadOnly true.
            /// </summary>
            [DynamicReadOnly]
            [DynamicVisible]
            [CategoryComponentModelAttribute("General")]
            public string Name { get; set; }

            public bool IsNameReadOnly
            {
                get
                {
                    return isNameReadOnly;
                }
                set
                {
                    isNameReadOnly = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsNameReadOnly"));
                    }
                }
            }

            public bool Visible { get; set; }

            [ReadOnly(true)] //one static property
            public string Description { get; set; }

            bool INotifyPropertyChange.HasParent { get; set; }

            /// <summary>
            /// Method checks if propertyName property is read-only (setter can be used).
            /// </summary>
            /// <param name="propertyName"></param>
            /// <returns></returns>
            [DynamicReadOnlyValidationMethod]
            public bool DynamicReadOnlyValidationMethod(string propertyName)
            {
                Assert.IsTrue(propertyName == "Name");

                return IsNameReadOnly;
            }

            [DynamicVisibleValidationMethod]
            public bool DynamicVisibleValidationMethod(string propertyName)
            {
                return Visible;
            }
        }

        public class TestWithoutSetterPropertyClassProperties
        {
            public string PrivateSetter { get; private set; }

            public string NoSetter
            {
                get
                {
                    return "";
                }
            }
        }

        #endregion
    }
}