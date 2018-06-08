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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GeotechnicFailureMechanismAssemblyCategoriesPropertiesTest
    {
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
                var properties = new GeotechnicFailureMechanismAssemblyCategoriesProperties(failureMechanism,
                                                                                            assessmentSection,
                                                                                            () => n);

                // Assert
                Assert.IsInstanceOf<FailureMechanismAssemblyCategoriesBaseProperties>(properties);

                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                IEnumerable<FailureMechanismAssemblyCategory> expectedFailureMechanismCategories =
                    AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(
                        failureMechanismContribution.SignalingNorm,
                        failureMechanismContribution.LowerLimitNorm,
                        failureMechanism.Contribution,
                        n);

                IEnumerable<FailureMechanismSectionAssemblyCategory> expectedFailureMechanismSectionCategories =
                    AssemblyToolCategoriesFactory.CreateGeotechnicFailureMechanismSectionAssemblyCategories(
                        failureMechanismContribution.SignalingNorm,
                        failureMechanismContribution.LowerLimitNorm,
                        failureMechanism.Contribution,
                        n);

                AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismAssemblyCategoryProperties(
                    expectedFailureMechanismCategories,
                    expectedFailureMechanismSectionCategories,
                    properties);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_Always_ReturnsExpectedPropertyCount()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = new AssessmentSectionStub();
            mocks.ReplayAll();

            // Call
            var properties = new GeotechnicFailureMechanismAssemblyCategoriesProperties(failureMechanism,
                                                                                        assessmentSection,
                                                                                        () => 0.01);
            // Assert
            Assert.AreEqual(2, PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties).Count);
            mocks.VerifyAll();
        }
    }
}