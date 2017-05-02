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
using System.Globalization;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.Properties;
using RingtoetsDuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// Factory for creating arrays of <see cref="MapFeature"/> for the <see cref="DuneErosionFailureMechanism"/> 
    /// to use in <see cref="FeatureBasedMapData"/> (created via <see cref="RingtoetsMapDataFactory"/>).
    /// </summary>
    internal static class DuneErosionMapDataFeaturesFactory
    {
        /// <summary>
        /// Create dune location features based on the provided <paramref name="duneLocations"/>.
        /// </summary>
        /// <param name="duneLocations">The array of <see cref="DuneLocation"/>
        /// to create the location features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="duneLocations"/>
        /// is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocations"/> is <c>null</c>.</exception>
        public static MapFeature[] CreateDuneLocationFeatures(DuneLocation[] duneLocations)
        {
            if (duneLocations == null)
            {
                throw new ArgumentNullException(nameof(duneLocations));
            }

            var features = new MapFeature[duneLocations.Length];

            for (var i = 0; i < duneLocations.Length; i++)
            {
                DuneLocation location = duneLocations[i];

                MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSinglePointMapFeature(location.Location);
                feature.MetaData[RingtoetsCommonFormsResources.MetaData_ID] = location.Id;
                feature.MetaData[RingtoetsCommonFormsResources.MetaData_Name] = location.Name;
                feature.MetaData[Resources.MetaData_CoastalAreaId] = location.CoastalAreaId;
                feature.MetaData[Resources.MetaData_Offset] = location.Offset.ToString(RingtoetsDuneErosionDataResources.DuneLocation_Offset_format,
                                                                                       CultureInfo.InvariantCulture);
                feature.MetaData[Resources.MetaData_WaterLevel] = location.Output?.WaterLevel ?? double.NaN;
                feature.MetaData[Resources.MetaData_WaveHeight] = location.Output?.WaveHeight ?? double.NaN;
                feature.MetaData[Resources.MetaData_WavePeriod] = location.Output?.WavePeriod ?? double.NaN;
                feature.MetaData[Resources.MetaData_D50] = location.D50;

                features[i] = feature;
            }

            return features;
        }
    }
}