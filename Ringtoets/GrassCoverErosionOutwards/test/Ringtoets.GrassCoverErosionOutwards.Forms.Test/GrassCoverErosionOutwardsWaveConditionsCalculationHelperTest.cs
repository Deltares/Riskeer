﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationHelperTest
    {
        [Test]
        public void AddCalculationsFromLocations_LocationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculations = new List<ICalculationBase>();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(null, calculations);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("locations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_CalculationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_EmptyCollections_ReturnsEmptyList()
        {
            // Setup
            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();
            var calculationBases = new List<ICalculationBase>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, calculationBases);

            // Assert
            CollectionAssert.IsEmpty(calculationBases);
        }

        [Test]
        public void AddCalculationsFromLocations_MultipleLocationsEmptyCalculationBase_ReturnsUniquelyNamedCalculations()
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
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, calculationBases);

            // Assert
            Assert.AreEqual(2, calculationBases.Count);
            var firstCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.First();
            Assert.AreEqual(name, firstCalculation.Name);
            Assert.AreEqual(locations[0], firstCalculation.InputParameters.HydraulicBoundaryLocation);

            var secondCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual(string.Format("{0} (1)", name), secondCalculation.Name);
            Assert.AreSame(locations[1], secondCalculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void AddCalculationsFromLocations_MultipleLocationsAndDuplicateNameInCalculationBase_ReturnsUniquelyNamedCalculations()
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
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(locations, calculationBases);

            // Assert
            Assert.AreEqual(3, calculationBases.Count);
            var firstCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual(string.Format("{0} (1)", name), firstCalculation.Name);
            Assert.AreEqual(locations[0], firstCalculation.InputParameters.HydraulicBoundaryLocation);

            var secondCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) calculationBases.ElementAt(2);
            Assert.AreEqual(string.Format("{0} (2)", name), secondCalculation.Name);
            Assert.AreSame(locations[1], secondCalculation.InputParameters.HydraulicBoundaryLocation);
        }
    }
}