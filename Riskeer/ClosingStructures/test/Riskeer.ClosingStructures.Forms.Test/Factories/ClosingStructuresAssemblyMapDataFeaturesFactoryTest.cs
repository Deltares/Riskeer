// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.Factories;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Riskeer.ClosingStructures.Forms.Test.Factories
{
    [TestFixture]
    public class ClosingStructuresAssemblyMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateSimpleAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void CreateSimpleAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new ClosingStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Call
                IEnumerable<MapFeature> features = ClosingStructuresAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(failureMechanism);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyMapFeatures(calculator.SimpleAssessmentAssemblyOutput, failureMechanism, features);
            }
        }

        [Test]
        public void CreateDetailedAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateDetailedAssemblyFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(new ClosingStructuresFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void CreateDetailedAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                                                                  random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Call
                IEnumerable<MapFeature> features = ClosingStructuresAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(failureMechanism,
                                                                                                                                  assessmentSection);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyMapFeatures(calculator.DetailedAssessmentAssemblyOutput, failureMechanism, features);
            }
        }

        [Test]
        public void CreateTailorMadeAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateTailorMadeAssemblyFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(new ClosingStructuresFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void CreateTailorMadeAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                                                                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Call
                IEnumerable<MapFeature> features = ClosingStructuresAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(failureMechanism,
                                                                                                                                    assessmentSection);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyMapFeatures(calculator.TailorMadeAssessmentAssemblyOutput, failureMechanism, features);
            }
        }

        [Test]
        public void CreateCombinedAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCombinedAssemblyFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(new ClosingStructuresFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void CreateCombinedAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.CombinedAssemblyOutput = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                                                        random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Call
                IEnumerable<MapFeature> features = ClosingStructuresAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(failureMechanism,
                                                                                                                                  assessmentSection);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyMapFeatures(calculator.CombinedAssemblyOutput, failureMechanism, features);
            }
        }
    }
}