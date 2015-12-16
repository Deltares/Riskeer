using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Utils.Attributes;
using Core.Common.Utils.PropertyBag;
using Core.Common.Utils.PropertyBag.Dynamic;
using NUnit.Framework;
using CategoryComponentModelAttribute = System.ComponentModel.CategoryAttribute;

namespace Core.Common.Utils.Test.PropertyBag
{
    [TestFixture]
    public class DynamicPropertyBagTest
    {
        [Test]
        public void DynamicPropertyBagReturnsCorrectProperties()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestProperties());

            Assert.AreEqual(4, dynamicPropertyBag.Properties.Count, "Expected property count wrong");
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
            Assert.IsTrue(nameProperty.Attributes.Any(x => x.GetType() == typeof(CategoryComponentModelAttribute)), "Static Category attribute not copied!");

            CollectionAssert.Contains(descriptionProperty.Attributes, ReadOnlyAttribute.Yes, "Static ReadOnly attribute not copied!");
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
            Assert.IsTrue(namePropertyDescriptor.Attributes.Matches(new DynamicReadOnlyAttribute()),"Dynamic ReadOnly attribute was not added");

            Assert.IsFalse(namePropertyDescriptor.Attributes.Matches(new ReadOnlyAttribute(true)), "Inactive dynamic ReadOnly attribute was resolved to static attribute: wrong.");
        }

        [Test]
        public void DynamicPropertyBagResolvesDynamicAttributes()
        {
            var dynamicPropertyBag = new DynamicPropertyBag(new TestProperties());

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicPropertyBag).GetProperties();

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.GetValue(dynamicPropertyBag);

            // asserts
            Assert.IsTrue(namePropertyDescriptor.Attributes.Matches(new DynamicReadOnlyAttribute()), "Dynamic ReadOnly attribute was not added");

            Assert.IsTrue(namePropertyDescriptor.Attributes.Matches(new ReadOnlyAttribute(true)), "Dynamic ReadOnly attribute was not resolved to static attribute: wrong.");
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
        [ExpectedException(typeof(MissingMethodException), ExpectedMessage = "DynamicReadOnlyValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: Core.Common.Utils.Test.PropertyBag.DynamicPropertyBagTest+TestWithoutValidationMethodClassProperties")]
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
            Assert.IsInstanceOf<DynamicPropertyBag>(wrappedValue, "Object properties wrapped in dynamic property bag");
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

            Assert.AreEqual(expected, testProperties.Name, "Name not correctly set");
        }

        [Test]
        [ExpectedException(typeof(MissingMethodException), ExpectedMessage = "DynamicReadOnlyValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: Core.Common.Utils.Test.PropertyBag.DynamicPropertyBagTest+TestInvalidValidationMethodClassProperties")]
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
        [ExpectedException(typeof(MissingMethodException), ExpectedMessage = "Slechts één DynamicReadOnlyValidationMethod toegestaan per klasse: Core.Common.Utils.Test.PropertyBag.DynamicPropertyBagTest+TestWithTwoValidationMethodsClassProperties.")]
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

        private class TestProperties
        {
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

            public bool IsNameReadOnly { get; set; }

            public bool Visible { get; set; }

            [ReadOnly(true)] //one static property
            public string Description { get; set; }

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