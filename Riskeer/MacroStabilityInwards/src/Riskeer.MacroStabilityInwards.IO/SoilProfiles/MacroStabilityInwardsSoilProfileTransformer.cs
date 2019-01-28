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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfiles
{
    /// <summary>
    /// Transforms generic <see cref="ISoilProfile"/> into <see cref="MacroStabilityInwardsSoilProfile1D"/>
    /// or <see cref="MacroStabilityInwardsSoilProfile2D"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilProfileTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="soilProfile"/> into an <see cref="IMacroStabilityInwardsSoilProfile{T}"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use in the transformation.</param>
        /// <returns>A new <see cref="IMacroStabilityInwardsSoilProfile{T}"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> Transform(ISoilProfile soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            var soilProfile1D = soilProfile as SoilProfile1D;
            if (soilProfile1D != null)
            {
                return Transform(soilProfile1D);
            }

            var soilProfile2D = soilProfile as SoilProfile2D;
            if (soilProfile2D != null)
            {
                return Transform(soilProfile2D);
            }

            string errorMessage = string.Format(RingtoetsCommonIOResources.SoilProfileTransformer_Cannot_tranform_Type_0_Only_types_Type_1_and_Type_2_are_supported,
                                                soilProfile.GetType().Name,
                                                nameof(SoilProfile1D),
                                                nameof(SoilProfile2D));
            throw new ImportedDataTransformException(errorMessage);
        }

        /// <summary>
        /// Transforms the generic <paramref name="soilProfile"/> into a
        /// <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use in the transformation.</param>
        /// <returns>A <see cref="MacroStabilityInwardsSoilProfile1D"/> based on the given data.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="soilProfile"/> cannot be
        /// transformed in a <see cref="MacroStabilityInwardsSoilProfile1D"/>.</exception>
        private static MacroStabilityInwardsSoilProfile1D Transform(SoilProfile1D soilProfile)
        {
            try
            {
                return new MacroStabilityInwardsSoilProfile1D(soilProfile.Name,
                                                              soilProfile.Bottom,
                                                              soilProfile.Layers
                                                                         .Select(MacroStabilityInwardsSoilLayerTransformer.Transform)
                                                                         .ToArray());
            }
            catch (ArgumentException e)
            {
                string errorMessage = CreateErrorMessage(soilProfile.Name, e.Message);
                throw new ImportedDataTransformException(errorMessage, e);
            }
        }

        /// <summary>
        /// Transforms the generic <paramref name="soilProfile"/> into a
        /// <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use in the transformation.</param>
        /// <returns>A <see cref="MacroStabilityInwardsSoilProfile2D"/> based on the given data.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="soilProfile"/> cannot be 
        /// transformed in a <see cref="MacroStabilityInwardsSoilProfile2D"/>.</exception>
        private static MacroStabilityInwardsSoilProfile2D Transform(SoilProfile2D soilProfile)
        {
            try
            {
                return new MacroStabilityInwardsSoilProfile2D(soilProfile.Name,
                                                              soilProfile.Layers
                                                                         .Select(MacroStabilityInwardsSoilLayerTransformer.Transform)
                                                                         .ToArray(),
                                                              TransformPreconsolidationStresses(soilProfile.Name, soilProfile.PreconsolidationStresses));
            }
            catch (ArgumentException e)
            {
                string errorMessage = CreateErrorMessage(soilProfile.Name, e.Message);
                throw new ImportedDataTransformException(errorMessage, e);
            }
        }

        /// <summary>
        /// Transforms the generic collection of <see cref="PreconsolidationStress"/> into a <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <param name="soilProfileName">The name of the soil profile.</param>
        /// <param name="preconsolidationStresses">The collection of <see cref="PreconsolidationStress"/> to transform.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> based on <paramref name="preconsolidationStresses"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when the <paramref name="preconsolidationStresses"/>
        /// could not be transformed into valid <see cref="MacroStabilityInwardsPreconsolidationStress"/>.</exception>
        private static IEnumerable<MacroStabilityInwardsPreconsolidationStress> TransformPreconsolidationStresses(string soilProfileName,
                                                                                                                  IEnumerable<PreconsolidationStress> preconsolidationStresses)
        {
            try
            {
                return preconsolidationStresses.Select(MacroStabilityInwardsPreconsolidationStressTransformer.Transform)
                                               .ToArray();
            }
            catch (ImportedDataTransformException e)
            {
                string errorMessage = CreateErrorMessage(soilProfileName, e.Message);
                throw new ImportedDataTransformException(errorMessage, e);
            }
        }

        private static string CreateErrorMessage(string soilProfileName, string errorMessage)
        {
            return string.Format(RingtoetsCommonIOResources.Transform_Error_occurred_when_transforming_SoilProfile_0_ErrorMessage_1_, soilProfileName, errorMessage);
        }
    }
}