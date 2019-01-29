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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using NUnit.Framework;
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

                Assert.AreEqual(expectedDuneLocation.Id, mapFeature.MetaData["ID"]);
                Assert.AreEqual(expectedDuneLocation.Name, mapFeature.MetaData["Naam"]);
                Assert.AreEqual(expectedDuneLocation.CoastalAreaId, mapFeature.MetaData["Kustvaknummer"]);
                Assert.AreEqual(expectedDuneLocation.Offset.ToString("0.#", CultureInfo.CurrentCulture), mapFeature.MetaData["Metrering"]);
                Assert.AreEqual(expectedDuneLocation.Location, mapFeature.MapGeometries.First().PointCollections.First().Single());
                MapFeaturesMetaDataTestHelper.AssertMetaData(expectedDuneLocation.D50.ToString(), mapFeature, "Rekenwaarde d50");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h gr.Iv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h gr.IIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h gr.IIIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h gr.IVv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForFactorizedLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h gr.Vv");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs gr.Iv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs gr.IIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs gr.IIIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs gr.IVv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForFactorizedLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs gr.Vv");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp gr.Iv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp gr.IIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp gr.IIIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp gr.IVv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForFactorizedLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp gr.Vv");

                Assert.AreEqual(20, mapFeature.MetaData.Keys.Count);
            }
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