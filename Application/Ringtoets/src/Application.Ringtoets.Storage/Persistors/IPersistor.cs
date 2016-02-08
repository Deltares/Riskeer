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

using System.Collections.Generic;
using System.Data.Entity;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Interface that describes the methods that must be implemented on classes that are storage persistors.
    /// </summary>
    public interface IPersistor<TEntity, TModel> where TEntity : class where TModel : IStorable
    {
        /// <summary>
        /// Ensures that the <paramref name="model"/> is set as <see cref="TEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// All other <see cref="TEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="TModel"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model">The <see cref="TModel"/> to be saved in the storage.</param>
        /// <param name="order">Value used for sorting.</param>
        void UpdateModel(ICollection<TEntity> parentNavigationProperty, TModel model, int order);

        /// <summary>
        /// Ensures that the <paramref name="model"/> is added as <see cref="TEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// All other <see cref="TEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="TEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model">The <see cref="TModel"/> to be saved in the storage.</param>
        /// <param name="order">Value used for sorting.</param>
        void InsertModel(ICollection<TEntity> parentNavigationProperty, TModel model, int order);

        /// <summary>
        /// Removes all entities from <see cref="IRingtoetsEntities.ProjectEntities"/> that are not marked as 'updated'.
        /// </summary>
        /// <param name="parentNavigationProperty">List where <see cref="TEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        void RemoveUnModifiedEntries(ICollection<TEntity> parentNavigationProperty);

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        void PerformPostSaveActions();

        /// <summary>
        /// Loads the <see cref="TEntity"/> as <see cref="TModel"/> from <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="TEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <returns>List of <see cref="TModel"/>.</returns>
        IEnumerable<TModel> LoadModels(ICollection<TEntity> parentNavigationProperty);

        /// <summary>
        /// Updates the children of <paramref name="model"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="model">The <see cref="TModel"/> of which children need to be updated.</param>
        /// <param name="entity">Referenced <see cref="TEntity"/>.</param>
        void UpdateChildren(TModel model, TEntity entity);

        /// <summary>
        /// Inserts the children of <paramref name="model"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="model">The <see cref="TModel"/> of which children need to be inserted.</param>
        /// <param name="entity">Referenced <see cref="TEntity"/>.</param>
        void InsertChildren(TModel model, TEntity entity);
    }
}