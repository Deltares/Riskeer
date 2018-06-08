// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
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
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestFailureMechanismAssemblyCategoriesProperties(null,
                                                                                           assessmentSection,
                                                                                           () => 0.01);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestFailureMechanismAssemblyCategoriesProperties(failureMechanism,
                                                                                           null,
                                                                                           () => 0.01);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetNFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestFailureMechanismAssemblyCategoriesProperties(failureMechanism,
                                                                                           assessmentSection,
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getNFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            const double n = 1.0;

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Contribution = 5;
            var assessmentSection = new AssessmentSectionStub();
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var properties = new TestFailureMechanismAssemblyCategoriesProperties(failureMechanism,
                                                                                      assessmentSection,
                                                                                      () => n);

                // Assert
                Assert.IsInstanceOf<ObjectProperties<IFailureMechanism>>(properties);
                Assert.AreSame(failureMechanism, properties.Data);

                TestHelper.AssertTypeConverter<TestFailureMechanismAssemblyCategoriesProperties, ExpandableArrayConverter>(
                    nameof(TestFailureMechanismAssemblyCategoriesProperties.FailureMechanismAssemblyCategories));
                TestHelper.AssertTypeConverter<TestFailureMechanismAssemblyCategoriesProperties, ExpandableArrayConverter>(
                    nameof(TestFailureMechanismAssemblyCategoriesProperties.FailureMechanismSectionAssemblyCategories));

                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                IEnumerable<FailureMechanismAssemblyCategory> expectedFailureMechanismCategories =
                    AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(
                        failureMechanismContribution.SignalingNorm,
                        failureMechanismContribution.LowerLimitNorm,
                        failureMechanism.Contribution,
                        n);

                AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismAssemblyCategoryProperties(
                    expectedFailureMechanismCategories,
                    new[]
                    {
                        new FailureMechanismSectionAssemblyCategory(0.0, 1.0, FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                    },
                    properties);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = new AssessmentSectionStub();
            mocks.ReplayAll();

            // Call
            var properties = new TestFailureMechanismAssemblyCategoriesProperties(failureMechanism,
                                                                                  assessmentSection,
                                                                                  () => 0.01);
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

        private class TestFailureMechanismAssemblyCategoriesProperties : FailureMechanismAssemblyCategoriesBaseProperties
        {
            public TestFailureMechanismAssemblyCategoriesProperties(IFailureMechanism failureMechanism, IAssessmentSection assessmentSection, Func<double> getNFunc)
                : base(failureMechanism, assessmentSection, getNFunc) {}

            protected override IEnumerable<FailureMechanismSectionAssemblyCategoryProperties> CreateFailureMechanismSectionAssemblyCategories()
            {
                return new[]
                {
                    new FailureMechanismSectionAssemblyCategoryProperties(
                        new FailureMechanismSectionAssemblyCategory(0.0, 1.0, FailureMechanismSectionAssemblyCategoryGroup.IIIv))
                };
            }
        }
    }
}