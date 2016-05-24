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

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresInputTest
    {
        [Test]
        public void Constructor_NullGeneralInput_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new HeightStructuresInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();

            // Call
            var input = new HeightStructuresInput(generalInput);

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            Assert.AreEqual(generalInput.GravitationalAcceleration, input.GravitationalAcceleration);
            Assert.AreEqual(generalInput.ModelfactorOvertopping, input.ModelfactorOvertopping);
            Assert.AreEqual(generalInput.ModelFactorForStorageVolume, input.ModelFactorForStorageVolume);
            Assert.AreEqual(generalInput.ModelFactorForIncomingFlowVolume, input.ModelFactorForIncomingFlowVolume);

            Assert.AreEqual(0.05, input.LevelOfCrestOfStructure.StandardDeviation, input.LevelOfCrestOfStructure.StandardDeviation.GetAccuracy());
            Assert.AreEqual(1.1, input.ModelfactorOvertoppingSuperCriticalFlow.Mean, input.ModelfactorOvertoppingSuperCriticalFlow.Mean.GetAccuracy());
            Assert.AreEqual(0.03, input.ModelfactorOvertoppingSuperCriticalFlow.StandardDeviation, input.ModelfactorOvertoppingSuperCriticalFlow.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0.1, input.AllowableIncreaseOfLevelForStorage.StandardDeviation, input.AllowableIncreaseOfLevelForStorage.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0.1, input.StorageStructureArea.StandardDeviation, input.StorageStructureArea.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0.05, input.FlowWidthAtBottomProtection.StandardDeviation, input.FlowWidthAtBottomProtection.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0.15, input.CriticalOvertoppingDischarge.StandardDeviation, input.CriticalOvertoppingDischarge.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0.05, input.WidthOfFlowApertures.StandardDeviation, input.WidthOfFlowApertures.StandardDeviation.GetAccuracy());
            Assert.AreEqual(7.5, input.StormDuration.Mean, input.StormDuration.Mean.GetAccuracy());
            Assert.AreEqual(0.25, input.StormDuration.StandardDeviation, input.StormDuration.StandardDeviation.GetAccuracy());
        }

        [Test]
        public void Properties_ExpectedValues()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();
            var input = new HeightStructuresInput(generalInput);
            var random = new Random(22);

            NormalDistribution levelOfCrestOfStructure = new NormalDistribution(2);
            RoundedDouble defaultModelfactorOvertoppingSupercriticalFlowStandardDeviation = input.ModelfactorOvertoppingSuperCriticalFlow.StandardDeviation;
            NormalDistribution modelfactorOvertoppingSupercriticalFlow = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(5, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            var orientationOfTheNormalOfTheStructure = new RoundedDouble(5, random.NextDouble());
            var allowableIncreaseOfLevelForStorage = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };
            var storageStructureArea = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };
            var flowWidthAtBottomProtection = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };
            var criticalOvertoppingDischarge = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };
            var failureProbabilityOfStructureGivenErosion = new RoundedDouble(5, random.NextDouble());
            var widthOfFlowApertures = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };
            var deviationOfTheWaveDirection = new RoundedDouble(5, random.NextDouble());
            RoundedDouble defaultStormDuration = input.StormDuration.StandardDeviation;
            var stormDuration = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, random.NextDouble()),
                StandardDeviation = new RoundedDouble(2, random.NextDouble())
            };

            // Call
            input.LevelOfCrestOfStructure = levelOfCrestOfStructure;
            input.OrientationOfTheNormalOfTheStructure = orientationOfTheNormalOfTheStructure;
            input.ModelfactorOvertoppingSuperCriticalFlow = modelfactorOvertoppingSupercriticalFlow;
            input.AllowableIncreaseOfLevelForStorage = allowableIncreaseOfLevelForStorage;
            input.StorageStructureArea = storageStructureArea;
            input.FlowWidthAtBottomProtection = flowWidthAtBottomProtection;
            input.CriticalOvertoppingDischarge = criticalOvertoppingDischarge;
            input.FailureProbabilityOfStructureGivenErosion = failureProbabilityOfStructureGivenErosion;
            input.WidthOfFlowApertures = widthOfFlowApertures;
            input.DeviationOfTheWaveDirection = deviationOfTheWaveDirection;
            input.StormDuration = stormDuration;
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreEqual(levelOfCrestOfStructure.Mean, input.LevelOfCrestOfStructure.Mean);
            Assert.AreEqual(levelOfCrestOfStructure.StandardDeviation, input.LevelOfCrestOfStructure.StandardDeviation);

            Assert.AreEqual(modelfactorOvertoppingSupercriticalFlow.Mean, input.ModelfactorOvertoppingSuperCriticalFlow.Mean);
            Assert.AreEqual(defaultModelfactorOvertoppingSupercriticalFlowStandardDeviation, input.ModelfactorOvertoppingSuperCriticalFlow.StandardDeviation);

            Assert.AreEqual(orientationOfTheNormalOfTheStructure, input.OrientationOfTheNormalOfTheStructure, input.OrientationOfTheNormalOfTheStructure.GetAccuracy());

            Assert.AreEqual(allowableIncreaseOfLevelForStorage.Mean, input.AllowableIncreaseOfLevelForStorage.Mean);
            Assert.AreEqual(allowableIncreaseOfLevelForStorage.StandardDeviation, input.AllowableIncreaseOfLevelForStorage.StandardDeviation);

            Assert.AreEqual(storageStructureArea.Mean, input.StorageStructureArea.Mean);
            Assert.AreEqual(storageStructureArea.StandardDeviation, input.StorageStructureArea.StandardDeviation);

            Assert.AreEqual(flowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(flowWidthAtBottomProtection.StandardDeviation, input.FlowWidthAtBottomProtection.StandardDeviation);

            Assert.AreEqual(criticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(criticalOvertoppingDischarge.StandardDeviation, input.CriticalOvertoppingDischarge.StandardDeviation);

            Assert.AreEqual(failureProbabilityOfStructureGivenErosion, input.FailureProbabilityOfStructureGivenErosion, input.FailureProbabilityOfStructureGivenErosion.GetAccuracy());

            Assert.AreEqual(widthOfFlowApertures.Mean, input.WidthOfFlowApertures.Mean);
            Assert.AreEqual(widthOfFlowApertures.StandardDeviation, input.WidthOfFlowApertures.StandardDeviation);

            Assert.AreEqual(deviationOfTheWaveDirection, input.DeviationOfTheWaveDirection, input.DeviationOfTheWaveDirection.GetAccuracy());

            Assert.AreEqual(stormDuration.Mean, input.StormDuration.Mean);
            Assert.AreEqual(defaultStormDuration, input.StormDuration.StandardDeviation);

            Assert.AreEqual(location, input.HydraulicBoundaryLocation);
        }
    }
}