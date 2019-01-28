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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.Properties;

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

            var location = new Point2D(preconsolidationStress.XCoordinate,
                                       preconsolidationStress.ZCoordinate);

            try
            {
                DistributionHelper.ValidateLogNormalDistribution(preconsolidationStress.StressDistributionType,
                                                                 preconsolidationStress.StressShift);

                var distribution = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) preconsolidationStress.StressMean,
                    CoefficientOfVariation = (RoundedDouble) preconsolidationStress.StressCoefficientOfVariation
                };

                return new MacroStabilityInwardsPreconsolidationStress(location, distribution);
            }
            catch (Exception e) when (e is DistributionValidationException || e is ArgumentOutOfRangeException)
            {
                string errorMessage = CreateErrorMessage(location, e.Message);
                throw new ImportedDataTransformException(errorMessage, e);
            }
            catch (ArgumentException e)
            {
                throw new ImportedDataTransformException(e.Message, e);
            }
        }

        private static string CreateErrorMessage(Point2D location, string errorMessage)
        {
            return string.Format(Resources.Transform_PreconsolidationStressLocation_0_has_invalid_configuration_ErrorMessage_1_,
                                 location,
                                 errorMessage);
        }
    }
}