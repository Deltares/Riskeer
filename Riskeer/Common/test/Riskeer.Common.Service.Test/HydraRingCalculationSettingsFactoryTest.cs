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

using System;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraRingCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryCalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraRingCalculationSettingsFactory.CreateSettings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryCalculationSettings", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithHydraulicBoundaryCalculationSettings_ReturnsExpectedSettings()
        {
            // Setup
            var hydraulicBoundaryCalculationSettings = new HydraulicBoundaryCalculationSettings("HydraulicBoundaryDataBaseFilePath",
                                                                                                "hlcdFilePath",
                                                                                                "preprocessorDirectory");

            // Call
            HydraRingCalculationSettings hydraRingCalculationSettings = HydraRingCalculationSettingsFactory.CreateSettings(hydraulicBoundaryCalculationSettings);

            // Assert
            Assert.AreEqual(hydraulicBoundaryCalculationSettings.HlcdFilePath, hydraRingCalculationSettings.HlcdFilePath);
            Assert.AreEqual(hydraulicBoundaryCalculationSettings.PreprocessorDirectory, hydraRingCalculationSettings.PreprocessorDirectory);
        }
    }
}