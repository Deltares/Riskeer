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

using Deltares.WTIPiping;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="PipingProfile"/> instances which are required by the <see cref="PipingCalculator"/>.
    /// </summary>
    internal static class PipingProfileCreator
    {
        /// <summary>
        /// Creates a <see cref="PipingProfile"/> based on information contained in the provided <paramref name="soilProfile"/>,
        /// which can then be used in the <see cref="PipingCalculator"/>.
        /// </summary>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> from which to take the information.</param>
        /// <returns>A new <see cref="PipingProfile"/> with information taken from the <paramref name="soilProfile"/>.</returns>
        public static PipingProfile Create(PipingSoilProfile soilProfile)
        {
            var profile = new PipingProfile
            {
                BottomLevel = soilProfile.Bottom
            };
            foreach (PipingSoilLayer layer in soilProfile.Layers)
            {
                var pipingLayer = new PipingLayer
                {
                    TopLevel = layer.Top,
                    IsAquifer = layer.IsAquifer
                };

                profile.Layers.Add(pipingLayer);
            }

            return profile;
        }
    }
}