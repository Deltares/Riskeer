using System;
using System.Linq;

using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms.PropertyGridView
{
    [TestFixture]
    public class PropertyResolverTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Call
            var resolver = new PropertyResolver(Enumerable.Empty<PropertyInfo>());

            // Assert
            Assert.IsInstanceOf<IPropertyResolver>(resolver);
        }

        [Test]
        public void ParameteredConstructor_InputIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PropertyResolver(null);

            // Assert
            const string expectedMessage = "Kan geen 'PropertyResolver' maken zonder een lijst van 'PropertyInfo'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void GetObjectProperties_DataIsNull_ReturnNull()
        {
            // Setup
            var propertyInfos = new PropertyInfo[] { new PropertyInfo<A, PropertiesForA>() };

            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var result = resolver.GetObjectProperties(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetObjectProperties_DataMatchesSinglePropertyInfoDataTypeDirectly_ReturnNewInstanceOfCorrespondingObjectProperties()
        {
            // Setup
            var propertyInfos = new PropertyInfo[] { new PropertyInfo<A, PropertiesForA>() };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<PropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesInfoWithGetObjectPropertiesData_ReturnObjectPropertiesWithDataFromInjectedDelegate()
        {
            // Setup
            var otherObject = new B
            {
                Name = "<Custom get properties data implementation did stuff>"
            };
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>
                {
                    GetObjectPropertiesData = a => otherObject
                }
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            var bag = (DynamicPropertyBag)result;
            var properties = (PropertiesForA)bag.WrappedObject;
            Assert.AreSame(otherObject, properties.Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesInfoWithAfterCreate_ReturnObjectPropertiesAfterCreateExecuted()
        {
            // Setup
            var otherObject = new B
            {
                Name = "<Custom get properties data implementation did stuff>"
            };
            var source = new A();

            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>
                {
                    AfterCreate = p =>
                    {
                        Assert.AreSame(source, p.Data,
                            "properties object should have been initialized with 'source'.");
                        p.Data = otherObject;
                    }
                }
            };

            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            var bag = (DynamicPropertyBag)result;
            var properties = (PropertiesForA)bag.WrappedObject;
            Assert.AreSame(otherObject, properties.Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoDataTypeDirectly_ReturnBasedOnAdditionalDataCheck()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>
                {
                    AdditionalDataCheck = a => false
                },
                new PropertyInfo<A, AlternativePropertiesForA>
                {
                    AdditionalDataCheck = a => true
                },
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<AlternativePropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoDataTypeDirectly_PrioritizeAdditionalDataCheckOverNotHavingCheck()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, AlternativePropertiesForA>
                {
                    AdditionalDataCheck = a => true
                },
                new PropertyInfo<A, PropertiesForA>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<AlternativePropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataHasNoMatches_ReturnNull()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, AlternativePropertiesForA>
                {
                    AdditionalDataCheck = a => false
                },
                new PropertyInfo<B, PropertiesForB>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoByDataTypeInheritance_PrioritizeMostSpecialized()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<InheritsFromA, AlternativePropertiesForA>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<AlternativePropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoByObjectPropertiesTypeInheritance_PrioritizeMostSpecialized()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<A, InheritsFromPropertiesForA>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<InheritsFromPropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoByDataTypeAndObjectPropertiesTypeInheritance_PrioritizeMostSpecializedDataTypeFollowedByMostSpecializedObjectPropertiesType()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<A, InheritsFromPropertiesForA>(),
                new PropertyInfo<InheritsFromA, PropertiesForA>(),
                new PropertyInfo<InheritsFromA, InheritsFromPropertiesForA>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<InheritsFromPropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesPropertyInfoDataTypeBase_ReturnMatchedObjectProperties()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<PropertiesForA>(((DynamicPropertyBag)result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties)((DynamicPropertyBag)result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultipleEqualPropertyInfos_ReturnNull()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<A, AlternativePropertiesForA>(),
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            var result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsNull(result);
        }

        #region Nested Types: various test-case classes

        private class A
        {
            
        }

        private class PropertiesForA : IObjectProperties
        {
            public object Data { get; set; }
        }

        private class AlternativePropertiesForA : IObjectProperties
        {
            public object Data { get; set; }
        }

        private class InheritsFromPropertiesForA : PropertiesForA {}

        private class InheritsFromA : A {}

        private class B
        {
            public string Name { get; set; }
        }

        private class PropertiesForB : IObjectProperties
        {
            public object Data { get; set; }
        }

        #endregion
    }
}