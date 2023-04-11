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
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(null, new TestHydraulicBoundaryLocation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void CreateSettings_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(new HydraulicBoundaryData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithLinkedHydraulicBoundaryDataAndValidHydraulicBoundaryLocation_ReturnsExpectedSettings()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            const string hrdFilePath1 = "some//FilePath//HRD dutch coast south.sqlite";
            const string hrdFilePath2 = "some//FilePath//HRD dutch coast north.sqlite";
            const string hlcdFilePath = "some//FilePath//HLCD.sqlite";
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = hlcdFilePath,
                    UsePreprocessorClosure = usePreprocessorClosure
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = hrdFilePath1,
                        Locations =
                        {
                            hydraulicBoundaryLocation1
                        }
                    },
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = hrdFilePath2,
                        Locations =
                        {
                            hydraulicBoundaryLocation2
                        }
                    }
                }
            };

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData, hydraulicBoundaryLocation2);

            // Assert
            Assert.AreEqual(hrdFilePath2, settings.HrdFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
        }
    }
}