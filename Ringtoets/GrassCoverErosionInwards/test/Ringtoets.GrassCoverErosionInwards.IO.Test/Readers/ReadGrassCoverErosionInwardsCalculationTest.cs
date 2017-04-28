// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.GrassCoverErosionInwards.IO.Readers;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Readers
{
    [TestFixture]
    public class ReadGrassCoverErosionInwardsCalculationTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReadGrassCoverErosionInwardsCalculation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValues_PropertiesAreDefault()
        {
            // Call
            var constructionProperties = new ReadGrassCoverErosionInwardsCalculation.ConstructionProperties();
            var readCalculation = new ReadGrassCoverErosionInwardsCalculation(constructionProperties);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.IsNull(readCalculation.Name);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(readCalculation.DikeProfile);
            Assert.IsNull(readCalculation.Orientation);
            Assert.IsNull(readCalculation.DikeHeight);
            Assert.IsNull(readCalculation.DikeHeightCalculationType);
            Assert.IsNull(readCalculation.OvertoppingRateCalculationType);
            Assert.IsNull(readCalculation.UseBreakWater);
            Assert.IsNull(readCalculation.BreakWaterType);
            Assert.IsNull(readCalculation.BreakWaterHeight);
            Assert.IsNull(readCalculation.UseForeshore);
            Assert.IsNull(readCalculation.CriticalFlowRateMean);
            Assert.IsNull(readCalculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAsExpected()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const string hydraulicBoundaryLocationName = "name of the hydraulic boundary location";
            const string dikeProfileName = "name of the dike profile";
            const double orientation = 1.1;
            const double dikeHeight = 2.2;
            const ReadHydraulicLoadsCalculationType dikeHeightCalculationType = ReadHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm;
            const ReadHydraulicLoadsCalculationType overtoppingRateCalculationType = ReadHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability;
            const bool useBreakWater = true;
            const ConfigurationBreakWaterType breakWaterType = ConfigurationBreakWaterType.Wall;
            const double breakWaterHeight = 3.3;
            const bool useForeshore = true;
            const double criticalFlowMean = 4.4;
            const double critifalFlowStandardDeviation = 5.5;

            var constructionProperties = new ReadGrassCoverErosionInwardsCalculation.ConstructionProperties
            {
                Name = calculationName,
                HydraulicBoundaryLocation = hydraulicBoundaryLocationName,
                DikeProfile = dikeProfileName,
                Orientation = orientation,
                DikeHeight = dikeHeight,
                DikeHeightCalculationType = dikeHeightCalculationType,
                OvertoppingRateCalculationType = overtoppingRateCalculationType,
                UseBreakWater = useBreakWater,
                BreakWaterType = breakWaterType,
                BreakWaterHeight = breakWaterHeight,
                UseForeshore = useForeshore,
                CriticalFlowRateMean = criticalFlowMean,
                CriticalFlowRateStandardDeviation = critifalFlowStandardDeviation
            };

            // Call
            var readCalculation = new ReadGrassCoverErosionInwardsCalculation(constructionProperties);

            // Assert
            Assert.AreEqual(calculationName, readCalculation.Name);
            Assert.AreEqual(hydraulicBoundaryLocationName, readCalculation.HydraulicBoundaryLocation);
            Assert.AreEqual(dikeProfileName, constructionProperties.DikeProfile);
            Assert.AreEqual(orientation, readCalculation.Orientation);
            Assert.AreEqual(dikeHeight, readCalculation.DikeHeight);
            Assert.AreEqual(dikeHeightCalculationType, readCalculation.DikeHeightCalculationType);
            Assert.AreEqual(overtoppingRateCalculationType, readCalculation.OvertoppingRateCalculationType);
            Assert.AreEqual(useBreakWater, readCalculation.UseBreakWater);
            Assert.AreEqual(breakWaterType, readCalculation.BreakWaterType);
            Assert.AreEqual(breakWaterHeight, readCalculation.BreakWaterHeight);
            Assert.AreEqual(useForeshore, readCalculation.UseForeshore);
            Assert.AreEqual(criticalFlowMean, readCalculation.CriticalFlowRateMean);
            Assert.AreEqual(critifalFlowStandardDeviation, readCalculation.CriticalFlowRateStandardDeviation);
        }
    }
}