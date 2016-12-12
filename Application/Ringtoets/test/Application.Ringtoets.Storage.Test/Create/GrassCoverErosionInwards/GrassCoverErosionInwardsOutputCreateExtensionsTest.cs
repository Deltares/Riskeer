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

using System;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputCreateExtensionsTest
    {
        [Test]
        public void Create_ValidInput_ReturnGrassCoverErosionInwardsOutputEntity()
        {
            // Setup
            var random = new Random(456);
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(random.NextDouble(), random.NextDouble(),
                                                                              random.NextDouble(), random.NextDouble(),
                                                                              random.NextDouble());
            var dikeHeightConvergence = (CalculationConvergence) random.Next(1, Enum.GetValues(typeof(CalculationConvergence)).Length + 1);
            var dikeHeightAssessmentOutput = new DikeHeightAssessmentOutput(random.NextDouble(), random.NextDouble(),
                                                                            random.NextDouble(), random.NextDouble(),
                                                                            random.NextDouble(), dikeHeightConvergence);
            var output = new GrassCoverErosionInwardsOutput(random.NextDouble(), false, probabilityAssessmentOutput, dikeHeightAssessmentOutput);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.AreEqual(output.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(Convert.ToByte(output.DikeHeightCalculated), entity.IsDikeHeightCalculated);
            Assert.AreEqual(output.WaveHeight.Value, entity.WaveHeight);
            Assert.AreEqual(Convert.ToByte(output.IsOvertoppingDominant), entity.IsOvertoppingDominant);

            Assert.AreEqual(probabilityAssessmentOutput.FactorOfSafety.Value, entity.FactorOfSafety);
            Assert.AreEqual(probabilityAssessmentOutput.Probability, entity.Probability);
            Assert.AreEqual(probabilityAssessmentOutput.Reliability.Value, entity.Reliability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredProbability, entity.RequiredProbability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredReliability.Value, entity.RequiredReliability);
        }

        [Test]
        public void Create_NaNValues_ReturnGrassCoverErosionInwardsOutputEntity()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var dikeHeightAssessmentOutput = new DikeHeightAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, CalculationConvergence.CalculatedConverged);
            var output = new GrassCoverErosionInwardsOutput(double.NaN, true, probabilityAssessmentOutput, dikeHeightAssessmentOutput);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsNull(entity.DikeHeight);
            Assert.IsNull(entity.WaveHeight);
            Assert.AreEqual(Convert.ToByte(output.IsOvertoppingDominant), entity.IsOvertoppingDominant);
            Assert.AreEqual(Convert.ToByte(true), entity.IsDikeHeightCalculated);
            Assert.IsNull(entity.FactorOfSafety);
            Assert.IsNull(entity.Probability);
            Assert.IsNull(entity.Reliability);
            Assert.IsNull(entity.RequiredProbability);
            Assert.IsNull(entity.RequiredReliability);
        }

        [Test]
        public void Create_DikeHeightIsNull_PersistDikeHeightAsNullToRepresentNaN()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, null);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsNull(entity.DikeHeight);
            Assert.AreEqual(Convert.ToByte(false), entity.IsDikeHeightCalculated);
        }
    }
}