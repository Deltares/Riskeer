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

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Represents the top level combination of wind direction, closing situation
    /// and the fault tree.
    /// </summary>
    public class TopLevelFaultTreeIllustrationPoint : TopLevelIllustrationPointBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="TopLevelFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingSituation">The closing situation.</param>
        /// <param name="faultTreeNodeRoot">The illustration point root node.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter
        /// is <c>null</c>.</exception>
        public TopLevelFaultTreeIllustrationPoint(WindDirection windDirection,
                                                  string closingSituation,
                                                  IllustrationPointNode faultTreeNodeRoot)
            : base(windDirection, closingSituation)
        {
            if (faultTreeNodeRoot == null)
            {
                throw new ArgumentNullException(nameof(faultTreeNodeRoot));
            }

            FaultTreeNodeRoot = faultTreeNodeRoot;
        }

        /// <summary>
        /// Gets the root of the illustration points of the fault tree.
        /// </summary>
        public IllustrationPointNode FaultTreeNodeRoot { get; private set; }

        public override object Clone()
        {
            var clone = (TopLevelFaultTreeIllustrationPoint) base.Clone();

            clone.FaultTreeNodeRoot = (IllustrationPointNode) FaultTreeNodeRoot.Clone();

            return clone;
        }
    }
}