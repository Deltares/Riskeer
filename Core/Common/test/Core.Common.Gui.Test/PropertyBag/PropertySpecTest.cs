using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;

using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class PropertySpecTest
    {
        [Test]
        public void ParameteredConstructor_FromPropertyWithoutAttributesWithPublicGetSet_ExpectedValues()
        {
            // Setup
            var propertyName = "IntegerProperty";
            var propertyInfo = typeof(ClassWithProperties).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            CollectionAssert.IsEmpty(propertySpec.Attributes);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithoutAttributesWithOnlyPublicGet_ExpectedValues()
        {
            // Setup
            var propertyName = "DoublePropertyWithOnlyPublicGet";
            var propertyInfo = typeof(ClassWithProperties).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(1, propertySpec.Attributes.Length);
            var readOnlyAttribute = (ReadOnlyAttribute)propertySpec.Attributes[0];
            Assert.IsTrue(readOnlyAttribute.IsReadOnly);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithAttributesWithPublicGetSet_ExpectedValues()
        {
            // Setup
            var propertyName = "StringPropertyWithAttributes";
            var propertyInfo = typeof(ClassWithProperties).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(2, propertySpec.Attributes.Length);
            var browsableAttribute = propertySpec.Attributes.OfType<BrowsableAttribute>().Single();
            Assert.IsTrue(browsableAttribute.Browsable);
            var readOnlyAttribute = propertySpec.Attributes.OfType<ReadOnlyAttribute>().Single();
            Assert.IsFalse(readOnlyAttribute.IsReadOnly);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyOverridingAttributesFromBaseClass_InheritedAttributesAreIgnored()
        {
            // Setup
            var propertyName = "StringPropertyWithAttributes";
            var propertyInfo = typeof(InheritorSettingPropertyToNotBrowsable).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(1, propertySpec.Attributes.Length);
            var browsableAttribute = propertySpec.Attributes.OfType<BrowsableAttribute>().Single();
            Assert.IsFalse(browsableAttribute.Browsable,
                "Should have the override value.");
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithAttributesFromBaseClass_InheritedAttributesAreIgnored()
        {
            // Setup
            var propertyName = "BoolPropertyWithAttributes";
            var propertyInfo = typeof(InheritorSettingPropertyToNotBrowsable).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(1, propertySpec.Attributes.Length);
            var browsableAttribute = propertySpec.Attributes.OfType<BrowsableAttribute>().Single();
            Assert.IsTrue(browsableAttribute.Browsable,
                "No override in 'InheritorSettingPropertyToNotBrowsable' for property 'BoolPropertyWithAttributes', so use base class.");
        }

        [Test]
        public void ParameteredConstructor_ForIndexProperty_ThrowArgumentException()
        {
            // Setup
            var propertyInfo = new ClassWithProperties().GetType().GetProperty("Item");

            // Call
            TestDelegate call = () => new PropertySpec(propertyInfo);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Index properties are not allowed.");
        }

        [Test]
        public void SetValue_ProperInstanceTypeAndValueType_PropertyIsUpdated()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            propertySpec.SetValue(target, 2);

            // Assert
            Assert.AreEqual(2, target.IntegerProperty);
        }

        [Test]
        public void SetValue_IncorrectInstanceType_ThrowArgumentException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(new object(), 2);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsInstanceOf<TargetException>(exception.InnerException);
        }

        [Test]
        public void SetValue_InstanceIsNull_ThrowArgumentException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(null, 2);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsInstanceOf<TargetException>(exception.InnerException);
        }

        [Test]
        public void SetValue_PropertyWithoutPublicGet_ThrowInvalidOperationException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("DoublePropertyWithOnlyGetter"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(target, 2);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            Assert.AreEqual("Property lacks public setter!", exception.Message);
        }

        [Test]
        public void SetValue_SettingValueOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(target, new object());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsNull(exception.InnerException);
        }

        private class ClassWithProperties
        {
            public int IntegerProperty { get; set; }

            public float this[int index]
            {
                get
                {
                    return default(float);
                }
                set
                {
                    
                }
            }

            public double DoublePropertyWithOnlyPublicGet { get; private set; }

            public double DoublePropertyWithOnlyGetter
            {
                get
                {
                    return 0.0;
                }
            }

            [Browsable(true)]
            [ReadOnly(false)]
            public virtual string StringPropertyWithAttributes { get; set; }

            [Browsable(true)]
            public bool BoolPropertyWithAttributes { get; set; }
        }

        private class InheritorSettingPropertyToNotBrowsable : ClassWithProperties
        {
            [Browsable(false)]
            public override string StringPropertyWithAttributes { get; set; }
        }
    }
}