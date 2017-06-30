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
using IllustrationPointResult = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.IllustrationPointResult;
using SubmechanismIllustrationPointStochast = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.SubmechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="IIllustrationPoint"/> data into <see cref="IllustrationPoint"/> data.
    /// </summary>
    public static class IllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPoint"/> based on the information of <paramref name="subMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="subMechanismIllustrationPoint">The <see cref="IllustrationPoint"/> to base the 
        /// <see cref="IllustrationPoint"/> to create on.</param>
        /// <returns>The newly created <see cref="IllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="subMechanismIllustrationPoint"/> 
        /// is <c>null</c>.</exception>
        public static IllustrationPoint CreateIllustrationPoint(SubMechanismIllustrationPoint subMechanismIllustrationPoint)
        {
            if (subMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(subMechanismIllustrationPoint));
            }

            IEnumerable<SubmechanismIllustrationPointStochast> stochasts = subMechanismIllustrationPoint
                .Stochasts.Select(StochastConverter.CreateSubmechanismIllustrationStochast);
            IEnumerable<IllustrationPointResult> illustrationPointResults = subMechanismIllustrationPoint
                .Results.Select(IllustrationPointResultConverter.CreateIllustrationPointResult);

            return new IllustrationPoint(subMechanismIllustrationPoint.Name,
                                         stochasts,
                                         illustrationPointResults,
                                         subMechanismIllustrationPoint.Beta);
        }
    }
}