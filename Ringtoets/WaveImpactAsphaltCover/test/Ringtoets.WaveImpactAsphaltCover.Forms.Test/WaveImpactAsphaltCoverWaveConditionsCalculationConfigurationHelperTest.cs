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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;
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
            var random = new Random(21);
            var calculations = new List<ICalculationBase>();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(null,
                                                                                                                                      calculations,
                                                                                                                                      random.NextEnumValue<NormType>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("locations", paramName);
        }

        [Test]
        public void AddCalculationsFromLocations_CalculationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations,
                                                                                                                                      null,
                                                                                                                                      random.NextEnumValue<NormType>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations,
                                                                                                            calculationBases,
                                                                                                            random.NextEnumValue<NormType>());

            // Assert
            CollectionAssert.IsEmpty(calculationBases);
        }

        [Test]
        [TestCase(NormType.LowerLimit, AssessmentSectionCategoryType.LowerLimitNorm)]
        [TestCase(NormType.Signaling, AssessmentSectionCategoryType.SignalingNorm)]
        public void AddCalculationsFromLocations_MultipleCalculationsEmptyCalculationBase_ReturnsUniquelyNamedCalculations(
            NormType normType,
            AssessmentSectionCategoryType expectedAssessmentSectionCategoryType)
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
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations,
                                                                                                            calculationBases,
                                                                                                            normType);

            // Assert
            Assert.AreEqual(2, calculationBases.Count);
            var firstCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.First();
            Assert.AreEqual(name, firstCalculation.Name);
            AssessmentSectionCategoryWaveConditionsInput firstCalculationInput = firstCalculation.InputParameters;
            Assert.AreEqual(locations[0], firstCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedAssessmentSectionCategoryType, firstCalculationInput.CategoryType);

            var secondCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual($"{name} (1)", secondCalculation.Name);
            AssessmentSectionCategoryWaveConditionsInput secondCalculationInput = secondCalculation.InputParameters;
            Assert.AreSame(locations[1], secondCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedAssessmentSectionCategoryType, secondCalculationInput.CategoryType);
        }

        [Test]
        [TestCase(NormType.LowerLimit, AssessmentSectionCategoryType.LowerLimitNorm)]
        [TestCase(NormType.Signaling, AssessmentSectionCategoryType.SignalingNorm)]
        public void AddCalculationsFromLocations_MultipleCalculationsAndDuplicateNameInCalculationBase_ReturnsUniquelyNamedCalculations(
            NormType normType,
            AssessmentSectionCategoryType expectedAssessmentSectionCategoryType)
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
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(locations, calculationBases, normType);

            // Assert
            Assert.AreEqual(3, calculationBases.Count);
            var firstCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.ElementAt(1);
            Assert.AreEqual($"{name} (1)", firstCalculation.Name);
            AssessmentSectionCategoryWaveConditionsInput firstCalculationInput = firstCalculation.InputParameters;
            Assert.AreEqual(locations[0], firstCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedAssessmentSectionCategoryType, firstCalculationInput.CategoryType);

            var secondCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationBases.ElementAt(2);
            Assert.AreEqual($"{name} (2)", secondCalculation.Name);
            AssessmentSectionCategoryWaveConditionsInput secondCalculationInput = secondCalculation.InputParameters;
            Assert.AreSame(locations[1], secondCalculationInput.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedAssessmentSectionCategoryType, secondCalculationInput.CategoryType);
        }
    }
}