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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Transforms generic <see cref="ISoilProfile"/> into <see cref="PipingSoilProfile"/>.
    /// </summary>
    public static class PipingSoilProfileTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="soilProfile"/> into a mechanism specific 
        /// soil profile of type <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use in the transformation.</param>
        /// <returns>A new <see cref="PipingSoilProfile"/> based on the given data, or <c>null</c> when 
        /// <paramref name="soilProfile"/> is not of a type that can be transformed to 
        /// the mechanism specific <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static PipingSoilProfile Transform(ISoilProfile soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            var soilProfile1D = soilProfile as SoilProfile1D;
            if (soilProfile1D != null)
            {
                return CreatePipingSoilProfile(soilProfile1D);
            }

            var soilProfile2D = soilProfile as SoilProfile2D;
            if (soilProfile2D != null)
            {
                return CreatePipingSoilProfile(soilProfile2D);
            }

            return null;
        }

        private static PipingSoilProfile CreatePipingSoilProfile(SoilProfile2D soilProfile2D)
        {
            return null;
        }

        private static PipingSoilProfile CreatePipingSoilProfile(SoilProfile1D soilProfile1D)
        {
            return null;
        }
    }
}