using System.Collections.Generic;
using DelftTools.Shell.Gui;
using DelftTools.Utils.PropertyBag.Dynamic;
using DeltaShell.Gui.Forms.PropertyGridView;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms.PropertyGridView
{
    [TestFixture]
    public class PropertyResolverTest
    {
        # region GetObjectProperties tests

        [Test]
        public void GetObjectProperties_WhenNoPropertyInfoIsFound_ReturnNull()
        {
            // Assert
            Assert.IsNull(PropertyResolver.GetObjectProperties(new List<PropertyInfo>(), 1.0));
        }

        [Test]
        public void GetObjectProperties_WhenOnePropertyInfoIsFound_ReturnDynamicPropertyBagContainingOnlyThatPropertiesObject()
        {
            // Setup
            var propertyInfos = new List<PropertyInfo>
            {
                new PropertyInfo<A, SimpleProperties<A>>()
            };

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new A());

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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new A());

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
                new PropertyInfo<C, SimpleProperties<C>>() // specifically for C
            };

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new C()); //we ask for C

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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new D());

            // Setup
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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new D());

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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new D());

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
                    AdditionalDataCheck = o => true
                }, // Additional data check which will be matched
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            };

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new B());

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
                    AdditionalDataCheck = o => false
                }, // Additional data check which will not be matched
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            };

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new B());

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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new B());

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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new B());

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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new B());
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

            // Call
            var objectProperties = PropertyResolver.GetObjectProperties(propertyInfos, new C());

            // Assert
            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(DerivedSimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        # endregion

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

        internal class A {}

        private class B : A {}

        internal class C : A {}

        private class D : C {}

        internal class SimpleProperties<T> : ObjectProperties<T> {}

        private class DerivedSimpleProperties<T> : SimpleProperties<T> {}

        private class OtherSimpleProperties<T> : ObjectProperties<T> {}

        # endregion
    }
}