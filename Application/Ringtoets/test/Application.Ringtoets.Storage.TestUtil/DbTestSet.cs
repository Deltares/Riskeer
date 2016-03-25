using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Application.Ringtoets.Storage.TestUtil
{
    public static class DbTestSet
    {
        public static DbSet<T> GetDbTestSet<T>(ObservableCollection<T> data) where T : class
        {
            return new TestDbSet<T>(data);
        }

    }
    public class TestDbSet<T> : DbSet<T>, IDbSet<T> where T: class
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

        public override IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            foreach(var e in entities)
            {
                collection.Remove(e);
            }
            return entities;
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