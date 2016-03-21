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
using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Interface for entity converters.
    /// </summary>
    public interface IEntityConverter<TModel, TEntity> where TEntity : class where TModel : IStorable
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="TModel"/>.
        /// </summary>
        /// <param name="entity">The <see cref="TEntity"/> to convert.</param>
        /// <param name="model">The <see cref="Func{TResult}"/> to obtain the model.</param>
        /// <returns>A new instance of <see cref="TModel"/>, based on the properties of <paramref name="entity"/>.</returns>
        TModel ConvertEntityToModel(TEntity entity, Func<TModel> model);

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="TModel"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="TEntity"/> to be saved.</param>
        void ConvertModelToEntity(TModel modelObject, TEntity entity);
    }
}