// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;

namespace Riskeer.Piping.Forms.Test.PropertyClasses.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingProfileSpecificOutputPropertiesTest
    {
        private const int requiredProbabilityPropertyIndex = 0;
        private const int requiredReliabilityPropertyIndex = 1;
        private const int probabilityPropertyIndex = 2;
        private const int reliabilityPropertyIndex = 3;
        private const int factorOfSafetyPropertyIndex = 4;

        private const string resultCategoryName = "\tResultaat";

        [Test]
        public void Constructor_OutputNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingProfileSpecificOutputProperties(null,
                                                                                  new ProbabilisticPipingCalculationScenario(),
                                                                                  new PipingFailureMechanism(),
                                                                                  assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingProfileSpecificOutputProperties(PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(),
                                                                                  null,
                                                                                  new PipingFailureMechanism(),
                                                                                  assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingProfileSpecificOutputProperties(PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(),
                                                                                  new ProbabilisticPipingCalculationScenario(),
                                                                                  null,
                                                                                  assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingProfileSpecificOutputProperties(PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(),
                                                                                  new ProbabilisticPipingCalculationScenario(),
                                                                                  new PipingFailureMechanism(),
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = failureMechanism.SurfaceLines.First()
                }
            };

            // Call
            var properties = new ProbabilisticPipingProfileSpecificOutputProperties(
                output, calculation, failureMechanism, new AssessmentSectionStub());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IPartialProbabilisticPipingOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null);
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = failureMechanism.SurfaceLines.First()
                }
            };

            // Call
            var properties = new ProbabilisticPipingProfileSpecificOutputProperties(
                output, calculation, failureMechanism, new AssessmentSectionStub());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het toetsspoor optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorOfSafetyProperty,
                                                                            resultCategoryName,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = failureMechanism.SurfaceLines.First()
                }
            };

            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();

            // Call
            var properties = new ProbabilisticPipingProfileSpecificOutputProperties(
                output, calculation, failureMechanism, assessmentSection);

            // Assert
            ProbabilityAssessmentOutput expectedProbabilityAssessmentOutput = PipingProbabilityAssessmentOutputFactory.Create(
                output, calculation, failureMechanism, assessmentSection);

            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedProbabilityAssessmentOutput.RequiredProbability), properties.RequiredProbability);
            Assert.AreEqual(expectedProbabilityAssessmentOutput.RequiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedProbabilityAssessmentOutput.Probability), properties.Probability);
            Assert.AreEqual(expectedProbabilityAssessmentOutput.Reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(expectedProbabilityAssessmentOutput.FactorOfSafety, properties.FactorOfSafety, properties.FactorOfSafety.GetAccuracy());
        }
    }
}