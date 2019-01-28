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

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class AssessmentSectionSettingsReaderTest
    {
        [Test]
        public void ReadSettings_FromValidFile_ReturnsAllAssessmentSectionSettings()
        {
            // Setup
            var reader = new AssessmentSectionSettingsReader();

            // Call
            AssessmentSectionSettings[] settingDefinitions = reader.ReadSettings();

            // Assert
            Assert.AreEqual(237, settingDefinitions.Length);
            AssessmentSectionSettings settings = settingDefinitions[1];
            Assert.AreEqual("1-2", settings.AssessmentSectionId);
            Assert.AreEqual(2.0, settings.N);
            Assert.IsFalse(settings.IsDune);

            settings = settingDefinitions[2];
            Assert.AreEqual("2-1", settings.AssessmentSectionId);
            Assert.AreEqual(3.0, settings.N);
            Assert.IsTrue(settings.IsDune);
        }
    }
}