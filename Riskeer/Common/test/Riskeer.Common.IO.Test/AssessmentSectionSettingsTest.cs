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

namespace Riskeer.Common.IO.Test
{
    [TestFixture]
    public class AssessmentSectionSettingsTest
    {
        [Test]
        public void CreateDikeAssessmentSectionSettings_ValidValues_ReturnSettings()
        {
            // Setup
            const string id = "test";
            const int n = 1;

            // Call
            AssessmentSectionSettings result = AssessmentSectionSettings.CreateDikeAssessmentSectionSettings(id, n);

            // Assert
            Assert.AreEqual(id, result.AssessmentSectionId);
            Assert.AreEqual(n, result.N);
            Assert.IsFalse(result.IsDune);
        }

        [Test]
        public void CreateDuneAssessmentSectionSettings_ValidValues_ReturnSettings()
        {
            // Setup
            const string id = "test";

            // Call
            AssessmentSectionSettings result = AssessmentSectionSettings.CreateDuneAssessmentSectionSettings(id);

            // Assert
            Assert.AreEqual(id, result.AssessmentSectionId);
            Assert.AreEqual(3, result.N);
            Assert.IsTrue(result.IsDune);
        }
    }
}