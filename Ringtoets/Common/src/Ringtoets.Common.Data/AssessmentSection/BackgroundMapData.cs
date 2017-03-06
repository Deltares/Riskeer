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

using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.AssessmentSection
{
    /// <summary>
    /// Class that holds information about configured background data.
    /// </summary>
    public class BackgroundMapData
    {
        /// <summary>
        /// Creates a new <see cref="BackgroundMapData"/>.
        /// </summary>
        public BackgroundMapData()
        {
            IsVisible = true;
            Transparency = new RoundedDouble(2);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the background is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the transparency of the background.
        /// </summary>
        public RoundedDouble Transparency { get; set; }

        /// <summary>
        /// Gets or sets the type of the background map data.
        /// </summary>
        public BackgroundMapDataType BackgroundMapDataType { get; set; }
    }
}