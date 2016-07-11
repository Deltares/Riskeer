// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            double waveHeight = 3.2934;
            bool isOvertoppingDominant = true;
            double dikeHeight = 7.3892;
            double requiredProbability = 0.2;
            double requiredReliability = 0.3;
            double probability = 0.4;
            double reliability = 0.1;
            double factorOfSafety = 0.7;

            ProbabilityAssessmentOutput probabilityAssessmentOutput = new ProbabilityAssessmentOutput(requiredProbability, requiredReliability, probability, reliability, factorOfSafety);

            // Call
            GrassCoverErosionInwardsOutput output = new GrassCoverErosionInwardsOutput(waveHeight, isOvertoppingDominant, probabilityAssessmentOutput, dikeHeight, true);

            // Assert
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.IsInstanceOf<Observable>(output);
            Assert.AreEqual(new RoundedDouble(2, waveHeight), output.WaveHeight);
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(isOvertoppingDominant, output.IsOvertoppingDominant);
            Assert.AreEqual(new RoundedDouble(2, dikeHeight), output.DikeHeight);
            Assert.AreEqual(2, output.DikeHeight.NumberOfDecimalPlaces);
            Assert.IsTrue(output.DikeHeightCalculated);
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
            Assert.AreEqual(requiredReliability, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(probability, output.Probability);
            Assert.AreEqual(reliability, output.Reliability, output.Reliability.GetAccuracy());
            Assert.AreEqual(factorOfSafety, output.FactorOfSafety, output.FactorOfSafety.GetAccuracy());
        }
    }
}