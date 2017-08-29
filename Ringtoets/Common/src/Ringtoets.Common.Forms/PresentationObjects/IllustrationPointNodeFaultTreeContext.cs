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

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for an <see cref="Data.IllustrationPoints.IllustrationPointNode"/> which' 
    /// <see cref="Data.IllustrationPoints.IllustrationPointNode.Data"/> is of type <see cref="FaultTreeIllustrationPoint"/>.
    /// </summary>
    public class IllustrationPointNodeFaultTreeContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointNodeFaultTreeContext"/>.
        /// </summary>
        /// <param name="illustrationPointNode">The illustration point node.</param>
        /// <param name="windDirectionName">The name of the wind direction.</param>
        /// <param name="closingSituation">The closing situation of the illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="illustrationPointNode"/> 
        /// is not of type <see cref="FaultTreeIllustrationPoint"/>.</exception>
        public IllustrationPointNodeFaultTreeContext(IllustrationPointNode illustrationPointNode,
                                                     string windDirectionName,
                                                     string closingSituation)
        {
            if (illustrationPointNode == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointNode));
            }
            if (windDirectionName == null)
            {
                throw new ArgumentNullException(nameof(windDirectionName));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }

            var illustrationPoint = illustrationPointNode.Data as FaultTreeIllustrationPoint;
            if (illustrationPoint == null)
            {
                throw new ArgumentException($"{nameof(illustrationPointNode)} type has to be {nameof(FaultTreeIllustrationPoint)}");
            }

            IllustrationPointNode = illustrationPointNode;
            WindDirectionName = windDirectionName;
            ClosingSituation = closingSituation;
        }

        /// <summary>
        /// Gets the illustration point node.
        /// </summary>
        public IllustrationPointNode IllustrationPointNode { get; }

        /// <summary>
        /// Gets the wind direction name.
        /// </summary>
        public string WindDirectionName { get; }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }
    }
}