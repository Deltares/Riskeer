﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="TopLevelFaultTreeIllustrationPoint"/> objects.
    /// </summary>
    public static class TopLevelFaultTreeIllustrationPointExtensions
    {
        /// <summary>
        /// Gets all the stochast names present in the <paramref name="topLevelFaultTreeIllustrationPoint"/>
        /// and its children.
        /// </summary>
        /// <param name="topLevelFaultTreeIllustrationPoint">The <see cref="TopLevelFaultTreeIllustrationPoint"/>
        /// to retrieve stochast names from.</param>
        /// <returns>A list of all stochast names.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="topLevelFaultTreeIllustrationPoint"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<string> GetStochastNamesRecursively(this TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint)
        {
            if (topLevelFaultTreeIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(topLevelFaultTreeIllustrationPoint));
            }

            return topLevelFaultTreeIllustrationPoint.FaultTreeNodeRoot.GetStochastNamesRecursively();
        }
    }
}