﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelperTest
    {
        [Test]
        public void AddCalculationsFromLocations_LocationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculations = new List<ICalculationBase>();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(null, calculations);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("locations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_CalculationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_EmptyCollections_ReturnsEmptyList()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();
            var calculationBases = new List<ICalculationBase>();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations, calculationBases);

            // Assert
            CollectionAssert.IsEmpty(calculationBases);
        }

        [Test]
        public void AddCalculationsFromLocations_MultipleCalculationsEmptyCalculationBase_ReturnsUniquelyNamedCalculations()
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
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations, calculationBases);

            // Assert
            Assert.AreEqual(2, calculationBases.Count);
            var firstCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.First();
            Assert.AreEqual(name, firstCalculation.Name);
            Assert.AreEqual(locations[0], firstCalculation.InputParameters.HydraulicBoundaryLocation);

            var secondCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual($"{name} (1)", secondCalculation.Name);
            Assert.AreSame(locations[1], secondCalculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void AddCalculationsFromLocations_MultipleCalculationsAndDuplicateNameInCalculationBase_ReturnsUniquelyNamedCalculations()
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
                new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    Name = name
                }
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations, calculationBases);

            // Assert
            Assert.AreEqual(3, calculationBases.Count);
            var firstCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual($"{name} (1)", firstCalculation.Name);
            Assert.AreEqual(locations[0], firstCalculation.InputParameters.HydraulicBoundaryLocation);

            var secondCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.ElementAt(2);
            Assert.AreEqual($"{name} (2)", secondCalculation.Name);
            Assert.AreSame(locations[1], secondCalculation.InputParameters.HydraulicBoundaryLocation);
        }
    }
}