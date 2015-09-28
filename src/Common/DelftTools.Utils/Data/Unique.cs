using System;
using System.ComponentModel;

namespace DelftTools.Utils.Data
{
    //for reference: http://code.google.com/p/unhaddins/source/browse/uNhAddIns/uNhAddIns.Entities/AbstractEntity.cs

    /// <summary>
    /// TODO: rename to Entity'T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Unique<T> : IUnique<T>
    {
        private int requestedHashCode = -1;

        [Browsable(false)]
        public virtual T Id { get; set; }

        /// <summary>
        /// Required in case if object is wrapped with a Proxy class.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetEntityType()
        {
            return GetType();
        }

        private static bool IsEntityPersistent(IUnique<T> e)
        {
            return e != null && !Equals(e.Id, default(T));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is IUnique<T>)
            {
                return Equals((IUnique<T>) obj);
            }
            return false;
        }

        public virtual bool Equals(IUnique<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (IsEntityPersistent(this) && IsEntityPersistent(other) && Equals(other.Id, Id))
            {
                var otherType = other.GetEntityType();
                var thisType = GetEntityType();

                return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        //The value of GetHashCode should not be allowed to change during an objects lifetime!! If it does, it 
        //leads to crazy problems with Dictionaries etc (as they 'cache' the value)
        public override int GetHashCode()
        {
            if (requestedHashCode == -1)
            {
                requestedHashCode = IsEntityPersistent(this) ? Id.GetHashCode() : base.GetHashCode();
            }
            return requestedHashCode;
        }

        public static bool operator ==(Unique<T> left, Unique<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Unique<T> left, Unique<T> right)
        {
            return !Equals(left, right);
        }
    }
}