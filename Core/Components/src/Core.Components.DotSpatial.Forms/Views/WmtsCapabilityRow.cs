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

namespace Core.Components.DotSpatial.Forms.Views
{
    /// <summary>
    /// Class for displaying a <see cref="WmtsCapability"/> as a row in a grid view.
    /// </summary>
    public class WmtsCapabilityRow
    {
        /// <summary>
        /// Creates new instance of <see cref="WmtsCapabilityRow"/>.
        /// </summary>
        /// <param name="wmtsCapability">The <see cref="WmtsCapability"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wmtsCapability"/> is <c>null</c>.</exception>
        public WmtsCapabilityRow(WmtsCapability wmtsCapability)
        {
            if (wmtsCapability == null)
            {
                throw new ArgumentNullException(nameof(wmtsCapability));
            }
            WmtsCapability = wmtsCapability;
        }

        /// <summary>
        /// Gets the id of the <see cref="WmtsCapability"/>.
        /// </summary>
        public string Id
        {
            get
            {
                return WmtsCapability.Id;
            }
        }

        /// <summary>
        /// Gets the image format of the <see cref="WmtsCapability"/>.
        /// </summary>
        public string Format
        {
            get
            {
                return WmtsCapability.Format;
            }
        }

        /// <summary>
        /// Gets the title of the <see cref="WmtsCapability"/>.
        /// </summary>
        public string Title
        {
            get
            {
                return WmtsCapability.Title;
            }
        }

        /// <summary>
        /// Gets the coordinate system of the <see cref="WmtsCapability"/>.
        /// </summary>
        public string CoordinateSystem
        {
            get
            {
                return WmtsCapability.CoordinateSystem;
            }
        }

        /// <summary>
        /// Gets the <see cref="WmtsCapability"/> this row contains.
        /// </summary>
        public WmtsCapability WmtsCapability { get; }
    }
}