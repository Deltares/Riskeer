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

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.GrassCoverErosionInwards;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var probabilisticOutputEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 95876,
                Probability = 0.7,
                RequiredProbability = 0.4,
                RequiredReliability = 0.5,
                Reliability = 0.2,
                FactorOfSafety = 9.5
            };
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 495876,
                WaveHeight = 1.1,
                IsOvertoppingDominant = Convert.ToByte(false),
                ProbabilisticOutputEntity = probabilisticOutputEntity,
                DikeHeight = 3.3
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.GrassCoverErosionInwardsOutputId, output.StorageId);
            Assert.AreEqual(entity.WaveHeight, output.WaveHeight.Value);
            Assert.IsFalse(output.IsOvertoppingDominant);
            Assert.AreEqual(entity.DikeHeight, output.DikeHeight.Value);

            Assert.AreEqual(probabilisticOutputEntity.ProbabilisticOutputEntityId, output.ProbabilisticAssessmentOutput.StorageId);
            Assert.AreEqual(probabilisticOutputEntity.FactorOfSafety, output.ProbabilisticAssessmentOutput.FactorOfSafety.Value);
            Assert.AreEqual(probabilisticOutputEntity.Probability, output.ProbabilisticAssessmentOutput.Probability);
            Assert.AreEqual(probabilisticOutputEntity.RequiredProbability, output.ProbabilisticAssessmentOutput.RequiredProbability);
            Assert.AreEqual(probabilisticOutputEntity.Reliability, output.ProbabilisticAssessmentOutput.Reliability.Value);
            Assert.AreEqual(probabilisticOutputEntity.RequiredReliability, output.ProbabilisticAssessmentOutput.RequiredReliability.Value);
        }

        [Test]
        public void Read_ValidEntityWithNullValues_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var probabilisticOutputEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 3456,
                Probability = null,
                RequiredProbability = null,
                RequiredReliability = null,
                Reliability = null,
                FactorOfSafety = null
            };
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 7536,
                WaveHeight = null,
                IsOvertoppingDominant = Convert.ToByte(true),
                ProbabilisticOutputEntity = probabilisticOutputEntity,
                DikeHeight = null
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.GrassCoverErosionInwardsOutputId, output.StorageId);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.IsNaN(output.DikeHeight);

            Assert.AreEqual(probabilisticOutputEntity.ProbabilisticOutputEntityId, output.ProbabilisticAssessmentOutput.StorageId);
            Assert.IsNaN(output.ProbabilisticAssessmentOutput.FactorOfSafety.Value);
            Assert.IsNaN(output.ProbabilisticAssessmentOutput.Probability);
            Assert.IsNaN(output.ProbabilisticAssessmentOutput.RequiredProbability);
            Assert.IsNaN(output.ProbabilisticAssessmentOutput.Reliability.Value);
            Assert.IsNaN(output.ProbabilisticAssessmentOutput.RequiredReliability.Value);
        }
    }
}