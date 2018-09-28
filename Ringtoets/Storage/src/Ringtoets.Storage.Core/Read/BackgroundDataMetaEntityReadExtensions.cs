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
using System.Collections.Generic;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read
{
    /// <summary>
    /// Extensions methods for read operations based on the <see cref="BackgroundDataMetaEntity"/>.
    /// </summary>
    internal static class BackgroundDataMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="BackgroundDataMetaEntity"/> and use the information
        /// to construct a <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="BackgroundDataMetaEntity"/>
        /// to create <see cref="KeyValuePair{TKey,TValue}"/> for.</param>
        /// <returns>A new <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="entity"/> is <c>null</c>.</exception>
        internal static KeyValuePair<string, string> Read(this BackgroundDataMetaEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new KeyValuePair<string, string>(entity.Key, entity.Value);
        }
    }
}