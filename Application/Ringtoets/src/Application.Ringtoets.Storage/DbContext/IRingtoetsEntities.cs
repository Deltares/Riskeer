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
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Interface that describes the properties and methods that must be implemented on classes that extend from a database context.
    /// </summary>
    public interface IRingtoetsEntities
    {
        /// <summary>
        /// <see cref="IDbSet{TEntity}"/> of <see cref="ProjectEntity"/>
        /// </summary>
        IDbSet<ProjectEntity> ProjectEntities { get; }

        /// <summary> 
        /// Persists all updates to the database and resets change tracking in the object context, see <see cref="ObjectContext.SaveChanges()"/>.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database. This can include state entries for entities and/or relationships. 
        /// Relationship state entries are created for many-to-many relationships and relationships where there is no foreign key property included in the entity class 
        /// (often referred to as independent associations).</returns>
        /// <exception cref="OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
        int SaveChanges();
    }
}