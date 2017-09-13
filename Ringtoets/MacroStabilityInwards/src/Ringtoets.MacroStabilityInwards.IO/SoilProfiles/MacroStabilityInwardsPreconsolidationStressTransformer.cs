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
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfiles
{
    /// <summary>
    /// Transforms a generic <see cref="PreconsolidationStress"/> into a 
    /// <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
    /// </summary>
    internal static class MacroStabilityInwardsPreconsolidationStressTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="preconsolidationStress"/> into 
        /// a <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <param name="preconsolidationStress">The preconsolidation stress to use 
        /// in the transformation.</param>
        /// <returns>A <see cref="MacroStabilityInwardsPreconsolidationStress"/>
        /// based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="preconsolidationStress"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when the 
        /// <paramref name="preconsolidationStress"/> could not be transformed into
        /// a <see cref="MacroStabilityInwardsPreconsolidationStress"/>.</exception>
        public static MacroStabilityInwardsPreconsolidationStress Transform(PreconsolidationStress preconsolidationStress)
        {
            if (preconsolidationStress == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStress));
            }

            DistributionHelper.ValidateIsNonShiftedLogNormal(preconsolidationStress.PreconsolidationStressDistributionType,
                                                                      preconsolidationStress.PreconsolidationStressShift,
                                                                      RingtoetsCommonDataResources.PreconsolidationStress_Name);

            try
            {
                return new MacroStabilityInwardsPreconsolidationStress(preconsolidationStress.XCoordinate,
                                                                       preconsolidationStress.ZCoordinate,
                                                                       preconsolidationStress.PreconsolidationStressMean,
                                                                       preconsolidationStress.PreconsolidationStressCoefficientOfVariation);
            }
            catch (ArgumentException e)
            {
                throw new ImportedDataTransformException(e.Message, e);
            }
        }
    }
}