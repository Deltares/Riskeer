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

using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// This class contains methods which can be performed on the IStorable interface.
    /// </summary>
    internal static class IStorableExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="IStorable"/> is concidered new from the database's perspective.
        /// </summary>
        /// <param name="storable">The <see cref="IStorable"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="storable"/> is concidered new, <c>false</c> otherwise.</returns>
        internal static bool IsNew(this IStorable storable)
        {
            return storable.StorageId <= 0;
        } 
    }
}