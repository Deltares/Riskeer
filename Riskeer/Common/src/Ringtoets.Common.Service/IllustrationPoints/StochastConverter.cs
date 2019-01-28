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
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingSubMechanismIllustrationPointStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingStochast"/> data into <see cref="Stochast"/> data.
    /// </summary>
    public static class StochastConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="Stochast"/> based on the information of <paramref name="hydraRingStochast"/>.
        /// </summary>
        /// <param name="hydraRingStochast">The <see cref="HydraRingStochast"/> to base the 
        /// <see cref="Stochast"/> to create on.</param>
        /// <returns>The newly created <see cref="Stochast"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingStochast"/> 
        /// is <c>null</c>.</exception>
        public static Stochast Convert(HydraRingStochast hydraRingStochast)
        {
            if (hydraRingStochast == null)
            {
                throw new ArgumentNullException(nameof(hydraRingStochast));
            }

            return new Stochast(hydraRingStochast.Name,
                                hydraRingStochast.Duration,
                                hydraRingStochast.Alpha);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPointStochast"/> 
        /// based on the information of <paramref name="hydraRingSubMechanismIllustrationPointStochast"/>.
        /// </summary>
        /// <param name="hydraRingSubMechanismIllustrationPointStochast">The <see cref="HydraRingSubMechanismIllustrationPointStochast"/> 
        /// to base the  <see cref="SubMechanismIllustrationPointStochast"/> to create on.</param>
        /// <returns>The newly created <see cref="Stochast"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingSubMechanismIllustrationPointStochast"/> 
        /// is <c>null</c>.</exception>
        public static SubMechanismIllustrationPointStochast Convert(
            HydraRingSubMechanismIllustrationPointStochast hydraRingSubMechanismIllustrationPointStochast)
        {
            if (hydraRingSubMechanismIllustrationPointStochast == null)
            {
                throw new ArgumentNullException(nameof(hydraRingSubMechanismIllustrationPointStochast));
            }

            return new SubMechanismIllustrationPointStochast(hydraRingSubMechanismIllustrationPointStochast.Name,
                                                             hydraRingSubMechanismIllustrationPointStochast.Duration,
                                                             hydraRingSubMechanismIllustrationPointStochast.Alpha,
                                                             hydraRingSubMechanismIllustrationPointStochast.Realization);
        }
    }
}