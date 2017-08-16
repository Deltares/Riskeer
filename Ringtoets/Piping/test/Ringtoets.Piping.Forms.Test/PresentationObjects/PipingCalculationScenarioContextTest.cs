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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationScenarioContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLines = new[]
            {
                new PipingSurfaceLine()
            };
            var soilModels = new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var presentationObject = new PipingCalculationScenarioContext(calculation,
                                                                          parent,
                                                                          surfaceLines,
                                                                          soilModels,
                                                                          failureMechanism,
                                                                          assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<PipingCalculationScenario>>(presentationObject);
            Assert.IsInstanceOf<ICalculationContext<PipingCalculationScenario, PipingFailureMechanism>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(parent, presentationObject.Parent);
            Assert.AreSame(surfaceLines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, presentationObject.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, presentationObject.FailureMechanism);
            Assert.AreSame(assessmentSection, presentationObject.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLines = new[]
            {
                new PipingSurfaceLine()
            };
            var soilModels = new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new PipingCalculationScenarioContext(calculation,
                                                                           null,
                                                                           surfaceLines,
                                                                           soilModels,
                                                                           failureMechanism,
                                                                           assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();
            var context = new PipingCalculationScenarioContext(calculationScenario,
                                                               parent,
                                                               new PipingSurfaceLine[0],
                                                               new PipingStochasticSoilModel[0],
                                                               failureMechanism,
                                                               assessmentSection);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();
            var context = new PipingCalculationScenarioContext(calculationScenario,
                                                               parent,
                                                               new PipingSurfaceLine[0],
                                                               new PipingStochasticSoilModel[0],
                                                               failureMechanism,
                                                               assessmentSection);
            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentType_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();
            var context = new PipingCalculationScenarioContext(calculationScenario,
                                                               parent,
                                                               new PipingSurfaceLine[0],
                                                               new PipingStochasticSoilModel[0],
                                                               failureMechanism,
                                                               assessmentSection);
            // Call
            bool isEqual = context.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentWrappedData_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario1 = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationScenario2 = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new PipingCalculationScenarioContext(calculationScenario1,
                                                                parent,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            var context2 = new PipingCalculationScenarioContext(calculationScenario2,
                                                                parent,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            // Precondition
            Assert.IsFalse(calculationScenario1.Equals(calculationScenario2));

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentParent_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent1 = new CalculationGroup();
            var parent2 = new CalculationGroup();
            var context1 = new PipingCalculationScenarioContext(calculationScenario,
                                                                parent1,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            var context2 = new PipingCalculationScenarioContext(calculationScenario,
                                                                parent2,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            // Precondition
            Assert.IsFalse(parent1.Equals(parent2));

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithSameWrappedDataAndParent_ReturnTrue()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new PipingCalculationScenarioContext(calculationScenario,
                                                                parent,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            var context2 = new PipingCalculationScenarioContext(calculationScenario,
                                                                parent,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new PipingCalculationScenarioContext(calculationScenario,
                                                                parent,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            var context2 = new PipingCalculationScenarioContext(calculationScenario,
                                                                parent,
                                                                new PipingSurfaceLine[0],
                                                                new PipingStochasticSoilModel[0],
                                                                failureMechanism,
                                                                assessmentSection);
            // Precondition
            Assert.AreEqual(context1, context2);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);

            mocksRepository.VerifyAll();
        }
    }
}