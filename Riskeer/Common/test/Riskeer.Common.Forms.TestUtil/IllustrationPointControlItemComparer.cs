// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// This class is used to compare <see cref="IllustrationPointControlItem"/> on equality. 
    /// </summary>
    /// <remarks>This class should not be used for sorting <see cref="IllustrationPointControlItem"/>,
    /// because the order is normally undefined when determining reference equality.</remarks>
    public class IllustrationPointControlItemComparer : IComparer<IllustrationPointControlItem>, IComparer
    {
        public int Compare(object x, object y)
        {
            var lhs = x as IllustrationPointControlItem;
            var rhs = y as IllustrationPointControlItem;
            if (lhs == null || rhs == null)
            {
                throw new ArgumentException("Arguments must be of type 'IllustrationPointControlItem', but found:" +
                                            $"x: {x.GetType()} and y: {y.GetType()}");
            }

            return Compare(lhs, rhs);
        }

        public int Compare(IllustrationPointControlItem x, IllustrationPointControlItem y)
        {
            if (!ReferenceEquals(x?.Source, y?.Source))
            {
                return -1;
            }

            if (!ReferenceEquals(x?.Stochasts, y?.Stochasts))
            {
                return -1;
            }

            if (x.WindDirectionName.CompareTo(y.WindDirectionName) != 0)
            {
                return x.WindDirectionName.CompareTo(y.WindDirectionName);
            }

            if (x.ClosingSituation.CompareTo(y.ClosingSituation) != 0)
            {
                return x.ClosingSituation.CompareTo(y.ClosingSituation);
            }

            return x.Beta.CompareTo(y.Beta);
        }
    }
}