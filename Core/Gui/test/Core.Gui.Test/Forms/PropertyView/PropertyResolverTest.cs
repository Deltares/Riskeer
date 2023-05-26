﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.TestUtil;
using Core.Gui.Forms.PropertyView;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.PropertyView
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
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            // Call
            object result = resolver.GetObjectProperties(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetObjectProperties_DataMatchesSinglePropertyInfoDataTypeDirectly_ReturnNewInstanceOfCorrespondingObjectProperties()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<PropertiesForA>(((DynamicPropertyBag) result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties) ((DynamicPropertyBag) result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesInfoWithCreateInstanceForCustomData_ReturnObjectPropertiesWithDataFromInjectedDelegate()
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
                    CreateInstance = a => new PropertiesForA
                    {
                        Data = otherObject
                    }
                }
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            var bag = (DynamicPropertyBag) result;
            var properties = (PropertiesForA) bag.WrappedObject;
            Assert.AreSame(otherObject, properties.Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoByDataTypeInheritance_PrioritizeMostSpecialized()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<InheritsFromA, AlternativePropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<AlternativePropertiesForA>(((DynamicPropertyBag) result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties) ((DynamicPropertyBag) result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultiplePropertyInfoByObjectPropertiesTypeInheritance_PrioritizeMostSpecialized()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<A, InheritsFromPropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new A();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<InheritsFromPropertiesForA>(((DynamicPropertyBag) result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties) ((DynamicPropertyBag) result).WrappedObject).Data);
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
                new PropertyInfo<InheritsFromA, InheritsFromPropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<InheritsFromPropertiesForA>(((DynamicPropertyBag) result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties) ((DynamicPropertyBag) result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesPropertyInfoDataTypeBase_ReturnMatchedObjectProperties()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsInstanceOf<DynamicPropertyBag>(result);
            Assert.IsInstanceOf<PropertiesForA>(((DynamicPropertyBag) result).WrappedObject);
            Assert.AreSame(source, ((IObjectProperties) ((DynamicPropertyBag) result).WrappedObject).Data);
        }

        [Test]
        public void GetObjectProperties_DataMatchesMultipleEqualPropertyInfos_ReturnNull()
        {
            // Setup
            var propertyInfos = new PropertyInfo[]
            {
                new PropertyInfo<A, PropertiesForA>(),
                new PropertyInfo<A, AlternativePropertiesForA>()
            };

            var resolver = new PropertyResolver(propertyInfos);

            var source = new InheritsFromA();

            // Call
            object result = resolver.GetObjectProperties(source);

            // Assert
            Assert.IsNull(result);
        }

        #region Nested Types: various test-case classes

        private class A {}

        private class PropertiesForA : ObjectProperties<object> {}

        private class AlternativePropertiesForA : ObjectProperties<object> {}

        private class InheritsFromPropertiesForA : PropertiesForA {}

        private class InheritsFromA : A {}

        private class B
        {
            public string Name { get; set; }
        }

        #endregion
    }
}