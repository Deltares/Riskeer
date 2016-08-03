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
using System.Data.Entity;

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class contains methods which can be performed on the IStorable interface.
    /// </summary>
    internal static class IStorableExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="IStorable"/> is considered new from the database's perspective.
        /// </summary>
        /// <param name="storable">The <see cref="IStorable"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="storable"/> is considered new, <c>false</c> otherwise.</returns>
        internal static bool IsNew(this IStorable storable)
        {
            return storable.StorageId <= 0;
        }

        /// <summary>
        /// Gets the corresponding Entity from the database based on the <paramref name="storable"/>.
        /// </summary>
        /// <param name="storable">The <see cref="IStorable"/> corresponding with the Entity.</param>
        /// <param name="entitySet">The Entity set to obtain the existing entity from.</param>
        /// <param name="context"></param>
        /// <returns>The stored <see cref="IStorable"/> as an Entity of type <typeparamref name="T"/>.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when either:
        /// <list type="bullet">
        /// <item>the Entity of type <typeparamref name="T"/> couldn't be found in the <paramref name="entitySet"/></item>
        /// <item>more than one Entity of type <typeparamref name="T"/> was found in the <paramref name="entitySet"/></item>
        /// <item><see cref="storable"/> has not yet been saved in the database</item>
        /// </list></exception>
        public static T GetCorrespondingEntity<T>(this IStorable storable, DbSet<T> entitySet, IRingtoetsEntities context) where T : class
        {
            if (storable.IsNew())
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(T).Name, storable.StorageId));
            }
            try
            {
                context.AutoDetectChangesEnabled = false;
                T entity = entitySet.Find(storable.StorageId);
                if (entity == null)
                {
                    throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(T).Name, storable.StorageId));
                }
                return entity;
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(T).Name, storable.StorageId), exception);
            }
            finally
            {
                context.AutoDetectChangesEnabled = true;
            }
        }
    }
}