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
using System.Linq;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;

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
            var dikeHeightConvergence = random.NextEnumValue<CalculationConvergence>();
            var overtoppingRateConvergence = random.NextEnumValue<CalculationConvergence>();
            var dikeHeightAssessmentOutput = new SubCalculationAssessmentOutput(random.NextDouble(), random.NextDouble(),
                                                                                random.NextDouble(), random.NextDouble(),
                                                                                random.NextDouble(), dikeHeightConvergence);
            var overtoppingRateAssessmentOutput = new SubCalculationAssessmentOutput(random.NextDouble(), random.NextDouble(),
                                                                                     random.NextDouble(), random.NextDouble(),
                                                                                     random.NextDouble(), overtoppingRateConvergence);
            var output = new GrassCoverErosionInwardsOutput(random.NextDouble(), false, probabilityAssessmentOutput,
                                                            dikeHeightAssessmentOutput, overtoppingRateAssessmentOutput);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.AreEqual(output.WaveHeight.Value, entity.WaveHeight);
            Assert.AreEqual(Convert.ToByte(output.IsOvertoppingDominant), entity.IsOvertoppingDominant);

            Assert.AreEqual(probabilityAssessmentOutput.FactorOfSafety, entity.FactorOfSafety, probabilityAssessmentOutput.FactorOfSafety.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.Probability, entity.Probability);
            Assert.AreEqual(probabilityAssessmentOutput.Reliability, entity.Reliability, probabilityAssessmentOutput.Reliability.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.RequiredProbability, entity.RequiredProbability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredReliability.Value, entity.RequiredReliability);

            GrassCoverErosionInwardsDikeHeightOutputEntity dikeHeightEntity = entity.GrassCoverErosionInwardsDikeHeightOutputEntities.First();
            Assert.AreEqual(dikeHeightAssessmentOutput.Result, dikeHeightEntity.DikeHeight, dikeHeightAssessmentOutput.Result.GetAccuracy());
            Assert.AreEqual(dikeHeightAssessmentOutput.TargetProbability, dikeHeightEntity.TargetProbability);
            Assert.AreEqual(dikeHeightAssessmentOutput.TargetReliability, dikeHeightEntity.TargetReliability, dikeHeightAssessmentOutput.TargetReliability.GetAccuracy());
            Assert.AreEqual(dikeHeightAssessmentOutput.CalculatedProbability, dikeHeightEntity.CalculatedProbability);
            Assert.AreEqual(dikeHeightAssessmentOutput.CalculatedReliability, dikeHeightEntity.CalculatedReliability, dikeHeightAssessmentOutput.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) dikeHeightAssessmentOutput.CalculationConvergence, dikeHeightEntity.CalculationConvergence);

            GrassCoverErosionInwardsOvertoppingRateOutputEntity overtoppingRateEntity = entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.First();
            Assert.AreEqual(overtoppingRateAssessmentOutput.Result, overtoppingRateEntity.OvertoppingRate, overtoppingRateAssessmentOutput.Result.GetAccuracy());
            Assert.AreEqual(overtoppingRateAssessmentOutput.TargetProbability, overtoppingRateEntity.TargetProbability);
            Assert.AreEqual(overtoppingRateAssessmentOutput.TargetReliability, overtoppingRateEntity.TargetReliability, overtoppingRateAssessmentOutput.TargetReliability.GetAccuracy());
            Assert.AreEqual(overtoppingRateAssessmentOutput.CalculatedProbability, overtoppingRateEntity.CalculatedProbability);
            Assert.AreEqual(overtoppingRateAssessmentOutput.CalculatedReliability, overtoppingRateEntity.CalculatedReliability, overtoppingRateAssessmentOutput.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) overtoppingRateAssessmentOutput.CalculationConvergence, overtoppingRateEntity.CalculationConvergence);
        }

        [Test]
        public void Create_NaNValues_ReturnGrassCoverErosionInwardsOutputEntity()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var dikeHeightAssessmentOutput = new TestSubCalculationAssessmentOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var overtoppingRateAssessmentOutput = new TestSubCalculationAssessmentOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var output = new GrassCoverErosionInwardsOutput(double.NaN, true, probabilityAssessmentOutput, dikeHeightAssessmentOutput, overtoppingRateAssessmentOutput);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsNull(entity.WaveHeight);
            Assert.AreEqual(Convert.ToByte(output.IsOvertoppingDominant), entity.IsOvertoppingDominant);
            Assert.IsNull(entity.FactorOfSafety);
            Assert.IsNull(entity.Probability);
            Assert.IsNull(entity.Reliability);
            Assert.IsNull(entity.RequiredProbability);
            Assert.IsNull(entity.RequiredReliability);

            GrassCoverErosionInwardsDikeHeightOutputEntity dikeHeightEntity = entity.GrassCoverErosionInwardsDikeHeightOutputEntities.First();
            Assert.IsNull(dikeHeightEntity.DikeHeight);
            Assert.IsNull(dikeHeightEntity.TargetProbability);
            Assert.IsNull(dikeHeightEntity.TargetReliability);
            Assert.IsNull(dikeHeightEntity.CalculatedProbability);
            Assert.IsNull(dikeHeightEntity.CalculatedReliability);
            Assert.AreEqual((byte) dikeHeightAssessmentOutput.CalculationConvergence, dikeHeightEntity.CalculationConvergence);

            GrassCoverErosionInwardsOvertoppingRateOutputEntity overtoppingRateEntity = entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.First();
            Assert.IsNull(overtoppingRateEntity.OvertoppingRate);
            Assert.IsNull(overtoppingRateEntity.TargetProbability);
            Assert.IsNull(overtoppingRateEntity.TargetReliability);
            Assert.IsNull(overtoppingRateEntity.CalculatedProbability);
            Assert.IsNull(overtoppingRateEntity.CalculatedReliability);
            Assert.AreEqual((byte) overtoppingRateAssessmentOutput.CalculationConvergence, overtoppingRateEntity.CalculationConvergence);
        }

        [Test]
        public void Create_DikeHeightAssessmentOutputIsNull_NoDikeHeightOutputEntityCreated()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, null, null);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsFalse(entity.GrassCoverErosionInwardsDikeHeightOutputEntities.Any());
        }

        [Test]
        public void Create_OvertoppingRateAssessmentOutputIsNull_NoOvertoppingRateOutputEntityCreated()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, null, null);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsFalse(entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.Any());
        }
    }
}