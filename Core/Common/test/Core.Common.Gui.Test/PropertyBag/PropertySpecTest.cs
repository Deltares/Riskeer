using System.ComponentModel;
using System.Linq;

using Core.Common.Gui.PropertyBag;

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
            var propertyName = "DoublePropertyWithOnlyPublicSet";
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

        private class ClassWithProperties
        {
            public int IntegerProperty { get; set; }

            public double DoublePropertyWithOnlyPublicSet { get; private set; }

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