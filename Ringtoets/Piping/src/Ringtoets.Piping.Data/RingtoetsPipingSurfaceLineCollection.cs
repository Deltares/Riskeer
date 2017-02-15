// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Collection to store <see cref="RingtoetsPipingSurfaceLine"/>.
    /// </summary>
    public class RingtoetsPipingSurfaceLineCollection : ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>
    {
        private const string separator = ", ";

        protected override void ValidateItems(IEnumerable<RingtoetsPipingSurfaceLine> items)
        {
            IEnumerable<IGrouping<string, RingtoetsPipingSurfaceLine>> duplicateItems =
                items.GroupBy(item => item.Name)
                     .Where(group => group.Count() > 1);

            if (duplicateItems.Any())
            {
                var names = string.Join(separator, duplicateItems.Select(group => group.First()));
                string message = string.Format(
                    Resources.RingtoetsPipingSurfaceLineCollection_ValidateItems_RingtoetsPipingSurfaceLine_require_unique_names_found_duplicate_items_0_,
                    names);
                throw new ArgumentException(message);
            }
        }
    }
}