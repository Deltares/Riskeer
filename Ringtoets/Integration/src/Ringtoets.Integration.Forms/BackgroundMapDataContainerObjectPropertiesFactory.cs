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
using Core.Common.Gui.PropertyBag;
using Core.Components.Gis;
using Core.Components.Gis.Data;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// Factory for creating <see cref="BackgroundMapDataContainerProperties"/> (or derived
    /// instances thereof) to be paired with a <see cref="BackgroundMapDataContainer"/> instance.
    /// </summary>
    public static class BackgroundMapDataContainerObjectPropertiesFactory
    {
        /// <summary>
        /// Create a new <see cref="BackgroundMapDataContainerProperties"/> implementation
        /// and initialize it with the given <see cref="BackgroundMapDataContainer"/> instance.
        /// </summary>
        /// <param name="container">The <see cref="BackgroundMapDataContainer"/> to create
        /// a <see cref="BackgroundMapDataContainerProperties"/> implementation for.</param>
        /// <returns>The <see cref="BackgroundMapDataContainerProperties"/> implementation
        /// with its <see cref="IObjectProperties.Data"/> initialized with <paramref name="container"/>.</returns>
        public static BackgroundMapDataContainerProperties GetObjectProperties(BackgroundMapDataContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (container.MapData is WmtsMapData)
            {
                return new BackgroundWmtsMapDataContainerProperties(container);
            }
            return new BackgroundMapDataContainerProperties(container);
        }
    }
}