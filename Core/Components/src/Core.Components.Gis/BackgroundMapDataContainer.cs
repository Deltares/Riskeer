// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Components.Gis.Data;

namespace Core.Components.Gis
{
    /// <summary>
    /// Holds a <see cref="ImageBasedMapData"/> to serve as the background map-data and allows
    /// preconfiguration of <see cref="ImageBasedMapData"/> properties.
    /// </summary>
    public class BackgroundMapDataContainer : Observable
    {
        private readonly ImageBasedMapDataConfigurationHolder configuration = new ImageBasedMapDataConfigurationHolder();
        private ImageBasedMapData mapData;

        /// <summary>
        /// The background <see cref="ImageBasedMapData"/>.
        /// </summary>
        public ImageBasedMapData MapData
        {
            get
            {
                return mapData;
            }
            set
            {
                mapData = value;
                if (mapData != null)
                {
                    mapData.IsVisible = IsVisible;
                    mapData.Transparency = Transparency;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapData"/> is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return configuration.IsVisible;
            }
            set
            {
                configuration.IsVisible = value;
                if (mapData != null)
                {
                    mapData.IsVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the transparency of <see cref="MapData"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a new value
        /// that is not in the range [0.0, 1.0].</exception>
        public RoundedDouble Transparency
        {
            get
            {
                return configuration.Transparency;
            }
            set
            {
                configuration.Transparency = value;
                if (mapData != null)
                {
                    mapData.Transparency = value;
                }
            }
        }

        public override void NotifyObservers()
        {
            base.NotifyObservers();
            mapData?.NotifyObservers();
        }

        private class ImageBasedMapDataConfigurationHolder : ImageBasedMapData
        {
            public ImageBasedMapDataConfigurationHolder() : base("c") {}
        }
    }
}