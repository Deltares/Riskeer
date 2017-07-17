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
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for <see cref="IllustrationPointControlItem"/>.
    /// </summary>
    public static class IllustrationPointControlItemHelper
    {
        /// <summary>
        /// Determines whether the items in <paramref name="illustrationPointControlItems"/>
        /// have the same closing situation.
        /// </summary>
        /// <param name="illustrationPointControlItems">The collection of <see cref="IllustrationPointControlItem"/>
        /// to determine the closing situations for.</param>
        /// <returns><c>true</c> if all items in the collection have the same closing situation; 
        /// <c>false</c> if otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="illustrationPointControlItems"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="illustrationPointControlItems"/>
        /// contains <c>null</c> items.</exception>
        public static bool AreClosingSituationsSame(IEnumerable<IllustrationPointControlItem> illustrationPointControlItems)
        {
            if (illustrationPointControlItems == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointControlItems));
            }

            IllustrationPointControlItem[] illustrationPointControlItemArray = illustrationPointControlItems.ToArray();
            if (illustrationPointControlItemArray.Contains(null))
            {
                throw new ArgumentException(@"Collection cannot contain null items", nameof(illustrationPointControlItems));
            }

            return illustrationPointControlItemArray.All(item => item.ClosingSituation ==
                                                                 illustrationPointControlItemArray[0].ClosingSituation);
        }
    }
}