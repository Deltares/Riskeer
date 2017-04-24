﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
            var dikeHeightOutput = new DikeHeightOutput(random.NextDouble(), random.NextDouble(),
                                                        random.NextDouble(), random.NextDouble(),
                                                        random.NextDouble(), dikeHeightConvergence);
            var overtoppingRateOutput = new OvertoppingRateOutput(random.NextDouble(), random.NextDouble(),
                                                                  random.NextDouble(), random.NextDouble(),
                                                                  random.NextDouble(), overtoppingRateConvergence);
            var output = new GrassCoverErosionInwardsOutput(random.NextDouble(), false, probabilityAssessmentOutput,
                                                            dikeHeightOutput, overtoppingRateOutput);

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
            Assert.AreEqual(dikeHeightOutput.Result, dikeHeightEntity.DikeHeight, dikeHeightOutput.Result.GetAccuracy());
            Assert.AreEqual(dikeHeightOutput.TargetProbability, dikeHeightEntity.TargetProbability);
            Assert.AreEqual(dikeHeightOutput.TargetReliability, dikeHeightEntity.TargetReliability, dikeHeightOutput.TargetReliability.GetAccuracy());
            Assert.AreEqual(dikeHeightOutput.CalculatedProbability, dikeHeightEntity.CalculatedProbability);
            Assert.AreEqual(dikeHeightOutput.CalculatedReliability, dikeHeightEntity.CalculatedReliability, dikeHeightOutput.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) dikeHeightOutput.CalculationConvergence, dikeHeightEntity.CalculationConvergence);

            GrassCoverErosionInwardsOvertoppingRateOutputEntity overtoppingRateEntity = entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.First();
            Assert.AreEqual(overtoppingRateOutput.Result, overtoppingRateEntity.OvertoppingRate, overtoppingRateOutput.Result.GetAccuracy());
            Assert.AreEqual(overtoppingRateOutput.TargetProbability, overtoppingRateEntity.TargetProbability);
            Assert.AreEqual(overtoppingRateOutput.TargetReliability, overtoppingRateEntity.TargetReliability, overtoppingRateOutput.TargetReliability.GetAccuracy());
            Assert.AreEqual(overtoppingRateOutput.CalculatedProbability, overtoppingRateEntity.CalculatedProbability);
            Assert.AreEqual(overtoppingRateOutput.CalculatedReliability, overtoppingRateEntity.CalculatedReliability, overtoppingRateOutput.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) overtoppingRateOutput.CalculationConvergence, overtoppingRateEntity.CalculationConvergence);
        }

        [Test]
        public void Create_NaNValues_ReturnGrassCoverErosionInwardsOutputEntity()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var dikeHeightOutput = new TestDikeHeightOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var overtoppingRateOutput = new TestOvertoppingRateOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var output = new GrassCoverErosionInwardsOutput(double.NaN, true, probabilityAssessmentOutput, dikeHeightOutput, overtoppingRateOutput);

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
            Assert.AreEqual((byte) dikeHeightOutput.CalculationConvergence, dikeHeightEntity.CalculationConvergence);

            GrassCoverErosionInwardsOvertoppingRateOutputEntity overtoppingRateEntity = entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.First();
            Assert.IsNull(overtoppingRateEntity.OvertoppingRate);
            Assert.IsNull(overtoppingRateEntity.TargetProbability);
            Assert.IsNull(overtoppingRateEntity.TargetReliability);
            Assert.IsNull(overtoppingRateEntity.CalculatedProbability);
            Assert.IsNull(overtoppingRateEntity.CalculatedReliability);
            Assert.AreEqual((byte) overtoppingRateOutput.CalculationConvergence, overtoppingRateEntity.CalculationConvergence);
        }

        [Test]
        public void Create_DikeHeightOutputIsNull_NoDikeHeightOutputEntityCreated()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            var overtoppingRateOutput = new TestOvertoppingRateOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, null, overtoppingRateOutput);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsFalse(entity.GrassCoverErosionInwardsDikeHeightOutputEntities.Any());
        }

        [Test]
        public void Create_OvertoppingRateOutputIsNull_NoOvertoppingRateOutputEntityCreated()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            var dikeHeightOutput = new TestDikeHeightOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, dikeHeightOutput, null);

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create();

            // Assert
            Assert.IsFalse(entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.Any());
        }
    }
}