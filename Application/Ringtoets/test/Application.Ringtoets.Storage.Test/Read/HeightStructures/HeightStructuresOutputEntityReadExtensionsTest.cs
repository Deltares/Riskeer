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
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;
using Application.Ringtoets.Storage.Read.HeightStructures;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read.HeightStructures
{
    [TestFixture]
    public class HeightStructuresOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnProbabilityAssessmentOutput()
        {
            // Setup
            var random = new Random(159);
            var entity = new HeightStructuresOutputEntity
            {
                RequiredProbability = random.NextDouble(),
                RequiredReliability = random.NextDouble(),
                Probability = random.NextDouble(),
                Reliability = random.NextDouble(),
                FactorOfSafety = random.NextDouble()
            };

            // Call
            ProbabilityAssessmentOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.RequiredProbability, output.RequiredProbability);
            AssertRoundedDouble(entity.RequiredReliability, output.RequiredReliability);
            Assert.AreEqual(entity.Probability, output.Probability);
            AssertRoundedDouble(entity.Reliability, output.Reliability);
            AssertRoundedDouble(entity.FactorOfSafety, output.FactorOfSafety);
        } 
        
        [Test]
        public void Read_ValidEntityWithNullValues_ReturnProbabilityAssessmentOutput()
        {
            // Setup
            var entity = new HeightStructuresOutputEntity
            {
                RequiredProbability = null,
                RequiredReliability = null,
                Probability = null,
                Reliability = null,
                FactorOfSafety = null
            };

            // Call
            ProbabilityAssessmentOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.RequiredProbability);
            Assert.IsNaN(output.RequiredReliability.Value);
            Assert.IsNaN(output.Probability);
            Assert.IsNaN(output.Reliability.Value);
            Assert.IsNaN(output.FactorOfSafety.Value);
        }

        private static void AssertRoundedDouble(double? expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual((RoundedDouble)expectedValue.ToNullAsNaN(), actualValue, actualValue.GetAccuracy());
        }
    }
}