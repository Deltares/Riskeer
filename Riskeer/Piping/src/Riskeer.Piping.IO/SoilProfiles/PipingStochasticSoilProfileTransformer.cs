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

using System;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.IO.SoilProfiles
{
    /// <summary>
    /// Transforms generic <see cref="StochasticSoilProfile"/> into <see cref="PipingStochasticSoilProfile"/>.
    /// </summary>
    public static class PipingStochasticSoilProfileTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="stochasticSoilProfile"/> into <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="stochasticSoilProfile">The stochastic soil profile to use in the transformation.</param>
        /// <param name="soilProfile">The transformed piping soil profile.</param>
        /// <returns>A new <paramref name="soilProfile"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when <see cref="StochasticSoilProfile"/>
        /// could not be transformed.</exception>
        public static PipingStochasticSoilProfile Transform(StochasticSoilProfile stochasticSoilProfile, PipingSoilProfile soilProfile)
        {
            if (stochasticSoilProfile == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilProfile));
            }

            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            try
            {
                return new PipingStochasticSoilProfile(stochasticSoilProfile.Probability, soilProfile);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ImportedDataTransformException(e.Message, e);
            }
        }
    }
}