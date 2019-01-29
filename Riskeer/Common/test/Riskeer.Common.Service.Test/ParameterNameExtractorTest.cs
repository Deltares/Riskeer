// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using NUnit.Framework;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class ParameterNameExtractorTest
    {
        [Test]
        public void GetFromDisplayName_EmptyString_ReturnEmptyString()
        {
            // Call
            string result = ParameterNameExtractor.GetFromDisplayName(string.Empty);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetFromDisplayName_ParameterWithoutUnits_ReturnParameterName()
        {
            // Setup
            const string parameterName = " A ";

            // Call
            string result = ParameterNameExtractor.GetFromDisplayName(parameterName);

            // Assert
            Assert.AreEqual("A", result);
        }

        [Test]
        public void GetFromDisplayName_ParameterWithUnits_ReturnParameterName()
        {
            // Setup
            const string parameterName = " A ";
            string parameterWithUnits = $"{parameterName} [m/s]";

            // Call
            string result = ParameterNameExtractor.GetFromDisplayName(parameterWithUnits);

            // Assert
            Assert.AreEqual("A", result);
        }
    }
}