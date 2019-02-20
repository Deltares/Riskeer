// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for wave conditions output of the <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsOutputContext
        : ObservableWrappedObjectContextBase<GrassCoverErosionOutwardsWaveConditionsOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsOutputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <param name="input">The input belonging to the wrapped data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveConditionsOutputContext(GrassCoverErosionOutwardsWaveConditionsOutput wrappedData,
                                                                    GrassCoverErosionOutwardsWaveConditionsInput input)
            : base(wrappedData)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Input = input;
        }

        /// <summary>
        /// Gets the input.
        /// </summary>
        public GrassCoverErosionOutwardsWaveConditionsInput Input { get; }
    }
}