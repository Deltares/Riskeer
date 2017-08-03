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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputTest
    {
        [Test]
        public void Constructor_OvertoppingOutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsOutput(
                null,
                new TestDikeHeightOutput(double.NaN),
                new TestOvertoppingRateOutput(double.NaN));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("overtoppingOutput", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_DefaultValues(bool withIllustrationPoints)
        {
            // Setup
            const double waveHeight = 3.2934;
            const double dikeHeight = 7.3892;
            const double requiredProbability = 0.2;
            const double requiredReliability = 0.3;
            const double probability = 0.4;
            const double reliability = 0.1;
            const double factorOfSafety = 0.7;
            const double overtoppingRate = 0.9;

            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(requiredProbability, requiredReliability, probability, reliability, factorOfSafety);
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = withIllustrationPoints
                                                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                                  : null;
            
            var overtoppingOutput = new OvertoppingOutput(waveHeight, true, probabilityAssessmentOutput, generalResult);
            var dikeHeightOutput = new TestDikeHeightOutput(dikeHeight);
            var overtoppingRateOutput = new TestOvertoppingRateOutput(overtoppingRate);

            // Call
            var output = new GrassCoverErosionInwardsOutput(overtoppingOutput, dikeHeightOutput, overtoppingRateOutput);

            // Assert
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.IsInstanceOf<Observable>(output);

            Assert.AreSame(overtoppingOutput, output.OvertoppingOutput);
            Assert.AreSame(probabilityAssessmentOutput, output.OvertoppingOutput.ProbabilityAssessmentOutput);
            Assert.AreSame(generalResult, output.OvertoppingOutput.GeneralResult);
            Assert.AreSame(dikeHeightOutput, output.DikeHeightOutput);
            Assert.AreSame(overtoppingRateOutput, output.OvertoppingRateOutput);
        }
    }
}