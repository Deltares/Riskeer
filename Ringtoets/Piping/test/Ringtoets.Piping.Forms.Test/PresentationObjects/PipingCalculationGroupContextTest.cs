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
    public class PipingCalculationGroupContextTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_ExpectedValues(bool withParent)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var surfaceLines = new[]
            {
                new PipingSurfaceLine(string.Empty)
            };
            var soilModels = new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };

            CalculationGroup parent = withParent ? new CalculationGroup() : null;

            var failureMechanism = new PipingFailureMechanism();

            // Call
            var groupContext = new PipingCalculationGroupContext(calculationGroup, parent, surfaceLines, soilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<CalculationGroup>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, PipingFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(parent, groupContext.Parent);
            Assert.AreSame(surfaceLines, groupContext.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, groupContext.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingCalculationGroupContext(calculationGroup,
                                                            parent,
                                                            new PipingSurfaceLine[0],
                                                            new PipingStochasticSoilModel[0],
                                                            failureMechanism,
                                                            assessmentSection);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);

            mocks.VerifyAll();
        }

        [TestFixture]
        private class PipingCalculationGroupContextEqualsTest
            : EqualsTestFixture<PipingCalculationGroupContext, DerivedPipingCalculationGroupContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly IEnumerable<PipingSurfaceLine> surfaceLines = new PipingSurfaceLine[0];
            private static readonly IEnumerable<PipingStochasticSoilModel> stochasticSoilModels = new PipingStochasticSoilModel[0];
            private static readonly PipingFailureMechanism failureMechanism = new PipingFailureMechanism();
            private static readonly CalculationGroup parent = new CalculationGroup();
            private static readonly CalculationGroup calculationGroup = new CalculationGroup();

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

            protected override PipingCalculationGroupContext CreateObject()
            {
                return new PipingCalculationGroupContext(calculationGroup,
                                                         parent,
                                                         surfaceLines,
                                                         stochasticSoilModels,
                                                         failureMechanism,
                                                         assessmentSection);
            }

            protected override DerivedPipingCalculationGroupContext CreateDerivedObject()
            {
                return new DerivedPipingCalculationGroupContext(calculationGroup,
                                                                parent,
                                                                surfaceLines,
                                                                stochasticSoilModels,
                                                                failureMechanism,
                                                                assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new PipingCalculationGroupContext(new CalculationGroup(),
                                                                                parent,
                                                                                surfaceLines,
                                                                                stochasticSoilModels,
                                                                                failureMechanism,
                                                                                assessmentSection))
                    .SetName("Wrapped Calculation Group");
                yield return new TestCaseData(new PipingCalculationGroupContext(calculationGroup,
                                                                                new CalculationGroup(),
                                                                                surfaceLines,
                                                                                stochasticSoilModels,
                                                                                failureMechanism,
                                                                                assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedPipingCalculationGroupContext : PipingCalculationGroupContext
        {
            public DerivedPipingCalculationGroupContext(CalculationGroup calculationGroup,
                                                        CalculationGroup parent,
                                                        IEnumerable<PipingSurfaceLine> surfaceLines,
                                                        IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                                        PipingFailureMechanism pipingFailureMechanism,
                                                        IAssessmentSection assessmentSection)
                : base(calculationGroup, parent, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }
    }
}