﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for generating unique names.
    /// </summary>
    public static class NamingHelper
    {
        /// <summary>
        /// Generate an unique name given a collection of existing named objects.
        /// </summary>
        /// <typeparam name="T">Type of objects in the collection.</typeparam>
        /// <param name="existingObjects">All existing named objects.</param>
        /// <param name="nameBase">The base naming scheme to use.</param>
        /// <param name="nameGetter">Getter method to determine the name of each object in <paramref cref="existingObjects"/>.</param>
        /// <returns>A unique name based on <paramref name="nameBase"/> that is not used
        /// in <paramref name="existingObjects"/>.</returns>
        public static string GetUniqueName<T>(IEnumerable<T> existingObjects, string nameBase, Func<T, string> nameGetter)
        {
            int i = 1;
            string result = nameBase;
            var existingNames = existingObjects.Select(nameGetter).ToArray();
            while (existingNames.Any(name => name.Equals(result)))
            {
                result = string.Format("{0} ({1})", nameBase, i++);
            }
            return result;
        }
    }
}