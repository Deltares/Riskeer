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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Interface for entity converters.
    /// </summary>
    public interface IEntityConverter<T1, T2>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="T1"/>.
        /// </summary>
        /// <param name="entity">The <see cref="T2"/> to convert.</param>
        /// <returns>A new instance of <see cref="T1"/>, based on the properties of <paramref name="entity"/>.</returns>
        T1 ConvertEntityToModel(T2 entity);

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="T1"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="T2"/> to be saved.</param>
        void ConvertModelToEntity(T1 modelObject, T2 entity);
    }
}