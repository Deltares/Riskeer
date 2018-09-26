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
using Core.Common.TestUtil;
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
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] soilModels =
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
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] soilModels =
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

        [TestFixture]
        private class PipingCalculationScenarioContextEqualsTest
            : EqualsTestFixture<PipingCalculationScenarioContext, DerivedPipingCalculationScenarioContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput());
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

            protected override PipingCalculationScenarioContext CreateObject()
            {
                return new PipingCalculationScenarioContext(calculation,
                                                            parent,
                                                            surfaceLines,
                                                            stochasticSoilModels,
                                                            failureMechanism,
                                                            assessmentSection);
            }

            protected override DerivedPipingCalculationScenarioContext CreateDerivedObject()
            {
                return new DerivedPipingCalculationScenarioContext(calculation,
                                                                   parent,
                                                                   surfaceLines,
                                                                   stochasticSoilModels,
                                                                   failureMechanism,
                                                                   assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                                   parent,
                                                                                   surfaceLines,
                                                                                   stochasticSoilModels,
                                                                                   failureMechanism,
                                                                                   assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new PipingCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   surfaceLines,
                                                                                   stochasticSoilModels,
                                                                                   failureMechanism,
                                                                                   assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedPipingCalculationScenarioContext : PipingCalculationScenarioContext
        {
            public DerivedPipingCalculationScenarioContext(PipingCalculationScenario calculation,
                                                           CalculationGroup parent,
                                                           IEnumerable<PipingSurfaceLine> surfaceLines,
                                                           IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                                           PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
                : base(calculation, parent, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }
    }
}