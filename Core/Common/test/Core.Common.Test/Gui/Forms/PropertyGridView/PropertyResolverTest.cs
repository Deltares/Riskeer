using System;
using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Properties;
using Core.Common.Utils.PropertyBag;

using NUnit.Framework;

namespace Core.Common.Test.Gui.Forms.PropertyGridView
{
    [TestFixture]
    public class PropertyResolverTest
    {
        [Test]
        public void Constructor_WithoutPropertyInfoList_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PropertyResolver(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.PropertyResolver_PropertyResolver_Cannot_create_PropertyResolver_without_list_of_PropertyInfo, message);
            StringAssert.EndsWith("propertyInfos", message);
        }

        [Test]
        public void Constructor_WithParams_NewInstance()
        {
            // Call
            var result = new PropertyResolver(new List<PropertyInfo>());

            // Assert
            Assert.IsInstanceOf<PropertyResolver>(result);
        }

        [Test]
        public void GetObjectProperties_WhenNoPropertyInfoIsFound_ReturnNull()
        {
            // Setup
            var resolver = new PropertyResolver(new List<PropertyInfo>());

            // Assert
            Assert.IsNull(resolver.GetObjectProperties(1.0));
        }

        [Test]
        public void GetObjectProperties_WhenOnePropertyInfoIsFound_ReturnDynamicPropertyBagContainingOnlyThatPropertiesObject()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new A());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<A>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_WhenOnePropertyInfoIsFoundButAdditionalChecksFail_ReturnNull()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>()
                {
                    AdditionalDataCheck = o => false
                }
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new A());

            // Assert
            Assert.IsNull(objectProperties);
        }

        [Test]
        public void GetObjectProperties_WhenTwoPropertyInfoAreFoundOneWithAdditionalCheckOneWithBetterType_ReturnPropertiesObjectMatchingAdditionCheck()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>()
                {
                    AdditionalDataCheck = o => false
                },
                new PropertyInfo<C, SimpleProperties<C>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new C());

            // Assert
            Assert.AreSame(typeof(SimpleProperties<C>),
                           ((DynamicPropertyBag) objectProperties).GetContentType(), "we got A, expected C");
        }

        [Test]
        public void GetObjectProperties_BasedOnDirectObjectTypeMatch_ReturnObjectPropertiesMatchingTypeD()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>(),
                new PropertyInfo<D, SimpleProperties<D>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new D());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<D>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedObjectTypeMatch_ReturnObjectPropertiesForBaseClass()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>(),
                new PropertyInfo<C, SimpleProperties<C>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new D());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedObjectTypeMatchAndAdditionalDataCheck_ReturnObjectPropertiesForBaseClassMatchingAdditionCheck()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>
                {
                    AdditionalDataCheck = o => true
                },
                new PropertyInfo<C, SimpleProperties<C>>
                {
                    AdditionalDataCheck = o => true
                }
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new D());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_BasedOnMatchingAdditionalDataCheck_ReturnMatchingWithAdditionalDataCheck()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => true // Additional data check which will be matched
                },
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new B());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_BasedOnMismatchingAdditionalDataCheck_ReturnFallBackPropertiesObject()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => false // Additional data check which will not be matched
                },
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new B());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(OtherSimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedPropertyTypeMatch_ReturnDerivedObjectPropertiesClass()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<B, SimpleProperties<B>>(),
                new PropertyInfo<B, DerivedSimpleProperties<B>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new B());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(DerivedSimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedPropertyTypeMatchAndAdditionalDataCheck_ReturnDerivedObjectProperties()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => true
                },
                new PropertyInfo<B, DerivedSimpleProperties<B>>
                {
                    AdditionalDataCheck = o => true
                }
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new B());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(DerivedSimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void GetObjectProperties_WhenMultiplePropertyObjectsAreFound_ReturnNull()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<B, SimpleProperties<B>>(),
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new B());

            // Assert
            Assert.IsNull(objectProperties);
        }

        [Test]
        public void GetObjectProperties_TakingAllPropertyInfoPropertiesIntoAccount_ReturnDerivedObjectPropertiesMatchingDataCheck()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<D, DerivedSimpleProperties<D>>
                {
                    AdditionalDataCheck = o => true
                }, // D is not assignable from C: no candidate
                new PropertyInfo<A, DerivedSimpleProperties<A>>
                {
                    AdditionalDataCheck = o => true
                }, // A is less specific than C: candidate but not the most specific one
                new PropertyInfo<C, DerivedSimpleProperties<C>>
                {
                    AdditionalDataCheck = o => false
                }, // Additional data check is false: no candidate
                new PropertyInfo<C, SimpleProperties<C>>
                {
                    AdditionalDataCheck = o => true
                }, // SimpleProperties is less specific than DerivedSimpleProperties: candidate but not the most specific one
                new PropertyInfo<C, DerivedSimpleProperties<C>>
                {
                    AdditionalDataCheck = o => true
                } // Most specific!
            };
            var resolver = new PropertyResolver(propertyInfos);

            // Call
            var objectProperties = resolver.GetObjectProperties(new C());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(DerivedSimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        # region Nested types

        /*
         *          A
         *       ___^___
         *       |     |
         *       B     C
         *             |
         *             D
         * 
         * 
         * 
         *      SimpleProperties                 OtherSimpleProperties
         *             ^
         *             |
         *   DerivedSimpleProperties
         * 
         */

        private class A {}

        private class B : A {}

        private class C : A {}

        private class D : C {}

        private class SimpleProperties<T> : ObjectProperties<T> {}

        private class DerivedSimpleProperties<T> : SimpleProperties<T> {}

        private class OtherSimpleProperties<T> : ObjectProperties<T> {}

        # endregion
    }
}