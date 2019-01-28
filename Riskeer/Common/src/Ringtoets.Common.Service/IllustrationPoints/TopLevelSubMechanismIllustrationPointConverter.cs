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
using HydraRingWindDirectionClosingSituation = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// Converter for <see cref="HydraRingWindDirectionClosingSituation"/> and 
    /// <see cref="HydraRingSubMechanismIllustrationPoint"/> related to creating a 
    /// <see cref="TopLevelSubMechanismIllustrationPoint"/>.
    /// </summary>
    public static class TopLevelSubMechanismIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="TopLevelSubMechanismIllustrationPoint"/>
        /// based on the information of <paramref name="hydraRingWindDirectionClosingSituation"/>
        /// and <paramref name="hydraRingSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="hydraRingWindDirectionClosingSituation">The <see cref="HydraRingWindDirectionClosingSituation"/>
        /// to base the <see cref="TopLevelSubMechanismIllustrationPoint"/> on.</param>
        /// <param name="hydraRingSubMechanismIllustrationPoint">The <see cref="HydraRingSubMechanismIllustrationPoint"/>
        /// to base the <see cref="TopLevelSubMechanismIllustrationPoint"/> on.</param>
        /// <returns>A <see cref="TopLevelSubMechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static TopLevelSubMechanismIllustrationPoint Convert(HydraRingWindDirectionClosingSituation hydraRingWindDirectionClosingSituation,
                                                                    HydraRingSubMechanismIllustrationPoint hydraRingSubMechanismIllustrationPoint)
        {
            if (hydraRingWindDirectionClosingSituation == null)
            {
                throw new ArgumentNullException(nameof(hydraRingWindDirectionClosingSituation));
            }

            if (hydraRingSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(hydraRingSubMechanismIllustrationPoint));
            }

            WindDirection windDirection = WindDirectionConverter.Convert(hydraRingWindDirectionClosingSituation.WindDirection);
            SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                SubMechanismIllustrationPointConverter.Convert(hydraRingSubMechanismIllustrationPoint);

            return new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                             hydraRingWindDirectionClosingSituation.ClosingSituation,
                                                             subMechanismIllustrationPoint);
        }
    }
}