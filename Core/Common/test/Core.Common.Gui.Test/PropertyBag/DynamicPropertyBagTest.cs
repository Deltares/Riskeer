using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

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
            var namePropertySpec = dynamicPropertyBag.Properties.First(ps => ps.Name == "Name");
            CollectionAssert.Contains(namePropertySpec.Attributes, new System.ComponentModel.CategoryAttribute("General"),
                "Should have initialized Attributes of the property spec with declared Category(\"General\").");

            var descriptionPropertySpec = dynamicPropertyBag.Properties.First(ps => ps.Name == "Description");
            CollectionAssert.Contains(descriptionPropertySpec.Attributes, ReadOnlyAttribute.Yes, 
                "Should have initialized Attributes of the property spec with declared ReadOnlyAttribute.");
        }

        [Test]
        public void ToString_ReturnToStringFromWrappedObject()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var text = dynamicPropertyBag.ToString();

            // Assert
            Assert.AreEqual(target.ToString(), text);
        }

        [Test]
        public void GetAttributes_Always_ReturnEmpty()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var attributes = dynamicPropertyBag.GetAttributes();

            // Assert
            CollectionAssert.IsEmpty(attributes);
        }

        [Test]
        public void GetClassName_Always_ReturnDynamicPropertyBagClassName()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var className = dynamicPropertyBag.GetClassName();

            // Assert
            Assert.AreEqual(dynamicPropertyBag.GetType().FullName, className);
        }

        [Test]
        public void GetComponentName_Always_ReturnNull()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var componentName = dynamicPropertyBag.GetComponentName();

            // Assert
            Assert.IsNull(componentName);
        }

        [Test]
        public void GetConverter_Always_ReturnDefaultTypeConverter()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var typeConverter = dynamicPropertyBag.GetConverter();

            // Assert
            Assert.AreEqual(TypeDescriptor.GetConverter(dynamicPropertyBag, true), typeConverter);
        }

        [Test]
        public void GetDefaultEvent_Always_ReturnNull()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var eventDescriptor = dynamicPropertyBag.GetDefaultEvent();

            // Assert
            Assert.IsNull(eventDescriptor);
        }

        [Test]
        public void GetEvents_Always_ReturnEmpty()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var events = dynamicPropertyBag.GetEvents();

            // Assert
            CollectionAssert.IsEmpty(events);
        }

        [Test]
        public void GetEventsParametered_Always_ReturnEmpty()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var events = dynamicPropertyBag.GetEvents(new Attribute[0]);

            // Assert
            CollectionAssert.IsEmpty(events);
        }

        [Test]
        public void GetEditor_Always_ReturnNull()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var editor = dynamicPropertyBag.GetEditor(typeof(UITypeEditor));

            // Assert
            Assert.IsNull(editor);
        }

        [Test]
        public void GetDefaultProperty_ObjectHasNotProperties_ReturnNull()
        {
            // Setup
            var dynamicPropertyBag = new DynamicPropertyBag(new object());

            // Call
            var defaultProperty = dynamicPropertyBag.GetDefaultProperty();

            // Assert
            Assert.IsNull(defaultProperty);
        }

        [Test]
        public void GetDefaultProperty_ObjectWithProperties_ReturnFirstFromProperties()
        {
            // Setup
            var target = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(target);

            // Call
            var defaultProperty = dynamicPropertyBag.GetDefaultProperty();

            // Assert
            Assert.AreEqual(dynamicPropertyBag.Properties.First().Name, defaultProperty.Name);
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
        public void GetProperties_SomePropertiesWithOrderAttribute_ReturnElementsInDesiredOrdering()
        {
            // Setup
            var dynamicPropertyBag = new DynamicPropertyBag(new TestOrderedProperties());

            // Call
            var propertyDescriptorCollection = dynamicPropertyBag.GetProperties();

            // Assert
            var index = 0;
            Assert.AreEqual("PropTwo", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("PropOne", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("Description", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("Name", propertyDescriptorCollection[index++].DisplayName);

            var propThreeDescriptor = propertyDescriptorCollection.Find("PropThree", false);
            Assert.GreaterOrEqual(propertyDescriptorCollection.IndexOf(propThreeDescriptor), index,
                "PropThree is not decorated with PropertyOrderAttribute, therefore should come after those that are.");
            var propFourDescriptor = propertyDescriptorCollection.Find("PropFour", false);
            Assert.GreaterOrEqual(propertyDescriptorCollection.IndexOf(propFourDescriptor), index,
                "PropFour is not decorated with PropertyOrderAttribute, therefore should come after those that are.");
        }

        [Test]
        public void DynamicPropertyBagWrapsNestedPropertyObjects()
        {
            // Setup
            var subProperties = new TestProperties
            {
                Name = "test"
            };
            var testProperties = new TestWithNestedPropertiesClassProperties
            {
                SubProperties = subProperties
            };
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            var propertiesCollection = dynamicPropertyBag.GetProperties();

            // Call
            var wrappedValue = propertiesCollection[0].GetValue(dynamicPropertyBag.WrappedObject);

            // Assert
            var bag = (DynamicPropertyBag)wrappedValue;
            Assert.AreSame(subProperties, bag.WrappedObject);
        }

        [Test]
        public void GivenPropertyDescriptorFromDynamicPropertyBag_WhenSettingProperty_ThenWrappedObjectUpdated()
        {
            // Setup
            var testProperties = new TestProperties
            {
                Name = "name"
            };
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            var newName = "newName";

            // Call
            dynamicPropertyBag.GetProperties()["Name"].SetValue(testProperties, newName);

            // Assert
            Assert.AreEqual(newName, testProperties.Name);
        }

        [Test]
        public void GivenPropertiesWithoutPublicSetter_WhenGettingPropertyDescriptors_ThenPropertiesDecoratedWithReadOnlyAttribute()
        {
            // Setup
            var dynamicPropertyBag = new DynamicPropertyBag(new TestWithoutSetterPropertyClassProperties());

            // Call
            var propertyDescriptorCollection = dynamicPropertyBag.GetProperties();

            // Assert
            Assert.IsTrue(propertyDescriptorCollection[0].Attributes.Matches(ReadOnlyAttribute.Yes));
            Assert.IsTrue(propertyDescriptorCollection[1].Attributes.Matches(ReadOnlyAttribute.Yes));
        }

        [Test]
        public void GetProperties_RepeatedCallForSameState_RetainSameElementOrderAndContents()
        {
            // Setup
            var propertyObject = new TestOrderedProperties();

            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            var originalProperties = dynamicPropertyBag.GetProperties();

            // Call
            for (int i = 0; i < 100; i++)
            {
                var currentProperties = dynamicPropertyBag.GetProperties();

                // Assert
                CollectionAssert.AreEqual(originalProperties, currentProperties);
            }
        }

        [Test]
        public void GetProperties_RepeatedConstructionsForSameState_RetainSameElementOrderAndContents()
        {
            // Setup
            var propertyObject = new TestOrderedProperties();

            var originalProperties = new DynamicPropertyBag(propertyObject).GetProperties();

            // Call
            for (int i = 0; i < 100; i++)
            {
                var currentProperties = new DynamicPropertyBag(propertyObject).GetProperties();

                // Assert
                CollectionAssert.AreEqual(originalProperties, currentProperties);
            }
        }

        [Test]
        public void GetProperties_BrowsableTrueFilter_ReturnOnlyPropertiesThatAreBrowsable()
        {
            // Setup
            var propertyObject = new TestProperties
            {
                Visible = false
            };
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Call
            var properties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });

            // Assert
            Assert.Less(properties.Count, dynamicPropertyBag.Properties.Count);
            Assert.IsNull(properties.Find("Name", false),
                "Name is dynamically not browsable, therefore should not be returned.");
            Assert.IsNotNull(properties.Find("Description", false));
            Assert.IsNotNull(properties.Find("IsNameReadOnly", false));
            Assert.IsNull(properties.Find("Visible", false),
                "Visible is statically not browsable, therefore should not be returned.");
        }

        [Test]
        public void GetProperties_BrowsableNoFilter_ReturnOnlyPropertiesThatAreBrowsable()
        {
            // Setup
            var propertyObject = new TestProperties
            {
                Visible = false
            };
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Call
            var properties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.No
            });

            // Assert
            Assert.Less(properties.Count, dynamicPropertyBag.Properties.Count);
            Assert.IsNotNull(properties.Find("Name", false),
                "Name is dynamically not browsable, therefore should be returned.");
            Assert.IsNull(properties.Find("Description", false));
            Assert.IsNull(properties.Find("IsNameReadOnly", false));
            Assert.IsNotNull(properties.Find("Visible", false),
                "Visible is statically not browsable, therefore should be returned.");
        }

        [Test]
        public void GetPropertyOwner_Always_ReturnWrappedObject()
        {
            // Setup
            var propertyObject = new TestProperties();
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Call
            var owner = dynamicPropertyBag.GetPropertyOwner(null);

            // Assert
            Assert.AreSame(propertyObject, owner);
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

            public int PropThree { get; set; }

            public int PropFour { get; set; }
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

            [Browsable(true)]
            public bool IsNameReadOnly { get; set; }

            [Browsable(false)]
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

            public override string ToString()
            {
                return Name;
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