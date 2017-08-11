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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.IO.Properties;
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
        /// <returns>A new <see cref="PipingSoilProfile"/> based on the given data.</returns>
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

            string message = string.Format(Resources.PipingSoilProfileTransformer_Cannot_tranform_Type_0_Only_types_Type_1_and_Type_2_are_supported,
                                           soilProfile.GetType().Name,
                                           nameof(SoilProfile1D),
                                           nameof(SoilProfile2D));
            throw new ImportedDataTransformException(message);
        }

        /// <summary>
        /// Creates a new instances of the <see cref="PipingSoilProfile"/> based on the <paramref name="soilProfile2D"/>.
        /// </summary>
        /// <param name="soilProfile2D">The soil profile to use in the transformation.</param>
        /// <returns>The created <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="soilProfile2D"/> can not be used to determine intersections with;</item>
        /// <item>Transforming the <paramref name="soilProfile2D"/> failed.</item>
        /// </list>
        /// </exception>
        private static PipingSoilProfile CreatePipingSoilProfile(SoilProfile2D soilProfile2D)
        {
            string profileName = soilProfile2D.Name;
            double intersectionX = soilProfile2D.IntersectionX;

            if (double.IsNaN(intersectionX))
            {
                string message = string.Format(Resources.Error_SoilProfileBuilder_cant_determine_intersect_SoilProfileName_0_at_double_NaN,
                                               profileName);
                throw new ImportedDataTransformException(message);
            }

            var layers = new List<PipingSoilLayer>();
            double bottom = double.MaxValue;
            foreach (SoilLayer2D soilLayer2D in soilProfile2D.Layers)
            {
                double newBottom;

                layers.AddRange(PipingSoilLayerTransformer.Transform(soilLayer2D,
                                                                     intersectionX,
                                                                     out newBottom));

                bottom = Math.Min(bottom, newBottom);
            }

            return new PipingSoilProfile(profileName, bottom, layers, SoilProfileType.SoilProfile2D, 0);
        }

        private static PipingSoilProfile CreatePipingSoilProfile(SoilProfile1D soilProfile1D)
        {
            IEnumerable<PipingSoilLayer> layers = soilProfile1D.Layers.Select(PipingSoilLayerTransformer.Transform);

            return new PipingSoilProfile(soilProfile1D.Name,
                                         soilProfile1D.Bottom,
                                         layers.ToArray(),
                                         SoilProfileType.SoilProfile1D,
                                         0);
        }
    }
}