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
using Core.Common.Base.Data;
using Core.Components.Gis.Data;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Holds a <see cref="ImageBasedMapData"/> to serve as the background map-data and allows
    /// preconfiguration of <see cref="ImageBasedMapData"/> properties.
    /// </summary>
    public class BackgroundMapDataContainer
    {
        private readonly ImageBasedMapDataConfigurationHolder configuration = new ImageBasedMapDataConfigurationHolder();
        private ImageBasedMapData backgroundMapData;

        /// <summary>
        /// The background <see cref="ImageBasedMapData"/>.
        /// </summary>
        public ImageBasedMapData BackgroundMapData
        {
            get
            {
                return backgroundMapData;
            }
            set
            {
                backgroundMapData = value;
                if (backgroundMapData != null)
                {
                    backgroundMapData.IsVisible = IsVisible;
                    backgroundMapData.Transparency = Transparency;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BackgroundMapData"/> is visible.
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
                if (backgroundMapData != null)
                {
                    backgroundMapData.IsVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the transparency of <see cref="BackgroundMapData"/>.
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
                if (backgroundMapData != null)
                {
                    backgroundMapData.Transparency = value;
                }
            }
        }

        private class ImageBasedMapDataConfigurationHolder : ImageBasedMapData
        {
            public ImageBasedMapDataConfigurationHolder() : base("c") {}
        }
    }
}