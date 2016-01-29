// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System.Data.Entity;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Interface for entity persistors.
    /// </summary>
    public interface IEntityPersistor<T1, T2> where T1 : class where T2 : class
    {
        /// <summary>
        /// Insert the <see cref="T2"/>, based upon the <paramref name="model"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="IDbSet{TEntity}"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="model"><see cref="T1"/> to be inserted in the storage.</param>
        /// <returns>New instance of <see cref="T2"/>.</returns>
        T2 AddEntity(T1 model);

        /// <summary>
        /// Updates the <see cref="T2"/>, based upon the <paramref name="model"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="IDbSet{TEntity}"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="model"><see cref="T1"/> to be saved in the storage.</param>
        /// <returns>The <see cref="T2"/>.</returns>
        T2 UpdateEntity(T1 model);
    }
}