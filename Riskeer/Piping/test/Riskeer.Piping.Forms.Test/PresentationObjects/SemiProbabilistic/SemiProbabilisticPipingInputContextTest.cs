﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
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
    public class SemiProbabilisticPipingInputContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new SemiProbabilisticPipingCalculationScenario(new GeneralPipingInput());
            var surfaceLines = new[]
            {
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] stochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };

            var failureMechanism = new PipingFailureMechanism();

            // Call
            var context = new SemiProbabilisticPipingInputContext(calculation.InputParameters, calculation, surfaceLines, stochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<SemiProbabilisticPipingInput>>(context);
            Assert.AreSame(calculation.InputParameters, context.WrappedData);
            Assert.AreSame(calculation, context.PipingCalculation);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            CollectionAssert.AreEqual(surfaceLines, context.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(stochasticSoilModels, context.AvailableStochasticSoilModels);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_CalculationItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationInput = new SemiProbabilisticPipingInput(new GeneralPipingInput());
            var surfaceLines = new[]
            {
                new PipingSurfaceLine(string.Empty)
            };
            PipingStochasticSoilModel[] stochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new SemiProbabilisticPipingInputContext(calculationInput, null, surfaceLines, stochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}