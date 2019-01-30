// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using NUnit.Framework;
using Riskeer.Common.IO.Structures;

namespace Riskeer.Common.IO.Test.Structures
{
    [TestFixture]
    public class StructuresParameterRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var parameter = new StructuresParameterRow();

            // Assert
            Assert.IsNull(parameter.LocationId);
            Assert.IsNull(parameter.ParameterId);
            Assert.IsNull(parameter.AlphanumericValue);
            Assert.IsNaN(parameter.NumericalValue);
            Assert.IsNaN(parameter.VarianceValue);
            Assert.AreEqual(VarianceType.NotSpecified, parameter.VarianceType);
            Assert.AreEqual(-1, parameter.LineNumber);
        }

        [Test]
        [TestCase("1", "A", "Q3", 1.1, -2.2, VarianceType.CoefficientOfVariation, 4)]
        [TestCase("2", "B", "s0", -3.3, 4.4, VarianceType.StandardDeviation, 96758)]
        public void SimpleProperties_SetNewValues_GetNewlySetValues(string locationId, string id, string alphanumeric, double numerical, double variance, VarianceType type, int lineNumber)
        {
            // Setup
            var parameter = new StructuresParameterRow();

            // Call
            parameter.LocationId = locationId;
            parameter.ParameterId = id;
            parameter.AlphanumericValue = alphanumeric;
            parameter.NumericalValue = numerical;
            parameter.VarianceValue = variance;
            parameter.VarianceType = type;
            parameter.LineNumber = lineNumber;

            // Assert
            Assert.AreEqual(locationId, parameter.LocationId);
            Assert.AreEqual(id, parameter.ParameterId);
            Assert.AreEqual(alphanumeric, parameter.AlphanumericValue);
            Assert.AreEqual(numerical, parameter.NumericalValue);
            Assert.AreEqual(variance, parameter.VarianceValue);
            Assert.AreEqual(type, parameter.VarianceType);
            Assert.AreEqual(lineNumber, parameter.LineNumber);
        }
    }
}