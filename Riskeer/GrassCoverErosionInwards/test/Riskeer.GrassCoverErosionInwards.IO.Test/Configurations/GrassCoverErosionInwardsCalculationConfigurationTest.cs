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

using System;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.GrassCoverErosionInwards.IO.Configurations;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var readCalculation = new GrassCoverErosionInwardsCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.AreEqual(name, readCalculation.Name);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocationName);
            Assert.IsNull(readCalculation.DikeProfileId);
            Assert.IsNull(readCalculation.Orientation);
            Assert.IsNull(readCalculation.DikeHeight);
            Assert.IsNull(readCalculation.DikeHeightCalculationType);
            Assert.IsNull(readCalculation.OvertoppingRateCalculationType);
            Assert.IsNull(readCalculation.WaveReduction);
            Assert.IsNull(readCalculation.CriticalFlowRate);
            Assert.IsNull(readCalculation.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsNull(readCalculation.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsNull(readCalculation.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const string hydraulicBoundaryLocationName = "name of the hydraulic boundary location";
            const string dikeProfileId = "id of the dike profile";
            const double orientation = 1.1;
            const double dikeHeight = 2.2;
            const ConfigurationHydraulicLoadsCalculationType dikeHeightCalculationType = ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm;
            const ConfigurationHydraulicLoadsCalculationType overtoppingRateCalculationType = ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability;
            const bool useBreakWater = true;
            const ConfigurationBreakWaterType breakWaterType = ConfigurationBreakWaterType.Wall;
            const double breakWaterHeight = 3.3;
            const bool useForeshore = true;
            const double criticalFlowMean = 4.4;
            const double critifalFlowStandardDeviation = 5.5;
            const bool readOvertoppingOutputIllustrationPoints = true;
            const bool readDikeHeightIllustrationPoints = true;
            const bool readOvertoppingRateIllustrationPoints = true;

            // Call
            var readCalculation = new GrassCoverErosionInwardsCalculationConfiguration(calculationName)
            {
                Name = calculationName,
                HydraulicBoundaryLocationName = hydraulicBoundaryLocationName,
                DikeProfileId = dikeProfileId,
                Orientation = orientation,
                DikeHeight = dikeHeight,
                DikeHeightCalculationType = dikeHeightCalculationType,
                OvertoppingRateCalculationType = overtoppingRateCalculationType,
                ShouldOvertoppingOutputIllustrationPointsBeCalculated = readOvertoppingOutputIllustrationPoints,
                ShouldDikeHeightIllustrationPointsBeCalculated = readDikeHeightIllustrationPoints,
                ShouldOvertoppingRateIllustrationPointsBeCalculated = readOvertoppingRateIllustrationPoints,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = useBreakWater,
                    BreakWaterType = breakWaterType,
                    BreakWaterHeight = breakWaterHeight,
                    UseForeshoreProfile = useForeshore
                },
                CriticalFlowRate = new StochastConfiguration
                {
                    Mean = criticalFlowMean,
                    StandardDeviation = critifalFlowStandardDeviation
                }
            };

            // Assert
            Assert.AreEqual(calculationName, readCalculation.Name);
            Assert.AreEqual(hydraulicBoundaryLocationName, readCalculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(dikeProfileId, readCalculation.DikeProfileId);
            Assert.AreEqual(orientation, readCalculation.Orientation);
            Assert.AreEqual(dikeHeight, readCalculation.DikeHeight);
            Assert.AreEqual(dikeHeightCalculationType, readCalculation.DikeHeightCalculationType);
            Assert.AreEqual(overtoppingRateCalculationType, readCalculation.OvertoppingRateCalculationType);
            Assert.AreEqual(useBreakWater, readCalculation.WaveReduction.UseBreakWater);
            Assert.AreEqual(breakWaterType, readCalculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(breakWaterHeight, readCalculation.WaveReduction.BreakWaterHeight);
            Assert.AreEqual(useForeshore, readCalculation.WaveReduction.UseForeshoreProfile);
            Assert.AreEqual(criticalFlowMean, readCalculation.CriticalFlowRate.Mean);
            Assert.AreEqual(critifalFlowStandardDeviation, readCalculation.CriticalFlowRate.StandardDeviation);
            Assert.AreEqual(readOvertoppingOutputIllustrationPoints, readCalculation.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(readDikeHeightIllustrationPoints, readCalculation.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(readOvertoppingRateIllustrationPoints, readCalculation.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new GrassCoverErosionInwardsCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}