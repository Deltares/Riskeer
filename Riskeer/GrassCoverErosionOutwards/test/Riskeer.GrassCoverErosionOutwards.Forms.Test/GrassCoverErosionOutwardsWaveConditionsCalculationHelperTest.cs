﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationHelperTest
    {
        [Test]
        public void AddCalculationsFromLocations_LocationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var calculations = new List<ICalculationBase>();

            // Call
            void Call() => GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(null, calculations, random.NextEnumValue<NormativeProbabilityType>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("locations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_CalculationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            void Call() => GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, null, random.NextEnumValue<NormativeProbabilityType>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_EmptyCollections_ReturnsEmptyList()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();
            var calculationBases = new List<ICalculationBase>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, calculationBases, random.NextEnumValue<NormativeProbabilityType>());

            // Assert
            CollectionAssert.IsEmpty(calculationBases);
        }

        [Test]
        [TestCase(NormativeProbabilityType.MaximumAllowableFloodingProbability, WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability)]
        [TestCase(NormativeProbabilityType.SignalFloodingProbability, WaveConditionsInputWaterLevelType.SignalFloodingProbability)]
        public void AddCalculationsFromLocations_MultipleLocationsEmptyCalculationBase_ReturnsUniquelyNamedCalculationsWithCorrectInputSet(
            NormativeProbabilityType normativeProbabilityType,
            WaveConditionsInputWaterLevelType expectedWaveConditionsInputWaterLevelType)
        {
            // Setup
            const string name = "name";
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, name, 1, 1),
                new HydraulicBoundaryLocation(2, name, 2, 2)
            };
            var calculationBases = new List<ICalculationBase>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, calculationBases, normativeProbabilityType);

            // Assert
            Assert.AreEqual(2, calculationBases.Count);
            var firstCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.First();
            Assert.AreEqual(name, firstCalculation.Name);
            GrassCoverErosionOutwardsWaveConditionsInput firstCalculationInput = firstCalculation.InputParameters;
            Assert.AreEqual(locations[0], firstCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, firstCalculationInput.WaterLevelType);

            var secondCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual($"{name} (1)", secondCalculation.Name);
            GrassCoverErosionOutwardsWaveConditionsInput secondCalculationInput = secondCalculation.InputParameters;
            Assert.AreSame(locations[1], secondCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, secondCalculationInput.WaterLevelType);
        }

        [Test]
        [TestCase(NormativeProbabilityType.MaximumAllowableFloodingProbability, WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability)]
        [TestCase(NormativeProbabilityType.SignalFloodingProbability, WaveConditionsInputWaterLevelType.SignalFloodingProbability)]
        public void AddCalculationsFromLocations_MultipleLocationsAndDuplicateNameInCalculationBase_ReturnsUniquelyNamedCalculationsWithCorrectInputSet(
            NormativeProbabilityType normativeProbabilityType,
            WaveConditionsInputWaterLevelType expectedWaveConditionsInputWaterLevelType)
        {
            // Setup
            const string name = "name";
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, name, 1, 1),
                new HydraulicBoundaryLocation(2, name, 2, 2)
            };
            var calculationBases = new List<ICalculationBase>
            {
                new GrassCoverErosionOutwardsWaveConditionsCalculation
                {
                    Name = name
                }
            };

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, calculationBases, normativeProbabilityType);

            // Assert
            Assert.AreEqual(3, calculationBases.Count);
            var firstCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual($"{name} (1)", firstCalculation.Name);
            GrassCoverErosionOutwardsWaveConditionsInput firstCalculationInput = firstCalculation.InputParameters;
            Assert.AreEqual(locations[0], firstCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, firstCalculationInput.WaterLevelType);

            var secondCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.ElementAt(2);
            Assert.AreEqual($"{name} (2)", secondCalculation.Name);
            GrassCoverErosionOutwardsWaveConditionsInput secondCalculationInput = secondCalculation.InputParameters;
            Assert.AreSame(locations[1], secondCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, secondCalculationInput.WaterLevelType);
        }
    }
}