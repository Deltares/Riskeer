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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryIsNull_ThrowArgumenNullException()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1.0, 1.0, 1.0, 1.0, 1.0);
            var output = new GrassCoverErosionInwardsOutput(1.1, false, probabilityAssessmentOutput, 2.2);

            // Call
            TestDelegate call = () => output.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_ValidInput_ReturnGrassCoverErosionInwardsOutputEntity()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(0.005, 1.0, 0.06, 0.8, 0.5);
            var output = new GrassCoverErosionInwardsOutput(1.1, false, probabilityAssessmentOutput, 2.2);

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create(registry);

            // Assert
            Assert.AreEqual(output.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(Convert.ToByte(output.DikeHeightCalculated), entity.IsDikeHeightCalculated);
            Assert.AreEqual(output.WaveHeight.Value, entity.WaveHeight);
            Assert.AreEqual(Convert.ToByte(output.IsOvertoppingDominant), entity.IsOvertoppingDominant);

            Assert.AreEqual(probabilityAssessmentOutput.FactorOfSafety.Value, entity.ProbabilisticOutputEntity.FactorOfSafety);
            Assert.AreEqual(probabilityAssessmentOutput.Probability, entity.ProbabilisticOutputEntity.Probability);
            Assert.AreEqual(probabilityAssessmentOutput.Reliability.Value, entity.ProbabilisticOutputEntity.Reliability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredProbability, entity.ProbabilisticOutputEntity.RequiredProbability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredReliability.Value, entity.ProbabilisticOutputEntity.RequiredReliability);
        }

        [Test]
        public void Create_NaNValues_ReturnGrassCoverErosionInwardsOutputEntity()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var output = new GrassCoverErosionInwardsOutput(double.NaN, true, probabilityAssessmentOutput, double.NaN);

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create(registry);

            // Assert
            Assert.IsNull(entity.DikeHeight);
            Assert.IsNull(entity.WaveHeight);
            Assert.AreEqual(Convert.ToByte(output.IsOvertoppingDominant), entity.IsOvertoppingDominant);
            Assert.AreEqual(Convert.ToByte(true), entity.IsDikeHeightCalculated);
            Assert.IsNull(entity.ProbabilisticOutputEntity.FactorOfSafety);
            Assert.IsNull(entity.ProbabilisticOutputEntity.Probability);
            Assert.IsNull(entity.ProbabilisticOutputEntity.Reliability);
            Assert.IsNull(entity.ProbabilisticOutputEntity.RequiredProbability);
            Assert.IsNull(entity.ProbabilisticOutputEntity.RequiredReliability);
        }

        [Test]
        public void Create_DikeHeightIsNull_PersistDikeHeightAsNullToRepresentNaN()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, null);

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create(registry);

            // Assert
            Assert.IsNull(entity.DikeHeight);
            Assert.AreEqual(Convert.ToByte(false), entity.IsDikeHeightCalculated);
        }

        [Test]
        public void Create_ValidValues_RegisterEntity()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(0.005, 1.0, 0.06, 0.8, 0.5);
            var output = new GrassCoverErosionInwardsOutput(1.1, false, probabilityAssessmentOutput, 2.2);

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsOutputEntity entity = output.Create(registry);

            // Assert
            entity.GrassCoverErosionInwardsOutputEntityId = 984756;
            registry.TransferIds();
            Assert.AreEqual(entity.GrassCoverErosionInwardsOutputEntityId, output.StorageId);
        }
    }
}