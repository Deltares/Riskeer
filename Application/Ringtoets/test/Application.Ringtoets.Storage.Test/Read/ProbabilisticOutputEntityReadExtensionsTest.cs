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

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ProbabilisticOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 567567,
                Probability = 0.1,
                RequiredProbability = 0.2,
                RequiredReliability = 0.3,
                Reliability = 0.4,
                FactorOfSafety = 0.5
            };

            // Call
            ProbabilityAssessmentOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.FactorOfSafety, output.FactorOfSafety.Value);
            Assert.AreEqual(entity.Probability, output.Probability);
            Assert.AreEqual(entity.RequiredProbability, output.RequiredProbability);
            Assert.AreEqual(entity.Reliability, output.Reliability.Value);
            Assert.AreEqual(entity.RequiredReliability, output.RequiredReliability.Value);
        }

        [Test]
        public void Read_ValidEntityWithNullValues_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 87345,
                Probability = null,
                RequiredProbability = null,
                RequiredReliability = null,
                Reliability = null,
                FactorOfSafety = null
            };

            // Call
            ProbabilityAssessmentOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.FactorOfSafety);
            Assert.IsNaN(output.Probability);
            Assert.IsNaN(output.RequiredProbability);
            Assert.IsNaN(output.Reliability);
            Assert.IsNaN(output.RequiredReliability);
        }
    }
}