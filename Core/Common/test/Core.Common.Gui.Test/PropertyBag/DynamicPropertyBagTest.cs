// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using CategoryAttribute = System.ComponentModel.CategoryAttribute;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class DynamicPropertyBagTest
    {
        [Test]
        public void Constructor_PropertyObjectNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DynamicPropertyBag(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyObject", paramName);
        }

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
        public void Constructor_ClassWith4Properties_CorrectNumberOfPropertiesCreated()
        {
            // Setup
            var propertyObject = new TestProperties();

            // Call
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Assert
            Assert.AreEqual(4, dynamicPropertyBag.Properties.Count(), "Expected property count wrong");
        }

        [Test]
        public void Constructor_ClassWithAttributes_PropertySpecsHaveAttributesSet()
        {
            // Setup
            var propertyObject = new TestProperties();

            // Call
            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);

            // Assert
            PropertySpec namePropertySpec = dynamicPropertyBag.Properties.First(ps => ps.Name == "Name");
            CollectionAssert.Contains(namePropertySpec.Attributes, new CategoryAttribute("General"),
                                      "Should have initialized Attributes of the property spec with declared Category(\"General\").");

            PropertySpec descriptionPropertySpec = dynamicPropertyBag.Properties.First(ps => ps.Name == "Description");
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
            string text = dynamicPropertyBag.ToString();

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
            AttributeCollection attributes = dynamicPropertyBag.GetAttributes();

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
            string className = dynamicPropertyBag.GetClassName();

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
            string componentName = dynamicPropertyBag.GetComponentName();

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
            TypeConverter typeConverter = dynamicPropertyBag.GetConverter();

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
            EventDescriptor eventDescriptor = dynamicPropertyBag.GetDefaultEvent();

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
            EventDescriptorCollection events = dynamicPropertyBag.GetEvents();

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
            EventDescriptorCollection events = dynamicPropertyBag.GetEvents(new Attribute[0]);

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
            object editor = dynamicPropertyBag.GetEditor(typeof(UITypeEditor));

            // Assert
            Assert.IsNull(editor);
        }

        [Test]
        public void GetDefaultProperty_ObjectHasNotProperties_ReturnNull()
        {
            // Setup
            var dynamicPropertyBag = new DynamicPropertyBag(new object());

            // Call
            PropertyDescriptor defaultProperty = dynamicPropertyBag.GetDefaultProperty();

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
            PropertyDescriptor defaultProperty = dynamicPropertyBag.GetDefaultProperty();

            // Assert
            Assert.NotNull(defaultProperty);
            Assert.AreEqual(dynamicPropertyBag.Properties.First().Name, defaultProperty.Name);
        }

        [Test]
        public void GivenObjectPropertiesWithDynamicVisibleProperties_WhenPropertyShown_ThenPropertyShouldBePresentInBag()
        {
            // Given
            var propertyObject = new TestProperties
            {
                Visible = true
            };

            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);
            PropertyDescriptorCollection propertyDescriptorCollection = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // When
            const string dynamicallyVisiblePropertyName = nameof(TestProperties.Name);
            PropertyDescriptor namePropertyDescriptor = propertyDescriptorCollection.Find(dynamicallyVisiblePropertyName, false);

            // Then
            Assert.IsTrue(namePropertyDescriptor.IsBrowsable,
                          $"{dynamicallyVisiblePropertyName} should be visible");
        }

        [Test]
        public void GivenObjectPropertiesWithDynamicVisibleProperties_WhenPropertyHidden_ThenPropertyNotPresentInBag()
        {
            // Setup
            var propertyObject = new TestProperties
            {
                Visible = false
            };

            var dynamicPropertyBag = new DynamicPropertyBag(propertyObject);
            PropertyDescriptorCollection propertyDescriptorCollection = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Call
            const string dynamicallyVisiblePropertyName = nameof(TestProperties.Name);
            PropertyDescriptor namePropertyDescriptor = propertyDescriptorCollection.Find(dynamicallyVisiblePropertyName, false);

            // Assert
            Assert.IsNull(namePropertyDescriptor, $"{dynamicallyVisiblePropertyName} should not be visible anymore");
        }

        [Test]
        public void GetProperties_SomePropertiesWithOrderAttribute_ReturnElementsInDesiredOrdering()
        {
            // Setup
            var dynamicPropertyBag = new DynamicPropertyBag(new TestOrderedProperties());

            // Call
            PropertyDescriptorCollection propertyDescriptorCollection = dynamicPropertyBag.GetProperties();

            // Assert
            var index = 0;
            Assert.AreEqual("PropSix", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("PropFour", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("PropTwo", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("PropOne", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("Description", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("PropFive", propertyDescriptorCollection[index++].DisplayName);
            Assert.AreEqual("Name", propertyDescriptorCollection[index++].DisplayName);

            PropertyDescriptor propThreeDescriptor = propertyDescriptorCollection.Find("PropThree", false);
            Assert.GreaterOrEqual(propertyDescriptorCollection.IndexOf(propThreeDescriptor), index,
                                  "PropThree is not decorated with PropertyOrderAttribute or DynamicPropertyOrderAttribute, therefore should come after those that are.");
            PropertyDescriptor propSevenDescriptor = propertyDescriptorCollection.Find("PropSeven", false);
            Assert.GreaterOrEqual(propertyDescriptorCollection.IndexOf(propSevenDescriptor), index,
                                  "PropSeven is not decorated with PropertyOrderAttribute or DynamicPropertyOrderAttribute, therefore should come after those that are.");
        }

        [Test]
        public void GetProperties_PropertyIsDecoratedWithExpandableObjectConverter_WrapPropertyValueInDynamicPropertyBag()
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

            // Call
            PropertyDescriptorCollection propertiesCollection = dynamicPropertyBag.GetProperties();

            // Assert
            var bag = propertiesCollection[0].GetValue(dynamicPropertyBag.WrappedObject) as DynamicPropertyBag;
            Assert.NotNull(bag);
            Assert.AreSame(subProperties, bag.WrappedObject);
        }

        [Test]
        public void GivenPropertyDescriptorFromDynamicPropertyBag_WhenSettingProperty_ThenWrappedObjectUpdated()
        {
            // Given
            var testProperties = new TestProperties
            {
                Name = "name"
            };
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);

            const string newName = "newName";

            // When
            dynamicPropertyBag.GetProperties()["Name"].SetValue(testProperties, newName);

            // Then
            Assert.AreEqual(newName, testProperties.Name);
        }

        [Test]
        public void GetProperties_ClassWithPropertiesWithoutPublicSetter_PropertiesDecoratedWithReadOnlyAttribute()
        {
            // Setup
            var dynamicPropertyBag = new DynamicPropertyBag(new TestWithoutSetterPropertyClassProperties());

            // Call
            PropertyDescriptorCollection propertyDescriptorCollection = dynamicPropertyBag.GetProperties();

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

            PropertyDescriptorCollection originalProperties = dynamicPropertyBag.GetProperties();

            // Call
            for (var i = 0; i < 100; i++)
            {
                PropertyDescriptorCollection currentProperties = dynamicPropertyBag.GetProperties();

                // Assert
                CollectionAssert.AreEqual(originalProperties, currentProperties);
            }
        }

        [Test]
        public void GetProperties_RepeatedConstructionsForSameState_RetainSameElementOrderAndContents()
        {
            // Setup
            var propertyObject = new TestOrderedProperties();

            PropertyDescriptorCollection originalProperties = new DynamicPropertyBag(propertyObject).GetProperties();

            // Call
            for (var i = 0; i < 100; i++)
            {
                PropertyDescriptorCollection currentProperties = new DynamicPropertyBag(propertyObject).GetProperties();

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
            PropertyDescriptorCollection properties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(propertyObject);

            // Assert
            Assert.Less(properties.Count, dynamicPropertyBag.Properties.Count());
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
            PropertyDescriptorCollection properties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.No
            });

            // Assert
            Assert.Less(properties.Count, dynamicPropertyBag.Properties.Count());
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
            object owner = dynamicPropertyBag.GetPropertyOwner(null);

            // Assert
            Assert.AreSame(propertyObject, owner);
        }

        #region Test Classes

        private class TestOrderedProperties
        {
            [PropertyOrder(6)]
            public string Name { get; set; }

            [PropertyOrder(4)]
            public string Description { get; set; }

            [PropertyOrder(3)]
            public string PropOne { get; set; }

            [PropertyOrder(2)]
            public string PropTwo { get; set; }

            public int PropThree { get; set; }

            [DynamicPropertyOrder]
            public int PropFour { get; set; }

            [DynamicPropertyOrder]
            public int PropFive { get; set; }

            [PropertyOrder(0)]
            [DynamicPropertyOrder]
            public int PropSix { get; set; }

            public int PropSeven { get; set; }

            [DynamicPropertyOrderEvaluationMethod]
            public int PropertyOrder(string propertyName)
            {
                if (propertyName == "PropFour")
                {
                    return 1;
                }

                if (propertyName == "PropFive")
                {
                    return 5;
                }

                return 7;
            }
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
            [Category("General")]
            public string Name { get; set; }

            [Browsable(true)]
            public bool IsNameReadOnly { get; set; } // Property needs to have a setter, otherwise some tests will fail

            [Browsable(false)]
            public bool Visible { get; set; }

            [ReadOnly(true)]
            public string Description { get; set; } // Property needs to have a setter, otherwise some tests will fail

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