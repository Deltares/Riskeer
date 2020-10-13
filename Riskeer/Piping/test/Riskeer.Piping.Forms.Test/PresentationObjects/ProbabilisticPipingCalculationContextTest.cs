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
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ProbabilisticPipingCalculationContextTest
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
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] soilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };
            var calculation = new ProbabilisticPipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var presentationObject = new ProbabilisticPipingCalculationContext(calculation,
                                                                               parent,
                                                                               surfaceLines,
                                                                               soilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<ProbabilisticPipingCalculation>>(presentationObject);
            Assert.IsInstanceOf<ICalculationContext<ProbabilisticPipingCalculation, PipingFailureMechanism>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(parent, presentationObject.Parent);
            Assert.AreSame(surfaceLines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, presentationObject.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, presentationObject.FailureMechanism);
            Assert.AreSame(assessmentSection, presentationObject.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLines = new[]
            {
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] soilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };
            var calculation = new ProbabilisticPipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => new ProbabilisticPipingCalculationContext(calculation, null, surfaceLines, soilModels, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("parent", exception.ParamName);
            mocks.VerifyAll();
        }

        [TestFixture]
        private class PipingCalculationScenarioContextEqualsTest
            : EqualsTestFixture<ProbabilisticPipingCalculationContext, DerivedProbabilisticPipingCalculationScenarioContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly ProbabilisticPipingCalculation calculation = new ProbabilisticPipingCalculation(new GeneralPipingInput());
            private static readonly IEnumerable<PipingSurfaceLine> surfaceLines = new PipingSurfaceLine[0];
            private static readonly IEnumerable<PipingStochasticSoilModel> stochasticSoilModels = new PipingStochasticSoilModel[0];
            private static readonly PipingFailureMechanism failureMechanism = new PipingFailureMechanism();
            private static readonly CalculationGroup parent = new CalculationGroup();

            [SetUp]
            public void SetUp()
            {
                mocks.ReplayAll();
            }

            [TearDown]
            public void TearDown()
            {
                mocks.VerifyAll();
            }

            protected override ProbabilisticPipingCalculationContext CreateObject()
            {
                return new ProbabilisticPipingCalculationContext(calculation,
                                                                 parent,
                                                                 surfaceLines,
                                                                 stochasticSoilModels,
                                                                 failureMechanism,
                                                                 assessmentSection);
            }

            protected override DerivedProbabilisticPipingCalculationScenarioContext CreateDerivedObject()
            {
                return new DerivedProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                parent,
                                                                                surfaceLines,
                                                                                stochasticSoilModels,
                                                                                failureMechanism,
                                                                                assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new ProbabilisticPipingCalculationContext(new ProbabilisticPipingCalculation(new GeneralPipingInput()),
                                                                                        parent,
                                                                                        surfaceLines,
                                                                                        stochasticSoilModels,
                                                                                        failureMechanism,
                                                                                        assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new ProbabilisticPipingCalculationContext(calculation,
                                                                                        new CalculationGroup(),
                                                                                        surfaceLines,
                                                                                        stochasticSoilModels,
                                                                                        failureMechanism,
                                                                                        assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedProbabilisticPipingCalculationScenarioContext : ProbabilisticPipingCalculationContext
        {
            public DerivedProbabilisticPipingCalculationScenarioContext(ProbabilisticPipingCalculation calculation,
                                                                        CalculationGroup parent,
                                                                        IEnumerable<PipingSurfaceLine> surfaceLines,
                                                                        IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                                                        PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
                : base(calculation, parent, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }
    }
}