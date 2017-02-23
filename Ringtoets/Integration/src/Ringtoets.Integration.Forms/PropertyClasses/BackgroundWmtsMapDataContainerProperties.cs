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
using Core.Common.Utils.Attributes;
using Core.Components.Gis;
using Core.Components.Gis.Data;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using DotspatialFormsResources = Core.Components.DotSpatial.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="BackgroundMapDataContainer"/> containing a <see cref="WmtsMapData"/>
    /// for properties panel.
    /// </summary>
    public class BackgroundWmtsMapDataContainerProperties : BackgroundMapDataContainerProperties
    {
        private readonly WmtsMapData wmtsMapData;

        /// <summary>
        /// Creates a new instance of <see cref="BackgroundWmtsMapDataContainerProperties"/>.
        /// </summary>
        /// <param name="container">The data for which the properties are shown.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="container"/> is
        /// not an instance containing an instance of <see cref="WmtsMapData"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>
        /// is <c>null</c>.</exception>
        public BackgroundWmtsMapDataContainerProperties(BackgroundMapDataContainer container) : base(Validate(container))
        {
            wmtsMapData = (WmtsMapData) container.MapData;
        }

        /// <summary>
        /// Validates the <see cref="BackgroundMapDataContainer"/> used for this properties
        /// object.
        /// </summary>
        /// <param name="container">The data to be used.</param>
        /// <returns><paramref name="container"/> if it is valid.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="container"/>
        /// is not suitable for this object.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>
        /// is <c>null</c>.</exception>
        private static BackgroundMapDataContainer Validate(BackgroundMapDataContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (!(container.MapData is WmtsMapData))
            {
                throw new ArgumentException($"{typeof(BackgroundMapDataContainer).Name} must contain an instance of {typeof(WmtsMapData).Name}.",
                    nameof(container));
            }
            return container;
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_WMTS_Category))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_Url_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_Url_Description))]
        public string Url
        {
            get
            {
                return wmtsMapData.SourceCapabilitiesUrl;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_WMTS_Category))]
        [ResourcesDisplayName(typeof(DotspatialFormsResources), nameof(DotspatialFormsResources.WmtsCapability_MapLayer_Id))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_SelectedCapabilityIdentifier_Description))]
        public string SelectedCapabilityIdentifier
        {
            get
            {
                return wmtsMapData.SelectedCapabilityIdentifier;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_WMTS_Category))]
        [ResourcesDisplayName(typeof(DotspatialFormsResources), nameof(DotspatialFormsResources.WmtsCapability_MapLayer_Format))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundWmtsMapDataContainerProperties_PreferredFormat_Description))]
        public string PreferredFormat
        {
            get
            {
                return wmtsMapData.PreferredFormat;
            }
        }
    }
}