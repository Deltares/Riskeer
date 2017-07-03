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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using HydraSubmechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPoint;
using IllustrationPointResult = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.IllustrationPointResult;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="IIllustrationPoint"/> data into <see cref="HydraSubmechanismIllustrationPoint"/> data.
    /// </summary>
    public static class SubMechanismIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPoint"/> based on the information of <paramref name="subMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="subMechanismIllustrationPoint">The <see cref="SubMechanismIllustrationPoint"/> to base the 
        /// <see cref="SubMechanismIllustrationPoint"/> to create on.</param>
        /// <returns>The newly created <see cref="SubMechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="subMechanismIllustrationPoint"/> 
        /// is <c>null</c>.</exception>
        public static SubMechanismIllustrationPoint CreateSubMechanismIllustrationPoint(HydraSubmechanismIllustrationPoint subMechanismIllustrationPoint)
        {
            if (subMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(subMechanismIllustrationPoint));
            }

            IEnumerable<SubMechanismIllustrationPointStochast> stochasts = subMechanismIllustrationPoint
                .Stochasts.Select(StochastConverter.CreateSubMechanismIllustrationStochast);
            IEnumerable<IllustrationPointResult> illustrationPointResults = subMechanismIllustrationPoint
                .Results.Select(IllustrationPointResultConverter.CreateIllustrationPointResult);

            return new SubMechanismIllustrationPoint(subMechanismIllustrationPoint.Name,
                                                     stochasts,
                                                     illustrationPointResults,
                                                     subMechanismIllustrationPoint.Beta);
        }
    }
}