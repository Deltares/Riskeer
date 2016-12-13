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
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                WaveHeight = 1.1,
                IsOvertoppingDominant = Convert.ToByte(false),
                Probability = 0.7,
                RequiredProbability = 0.4,
                RequiredReliability = 0.5,
                Reliability = 0.2,
                FactorOfSafety = 9.5
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.WaveHeight, output.WaveHeight.Value);
            Assert.IsFalse(output.IsOvertoppingDominant);
            Assert.IsNull(output.DikeHeightAssessmentOutput);
            Assert.AreEqual(entity.FactorOfSafety, output.ProbabilityAssessmentOutput.FactorOfSafety.Value);
            Assert.AreEqual(entity.Probability, output.ProbabilityAssessmentOutput.Probability);
            Assert.AreEqual(entity.RequiredProbability, output.ProbabilityAssessmentOutput.RequiredProbability);
            Assert.AreEqual(entity.Reliability, output.ProbabilityAssessmentOutput.Reliability.Value);
            Assert.AreEqual(entity.RequiredReliability, output.ProbabilityAssessmentOutput.RequiredReliability.Value);
        }

        [Test]
        public void Read_ValidEntityWithNullValues_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                WaveHeight = null,
                IsOvertoppingDominant = Convert.ToByte(true),
                Probability = null,
                RequiredProbability = null,
                RequiredReliability = null,
                Reliability = null,
                FactorOfSafety = null
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.WaveHeight);
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.IsNaN(output.DikeHeight);
            Assert.IsNull(output.DikeHeightAssessmentOutput);
            Assert.IsNaN(output.ProbabilityAssessmentOutput.FactorOfSafety.Value);
            Assert.IsNaN(output.ProbabilityAssessmentOutput.Probability);
            Assert.IsNaN(output.ProbabilityAssessmentOutput.RequiredProbability);
            Assert.IsNaN(output.ProbabilityAssessmentOutput.Reliability.Value);
            Assert.IsNaN(output.ProbabilityAssessmentOutput.RequiredReliability.Value);
            Assert.IsNull(output.DikeHeightAssessmentOutput);
        }
    }
}