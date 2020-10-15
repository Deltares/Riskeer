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

using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();

            // Call
            var inputParameters = new SemiProbabilisticPipingInput(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<PipingInput>(inputParameters);

            Assert.IsNaN(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);
        }

        [Test]
        public void AssessmentLevel_SetToNew_ValueIsRounded()
        {
            // Setup
            const double assessmentLevel = 1.111111;
            var input = new SemiProbabilisticPipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = input.AssessmentLevel.NumberOfDecimalPlaces;

            // Call
            input.AssessmentLevel = (RoundedDouble) assessmentLevel;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(assessmentLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new SemiProbabilisticPipingInput(new GeneralPipingInput());

            PipingTestDataGenerator.SetRandomDataToPipingInput(original);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new SemiProbabilisticPipingInput(new GeneralPipingInput());

            PipingTestDataGenerator.SetRandomDataToPipingInput(original);

            original.SurfaceLine = null;
            original.StochasticSoilModel = null;
            original.StochasticSoilProfile = null;
            original.HydraulicBoundaryLocation = null;

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }
    }
}