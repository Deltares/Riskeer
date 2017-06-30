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
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using HydraSubmechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPoint;
using IllustrationPointResult = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.IllustrationPointResult;
using SubmechanismIllustrationPoint = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.SubmechanismIllustrationPoint;
using SubmechanismIllustrationPointStochast = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.SubmechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="IIllustrationPoint"/> data into <see cref="HydraSubmechanismIllustrationPoint"/> data.
    /// </summary>
    public static class SubmechanismIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="SubmechanismIllustrationPoint"/> based on the information of <paramref name="submechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="submechanismIllustrationPoint">The <see cref="SubmechanismIllustrationPoint"/> to base the 
        /// <see cref="SubmechanismIllustrationPoint"/> to create on.</param>
        /// <returns>The newly created <see cref="SubmechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="submechanismIllustrationPoint"/> 
        /// is <c>null</c>.</exception>
        public static SubmechanismIllustrationPoint CreateSubmechanismIllustrationPoint(HydraSubmechanismIllustrationPoint submechanismIllustrationPoint)
        {
            if (submechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(submechanismIllustrationPoint));
            }

            IEnumerable<SubmechanismIllustrationPointStochast> stochasts = submechanismIllustrationPoint
                .Stochasts.Select(StochastConverter.CreateSubmechanismIllustrationStochast);
            IEnumerable<IllustrationPointResult> illustrationPointResults = submechanismIllustrationPoint
                .Results.Select(IllustrationPointResultConverter.CreateIllustrationPointResult);

            return new SubmechanismIllustrationPoint(submechanismIllustrationPoint.Name,
                                                     stochasts,
                                                     illustrationPointResults,
                                                     submechanismIllustrationPoint.Beta);
        }
    }
}