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
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using HydraSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using SubMechanismIllustrationPoint = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.SubMechanismIllustrationPoint;
using WindDirection = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.WindDirection;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// Converter for <see cref="WindDirectionClosingSituation"/> and 
    /// <see cref="HydraSubMechanismIllustrationPoint"/> related to creating a 
    /// <see cref="TopLevelSubMechanismIllustrationPoint"/>.
    /// </summary>
    public static class TopLevelSubMechanismIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="TopLevelSubMechanismIllustrationPoint"/>
        /// based on the information of <paramref name="hydraWindDirectionClosingSituation"/>
        /// and <paramref name="hydraSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="hydraWindDirectionClosingSituation">The <see cref="WindDirectionClosingSituation"/>
        /// to base the <see cref="TopLevelSubMechanismIllustrationPoint"/> on.</param>
        /// <param name="hydraSubMechanismIllustrationPoint">The <see cref="HydraSubMechanismIllustrationPoint"/>
        /// to base the <see cref="TopLevelSubMechanismIllustrationPoint"/> on.</param>
        /// <returns>A <see cref="TopLevelSubMechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static TopLevelSubMechanismIllustrationPoint CreateTopLevelSubMechanismIllustrationPoint(
            WindDirectionClosingSituation hydraWindDirectionClosingSituation,
            HydraSubMechanismIllustrationPoint hydraSubMechanismIllustrationPoint)
        {
            if (hydraWindDirectionClosingSituation == null)
            {
                throw new ArgumentNullException(nameof(hydraWindDirectionClosingSituation));
            }
            if (hydraSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(hydraSubMechanismIllustrationPoint));
            }

            WindDirection windDirection = WindDirectionConverter.CreateWindDirection(hydraWindDirectionClosingSituation.WindDirection);
            SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                SubMechanismIllustrationPointConverter.CreateSubMechanismIllustrationPoint(hydraSubMechanismIllustrationPoint);

            return new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                             hydraWindDirectionClosingSituation.ClosingSituation,
                                                             subMechanismIllustrationPoint);
        }
    }
}