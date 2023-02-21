// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithLinkedHydraulicBoundaryData_ReturnsExpectedSettings()
        {
            // Setup
            const string hrdFilePath = "some//FilePath//HRD dutch coast south.sqlite";
            const string hlcdFilePath = "some//FilePath//HLCD dutch coast south.sqlite";
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = hrdFilePath
            };

            hydraulicBoundaryData.HydraulicLocationConfigurationSettings.SetValues(hlcdFilePath, string.Empty, 10, string.Empty,
                                                                                   usePreprocessorClosure, null, null, null, null, null, null);

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData);

            // Assert
            Assert.AreEqual(hrdFilePath, settings.HrdFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
        }
    }
}