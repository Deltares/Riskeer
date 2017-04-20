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
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class ProbabilityAssessmentOutputCreateExtensionsTest
    {
        [Test]
        public void Create_ValidInput_ReturnProbabilityAssessmentOutputEntity()
        {
            // Setup
            var random = new Random(456);
            var output = new ProbabilityAssessmentOutput(random.NextDouble(), random.NextDouble(),
                                                         random.NextDouble(), random.NextDouble(),
                                                         random.NextDouble());

            // Call
            var entity = output.Create<TestProbabilityAssessmentOutputEntity>();

            // Assert
            Assert.AreEqual(output.RequiredProbability, entity.RequiredProbability);
            Assert.AreEqual(output.RequiredReliability.Value, entity.RequiredReliability);
            Assert.AreEqual(output.Probability, entity.Probability);
            Assert.AreEqual(output.Reliability.Value, entity.Reliability);
            Assert.AreEqual(output.FactorOfSafety.Value, entity.FactorOfSafety);
        }

        [Test]
        public void Create_NaNValues_ReturnProbabilityAssessmentOutputEntity()
        {
            // Setup
            var output = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN,
                                                         double.NaN, double.NaN);

            // Call
            var entity = output.Create<TestProbabilityAssessmentOutputEntity>();

            // Assert
            Assert.IsNull(entity.RequiredProbability);
            Assert.IsNull(entity.RequiredReliability);
            Assert.IsNull(entity.Probability);
            Assert.IsNull(entity.Reliability);
            Assert.IsNull(entity.FactorOfSafety);
        }

        private class TestProbabilityAssessmentOutputEntity : IProbabilityAssessmentOutputEntity
        {
            public double? RequiredProbability { get; set; }
            public double? RequiredReliability { get; set; }
            public double? Probability { get; set; }
            public double? Reliability { get; set; }
            public double? FactorOfSafety { get; set; }
        }
    }
}