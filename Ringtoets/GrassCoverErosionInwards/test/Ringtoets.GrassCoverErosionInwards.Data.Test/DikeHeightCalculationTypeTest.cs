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

using Core.Common.Utils.Attributes;
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class DikeHeightCalculationTypeTest
    {
        [Test]
        public void DisplayName_Always_ReturnExpectedValues()
        {
            // Assert
            Assert.AreEqual("Niet", GetDisplayName(DikeHeightCalculationType.NoCalculation));
            Assert.AreEqual("HBN bij norm", GetDisplayName(DikeHeightCalculationType.CalculateByAssessmentSectionNorm));
            Assert.AreEqual("HBN bij doorsnede-eis", GetDisplayName(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability));
        }

        [Test]
        public void ConvertToByte_Always_ReturnExpectedValues()
        {
            // Assert
            Assert.AreEqual(1, (byte) DikeHeightCalculationType.NoCalculation);
            Assert.AreEqual(2, (byte) DikeHeightCalculationType.CalculateByAssessmentSectionNorm);
            Assert.AreEqual(3, (byte) DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability);
        }

        private static string GetDisplayName(DikeHeightCalculationType value)
        {
            var type = typeof(DikeHeightCalculationType);
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(ResourcesDisplayNameAttribute), false);
            return ((ResourcesDisplayNameAttribute) attributes[0]).DisplayName;
        }
    }
}