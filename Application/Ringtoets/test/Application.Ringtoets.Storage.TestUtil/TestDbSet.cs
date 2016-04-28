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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Application.Ringtoets.Storage.TestUtil
{
    public class TestDbSet<T> : DbSet<T>, IDbSet<T> where T : class
    {
        private readonly IQueryable<T> queryable;
        private readonly ObservableCollection<T> collection;

        public TestDbSet(ObservableCollection<T> queryable)
        {
            collection = queryable;
            this.queryable = queryable.AsQueryable();
        }

        public IQueryProvider Provider
        {
            get
            {
                return queryable.Provider;
            }
        }

        public Expression Expression
        {
            get
            {
                return queryable.Expression;
            }
        }

        public Type ElementType
        {
            get
            {
                return queryable.ElementType;
            }
        }

        public override ObservableCollection<T> Local
        {
            get
            {
                return collection;
            }
        }

        public override IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            var list = entities.ToList();
            foreach (var e in list)
            {
                collection.Remove(e);
            }
            return entities;
        }

        public override T Add(T entity)
        {
            collection.Add(entity);
            return entity;
        }

        public override T Remove(T entity)
        {
            collection.Remove(entity);
            return entity;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}