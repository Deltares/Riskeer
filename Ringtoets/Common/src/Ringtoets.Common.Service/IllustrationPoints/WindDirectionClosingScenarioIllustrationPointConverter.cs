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
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;
using HydraWindDirection = Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints.WindDirection;
using WindDirection = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.WindDirection;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// 
    /// </summary>
    public static class WindDirectionClosingScenarioIllustrationPointConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hydraWindDirectionClosingSituation"></param>
        /// <param name="hydraSubMechanismIllustrationPoint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when any of:
        /// <list type="bullet">
        /// <item><paramref name="hydraWindDirectionClosingSituation"/>,</item>
        /// <item><paramref name="hydraSubMechanismIllustrationPoint"/>,</item>
        /// <item><see cref="SubMechanismIllustrationPoint.Name"/>,</item>
        /// <item><see cref="HydraWindDirection.Name"/></item>
        /// </list> are <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="HydraWindDirection.Angle"/>
        /// is not in the interval of [0, 360].</exception>
        public static WindDirectionClosingScenarioIllustrationPoint CreateWindDirectionClosingScenarioIllustrationPoint(
            WindDirectionClosingSituation hydraWindDirectionClosingSituation,
            SubMechanismIllustrationPoint hydraSubMechanismIllustrationPoint)
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
            IllustrationPoint illustrationPoint = IllustrationPointConverter.CreateIllustrationPoint(hydraSubMechanismIllustrationPoint);

            return new WindDirectionClosingScenarioIllustrationPoint(windDirection,
                                                                     hydraWindDirectionClosingSituation.ClosingSituation,
                                                                     illustrationPoint);
        }
    }
}