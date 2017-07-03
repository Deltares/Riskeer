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
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using HydraStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraSubmechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraStochast"/> data into <see cref="Stochast"/> data.
    /// </summary>
    public static class StochastConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="Stochast"/> based on the information of <paramref name="hydraStochast"/>.
        /// </summary>
        /// <param name="hydraStochast">The <see cref="HydraStochast"/> to base the 
        /// <see cref="Stochast"/> to create on.</param>
        /// <returns>The newly created <see cref="Stochast"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraStochast"/> 
        /// is <c>null</c>.</exception>
        public static Stochast CreateStochast(HydraStochast hydraStochast)
        {
            if (hydraStochast == null)
            {
                throw new ArgumentNullException(nameof(hydraStochast));
            }
            return new Stochast(hydraStochast.Name, hydraStochast.Duration, hydraStochast.Alpha);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPointStochast"/> based on the information of <paramref name="hydraSubmechanismIllustrationPointStochast"/>.
        /// </summary>
        /// <param name="hydraSubmechanismIllustrationPointStochast">The <see cref="HydraSubmechanismIllustrationPointStochast"/> to base the 
        /// <see cref="SubMechanismIllustrationPointStochast"/> to create on.</param>
        /// <returns>The newly created <see cref="Stochast"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraSubmechanismIllustrationPointStochast"/> 
        /// is <c>null</c>.</exception>
        public static SubMechanismIllustrationPointStochast CreateSubMechanismIllustrationStochast(HydraSubmechanismIllustrationPointStochast hydraSubmechanismIllustrationPointStochast)
        {
            if (hydraSubmechanismIllustrationPointStochast == null)
            {
                throw new ArgumentNullException(nameof(hydraSubmechanismIllustrationPointStochast));
            }
            return new SubMechanismIllustrationPointStochast(hydraSubmechanismIllustrationPointStochast.Name,
                                                             hydraSubmechanismIllustrationPointStochast.Duration,
                                                             hydraSubmechanismIllustrationPointStochast.Alpha,
                                                             hydraSubmechanismIllustrationPointStochast.Realization);
        }
    }
}