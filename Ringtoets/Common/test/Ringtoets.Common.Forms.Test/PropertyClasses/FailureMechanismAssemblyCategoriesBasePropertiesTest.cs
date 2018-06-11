﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoriesBasePropertiesTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoriesBaseProperties(null,
                                                                                           GetFailureMechanismAssemblyCategory,
                                                                                           GetFailureMechanismSectionAssemblyCategory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_GetFailureMechanismAssemblyCategoryFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoriesBaseProperties(failureMechanism,
                                                                                           null,
                                                                                           GetFailureMechanismSectionAssemblyCategory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getFailureMechanismAssemblyCategoryFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetFailureMechanismSectionAssemblyCategoryFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoriesBaseProperties(failureMechanism,
                                                                                           GetFailureMechanismAssemblyCategory,
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getFailureMechanismSectionAssemblyCategoryFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var properties = new FailureMechanismAssemblyCategoriesBaseProperties(failureMechanism,
                                                                                  GetFailureMechanismAssemblyCategory,
                                                                                  GetFailureMechanismSectionAssemblyCategory);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);

            TestHelper.AssertTypeConverter<FailureMechanismAssemblyCategoriesBaseProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismAssemblyCategoriesBaseProperties.FailureMechanismAssemblyCategories));
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyCategoriesBaseProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismAssemblyCategoriesBaseProperties.FailureMechanismSectionAssemblyCategories));

            AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismAssemblyCategoryProperties(
                GetFailureMechanismAssemblyCategory(),
                GetFailureMechanismSectionAssemblyCategory(),
                properties);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var properties = new FailureMechanismAssemblyCategoriesBaseProperties(failureMechanism,
                                                                                  GetFailureMechanismAssemblyCategory,
                                                                                  GetFailureMechanismSectionAssemblyCategory);
            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor failureMechanismCategoriesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureMechanismCategoriesProperty,
                                                                            generalCategoryName,
                                                                            "Categoriegrenzen per traject",
                                                                            "De categoriegrenzen per traject voor dit toetsspoor.",
                                                                            true);

            PropertyDescriptor failureMechanismSectionCategoriesProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureMechanismSectionCategoriesProperty,
                                                                            generalCategoryName,
                                                                            "Categoriegrenzen per vak",
                                                                            "De categoriegrenzen per vak voor dit toetsspoor.",
                                                                            true);

            mocks.VerifyAll();
        }

        private IEnumerable<FailureMechanismSectionAssemblyCategory> GetFailureMechanismSectionAssemblyCategory()
        {
            var random = new Random(21);

            yield return new FailureMechanismSectionAssemblyCategory(random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
        }

        private IEnumerable<FailureMechanismAssemblyCategory> GetFailureMechanismAssemblyCategory()
        {
            var random = new Random(21);

            yield return new FailureMechanismAssemblyCategory(random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
        }
    }
}