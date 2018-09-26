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
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// Converter for <see cref="HydraRingWindDirectionClosingSituation"/> and 
    /// <see cref="HydraRingIllustrationPointTreeNode"/> related to creating a 
    /// <see cref="TopLevelFaultTreeIllustrationPoint"/>.
    /// </summary>
    public static class TopLevelFaultTreeIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="TopLevelFaultTreeIllustrationPoint"/>
        /// based on the information of <paramref name="hydraRingWindDirectionClosingSituation"/>
        /// and <paramref name="hydraRingIllustrationPointTreeNode"/>.
        /// </summary>
        /// <param name="hydraRingWindDirectionClosingSituation">The <see cref="HydraRingWindDirectionClosingSituation"/>
        /// to base the <see cref="TopLevelFaultTreeIllustrationPoint"/> on.</param>
        /// <param name="hydraRingIllustrationPointTreeNode">The <see cref="HydraRingIllustrationPointTreeNode"/>
        /// to base the <see cref="TopLevelFaultTreeIllustrationPoint"/> on.</param>
        /// <returns>A <see cref="TopLevelFaultTreeIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hydraRingIllustrationPointTreeNode"/>
        /// has tree node elements which do not contain exactly 0 or 2 children.</exception>
        public static TopLevelFaultTreeIllustrationPoint Convert(HydraRingWindDirectionClosingSituation hydraRingWindDirectionClosingSituation,
                                                                 HydraRingIllustrationPointTreeNode hydraRingIllustrationPointTreeNode)
        {
            if (hydraRingWindDirectionClosingSituation == null)
            {
                throw new ArgumentNullException(nameof(hydraRingWindDirectionClosingSituation));
            }

            if (hydraRingIllustrationPointTreeNode == null)
            {
                throw new ArgumentNullException(nameof(hydraRingIllustrationPointTreeNode));
            }

            WindDirection windDirection = WindDirectionConverter.Convert(hydraRingWindDirectionClosingSituation.WindDirection);

            return new TopLevelFaultTreeIllustrationPoint(windDirection,
                                                          hydraRingWindDirectionClosingSituation.ClosingSituation,
                                                          IllustrationPointNodeConverter.Convert(hydraRingIllustrationPointTreeNode));
        }
    }
}