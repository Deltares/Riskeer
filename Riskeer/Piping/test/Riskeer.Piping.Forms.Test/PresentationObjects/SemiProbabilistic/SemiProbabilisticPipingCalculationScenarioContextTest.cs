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
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.PresentationObjects.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingCalculationScenarioContextTest
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var presentationObject = new SemiProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                           parent,
                                                                                           surfaceLines,
                                                                                           soilModels,
                                                                                           failureMechanism,
                                                                                           assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<SemiProbabilisticPipingCalculationScenario>>(presentationObject);
            Assert.IsInstanceOf<ICalculationContext<SemiProbabilisticPipingCalculationScenario, PipingFailureMechanism>>(presentationObject);
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
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] soilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };
            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new SemiProbabilisticPipingCalculationScenarioContext(calculation,
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

        [TestFixture]
        private class SemiProbabilisticPipingCalculationScenarioContextEqualsTest
            : EqualsTestFixture<SemiProbabilisticPipingCalculationScenarioContext, DerivedSemiProbabilisticPipingCalculationScenarioContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly SemiProbabilisticPipingCalculationScenario calculation = new SemiProbabilisticPipingCalculationScenario();
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

            protected override SemiProbabilisticPipingCalculationScenarioContext CreateObject()
            {
                return new SemiProbabilisticPipingCalculationScenarioContext(calculation,
                                                                             parent,
                                                                             surfaceLines,
                                                                             stochasticSoilModels,
                                                                             failureMechanism,
                                                                             assessmentSection);
            }

            protected override DerivedSemiProbabilisticPipingCalculationScenarioContext CreateDerivedObject()
            {
                return new DerivedSemiProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                    parent,
                                                                                    surfaceLines,
                                                                                    stochasticSoilModels,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new SemiProbabilisticPipingCalculationScenarioContext(new SemiProbabilisticPipingCalculationScenario(),
                                                                                                    parent,
                                                                                                    surfaceLines,
                                                                                                    stochasticSoilModels,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new SemiProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                                    new CalculationGroup(),
                                                                                                    surfaceLines,
                                                                                                    stochasticSoilModels,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedSemiProbabilisticPipingCalculationScenarioContext : SemiProbabilisticPipingCalculationScenarioContext
        {
            public DerivedSemiProbabilisticPipingCalculationScenarioContext(SemiProbabilisticPipingCalculationScenario calculation,
                                                                            CalculationGroup parent,
                                                                            IEnumerable<PipingSurfaceLine> surfaceLines,
                                                                            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                                                            PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
                : base(calculation, parent, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }
    }
}