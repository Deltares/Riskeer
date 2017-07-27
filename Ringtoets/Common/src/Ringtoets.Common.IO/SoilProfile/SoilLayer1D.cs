﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on 
    /// be used to create the necessary input for executing a calculation.
    /// </summary>
    public class SoilLayer1D : SoilLayerBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer1D"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top">The top level of the layer.</param>
        public SoilLayer1D(double top)
        {
            Top = top;
        }

        /// <summary>
        /// Gets the top level of the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double Top { get; }
    }
}