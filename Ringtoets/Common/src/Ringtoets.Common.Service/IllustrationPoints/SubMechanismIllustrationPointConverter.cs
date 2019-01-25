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
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingSubMechanismIllustrationPoint"/> data into 
    /// <see cref="SubMechanismIllustrationPoint"/> data.
    /// </summary>
    public static class SubMechanismIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPoint"/> based on the 
        /// information of <paramref name="hydraRingSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="hydraRingSubMechanismIllustrationPoint">The <see cref="HydraRingSubMechanismIllustrationPoint"/> 
        /// to base the <see cref="SubMechanismIllustrationPoint"/> to create on.</param>
        /// <returns>The newly created <see cref="SubMechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingSubMechanismIllustrationPoint"/> 
        /// is <c>null</c>.</exception>
        public static SubMechanismIllustrationPoint Convert(HydraRingSubMechanismIllustrationPoint hydraRingSubMechanismIllustrationPoint)
        {
            if (hydraRingSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(hydraRingSubMechanismIllustrationPoint));
            }

            IEnumerable<SubMechanismIllustrationPointStochast> stochasts = hydraRingSubMechanismIllustrationPoint.Stochasts
                                                                                                                 .Select(StochastConverter.Convert)
                                                                                                                 .ToArray();
            IEnumerable<IllustrationPointResult> illustrationPointResults = hydraRingSubMechanismIllustrationPoint.Results
                                                                                                                  .Select(IllustrationPointResultConverter.Convert)
                                                                                                                  .ToArray();

            return new SubMechanismIllustrationPoint(hydraRingSubMechanismIllustrationPoint.Name,
                                                     hydraRingSubMechanismIllustrationPoint.Beta,
                                                     stochasts,
                                                     illustrationPointResults);
        }
    }
}