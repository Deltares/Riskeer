// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Util.Helpers;
using Riskeer.Common.Util.TestUtil;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapFeature"/>.
    /// </summary>
    public static class DuneErosionMapFeaturesTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="features"/> contains the data that is representative for the data of
        /// dune locations and calculations in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism that contains the first part of the original data.</param>
        /// <param name="features">The collection of <see cref="MapFeature"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>the number of dune locations and features are not the same;</item>
        /// <item>the general properties (such as id, name and location) of dune locations and features
        /// are not the same;</item>
        /// <item>the water level, wave height or wave period calculation results of a dune location and the
        /// respective outputs of a corresponding feature are not the same.</item>
        /// <item>the number of meta data items does not match with the expected number of items.</item>
        /// </list>
        /// </exception>
        public static void AssertDuneLocationFeaturesData(DuneErosionFailureMechanism failureMechanism,
                                                          IEnumerable<MapFeature> features)
        {
            IEnumerable<DuneLocation> expectedDuneLocations = failureMechanism.DuneLocations;

            Assert.AreEqual(expectedDuneLocations.Count(), features.Count());
            for (var i = 0; i < expectedDuneLocations.Count(); i++)
            {
                DuneLocation expectedDuneLocation = expectedDuneLocations.ElementAt(i);
                MapFeature mapFeature = features.ElementAt(i);

                MapFeaturesMetaDataTestHelper.AssertMetaData(expectedDuneLocation.Id, mapFeature, "ID");
                MapFeaturesMetaDataTestHelper.AssertMetaData(expectedDuneLocation.Name, mapFeature, "Naam");
                MapFeaturesMetaDataTestHelper.AssertMetaData(expectedDuneLocation.CoastalAreaId, mapFeature, "Kustvaknummer");
                MapFeaturesMetaDataTestHelper.AssertMetaData(expectedDuneLocation.Offset.ToString("0.#", CultureInfo.CurrentCulture), mapFeature, "Metrering");
                MapFeaturesMetaDataTestHelper.AssertMetaData(expectedDuneLocation.D50.ToString(), mapFeature, "Rekenwaarde d50");

                Assert.AreEqual(expectedDuneLocation.Location, mapFeature.MapGeometries.First().PointCollections.First().Single());

                var presentedMetaDataItems = new List<string>();
                foreach (DuneLocationCalculationsForTargetProbability calculationsForTargetProbability in failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities)
                {
                    AssertMetaData(calculationsForTargetProbability.DuneLocationCalculations, expectedDuneLocation, GetExpectedWaterLevel,
                                   mapFeature, calculationsForTargetProbability.TargetProbability, "Rekenwaarde h - {0}", presentedMetaDataItems);

                    AssertMetaData(calculationsForTargetProbability.DuneLocationCalculations, expectedDuneLocation, GetExpectedWaveHeight,
                                   mapFeature, calculationsForTargetProbability.TargetProbability, "Rekenwaarde Hs - {0}", presentedMetaDataItems);

                    AssertMetaData(calculationsForTargetProbability.DuneLocationCalculations, expectedDuneLocation, GetExpectedWavePeriod,
                                   mapFeature, calculationsForTargetProbability.TargetProbability, "Rekenwaarde Tp - {0}", presentedMetaDataItems);
                }

                int expectedMetaDataCount = 5 + (3 * failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Count);
                Assert.AreEqual(expectedMetaDataCount, mapFeature.MetaData.Keys.Count);
            }
        }

        private static void AssertMetaData(IEnumerable<DuneLocationCalculation> calculations, DuneLocation hydraulicBoundaryLocation,
                                           Func<IEnumerable<DuneLocationCalculation>, DuneLocation, string> getExpectedResultFunc,
                                           MapFeature mapFeature, double targetProbability, string displayNameFormat,
                                           List<string> presentedMetaDataItems)
        {
            string uniqueName = NamingHelper.GetUniqueName(
                presentedMetaDataItems, string.Format(displayNameFormat, ProbabilityFormattingHelper.Format(targetProbability)),
                v => v);

            MapFeaturesMetaDataTestHelper.AssertMetaData(
                getExpectedResultFunc(calculations, hydraulicBoundaryLocation),
                mapFeature, uniqueName);

            presentedMetaDataItems.Add(uniqueName);
        }

        private static string GetExpectedWaterLevel(IEnumerable<DuneLocationCalculation> calculations,
                                                    DuneLocation duneLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.DuneLocation.Equals(duneLocation))
                                   .Output?.WaterLevel ?? RoundedDouble.NaN;

            return result.ToString();
        }

        private static string GetExpectedWaveHeight(IEnumerable<DuneLocationCalculation> calculations,
                                                    DuneLocation duneLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.DuneLocation.Equals(duneLocation))
                                   .Output?.WaveHeight ?? RoundedDouble.NaN;

            return result.ToString();
        }

        private static string GetExpectedWavePeriod(IEnumerable<DuneLocationCalculation> calculations,
                                                    DuneLocation duneLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.DuneLocation.Equals(duneLocation))
                                   .Output?.WavePeriod ?? RoundedDouble.NaN;

            return result.ToString();
        }
    }
}