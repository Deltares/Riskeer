// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Primitives;

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double maximumAllowableFloodingProbability = random.NextDouble();
            double signalFloodingProbability = random.NextDouble();
            bool isRelevant = random.NextBoolean();
            bool hasProbabilitySpecified = random.NextBoolean();
            double sectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            // Call
            var input = new FailureMechanismSectionAssemblyInput(
                maximumAllowableFloodingProbability, signalFloodingProbability, isRelevant, hasProbabilitySpecified,
                sectionProbability, furtherAnalysisType, refinedSectionProbability);

            // Assert
            Assert.AreEqual(signalFloodingProbability, input.SignalFloodingProbability);
            Assert.AreEqual(maximumAllowableFloodingProbability, input.MaximumAllowableFloodingProbability);

            Assert.AreEqual(isRelevant, input.IsRelevant);
            Assert.AreEqual(hasProbabilitySpecified, input.HasProbabilitySpecified);
            Assert.AreEqual(sectionProbability, input.InitialSectionProbability);
            Assert.AreEqual(furtherAnalysisType, input.FurtherAnalysisType);
            Assert.AreEqual(refinedSectionProbability, input.RefinedSectionProbability);
        }
    }
}