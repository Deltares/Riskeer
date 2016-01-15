using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class DynamicPropertyBagTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var propertyObject = new object();

            // Call
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Assert
            CollectionAssert.IsEmpty(dynamicPropertyBag.Properties,
                "Object has no properties, therefore bag should have none too.");
            Assert.AreSame(propertyObject, dynamicPropertyBag.WrappedObject);
        }

        [Test]
        public void GivenTestProperties_WhenConstructing_ThenCorrectNumberOfPropertiesCreated()
        {
            // Setup
            var propertyObject = new TestProperties();

            // Call
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Assert
            Assert.AreEqual(4, dynamicPropertyBag.Properties.Count, "Expected property count wrong");
        }

        [Test]
        public void GivenTestProperties_WhenConstructing_ThenPropertySpecsHaveAttributesSet()
        {
            // Setup
            var propertyObject = new TestProperties();

            // Call
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Assert
            var namePropertySpec = dynamicPropertyBag.Properties.OfType<PropertySpec>().First(ps => ps.Name == "Name");
            Assert.IsTrue(namePropertySpec.Attributes.Any(a => a is System.ComponentModel.CategoryAttribute));

            var descriptionPropertySpec = dynamicPropertyBag.Properties.OfType<PropertySpec>().First(ps => ps.Name == "Description");
            CollectionAssert.Contains(descriptionPropertySpec.Attributes, ReadOnlyAttribute.Yes, 
                "Should have initialized Attributes of the property spec with declared ReadOnlyAttribute.");
        }

        [Test]
        public void GivenClassWithDynamicReadOnlyAttribute_WhenNotReadOnly_ThenTypeDescriptorDoesNotHaveReadOnlyAttribute()
        {
            // Setup
            var testProperties = new TestProperties
            {
                IsNameReadOnly = false
            };

            // Precondition
            var namePropertyAttributes = testProperties.GetType().GetProperty("Name").GetCustomAttributes(true);
            Assert.IsFalse(namePropertyAttributes.Any(a => a is ReadOnlyAttribute));
            CollectionAssert.Contains(namePropertyAttributes, new DynamicReadOnlyAttribute());
            Assert.IsFalse(testProperties.DynamicReadOnlyValidationMethod("Name"));

            // Call
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            // Assert
            PropertyDescriptorCollection propertyDescriptorCollection = dynamicPropertyBag.GetProperties();
            PropertyDescriptor namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            CollectionAssert.Contains(namePropertyDescriptor.Attributes, new DynamicReadOnlyAttribute(),
                "DynamicReadOnlyAttribute declared on Name property should also be present on PropertyDescriptor.");
            Assert.IsFalse(namePropertyDescriptor.Attributes.OfType<Attribute>().Any(a => a is ReadOnlyAttribute),
                "As Name property has no ReadOnlyAttribute nor does DyanmicReadOnlyValidationMethod evaluate to true, no ReadOnlyAttribute should be on PropertyDescriptor.");
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
            var wrappedValue = propertiesCollection[0].GetValue(dynamicPropertyBag.WrappedObject);

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
            propertyGrid.SelectedGridItem.PropertyDescriptor.SetValue(testProperties, expected);

            Assert.AreEqual(expected, testProperties.Name, "Name not correctly set");
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

        #region Test Classes

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

        private class TestWithNestedPropertiesClassProperties
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public TestProperties SubProperties { get; set; }
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
            [System.ComponentModel.Category("General")]
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

        private class TestWithoutSetterPropertyClassProperties
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