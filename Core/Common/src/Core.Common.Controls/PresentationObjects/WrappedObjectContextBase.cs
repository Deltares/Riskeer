// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;

namespace Core.Common.Controls.PresentationObjects
{
    /// <summary>
    /// This abstract class provides common boilerplate implementations for presentation
    /// objects based on a single object that needs additional dependencies or behavior
    /// for the UI layer of the application.
    /// </summary>
    /// <typeparam name="T">The object type of the wrapped instance.</typeparam>
    public abstract class WrappedObjectContextBase<T> : IEquatable<WrappedObjectContextBase<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedObjectContextBase{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/> is <c>null</c>.</exception>
        protected WrappedObjectContextBase(T wrappedData)
        {
            if (wrappedData == null)
            {
                throw new ArgumentNullException(nameof(wrappedData), @"Wrapped data of context cannot be null.");
            }

            WrappedData = wrappedData;
        }

        /// <summary>
        /// Gets the data wrapped in this presentation object.
        /// </summary>
        public T WrappedData { get; }

        #region IEquatable members

        public virtual bool Equals(WrappedObjectContextBase<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return WrappedData.Equals(other.WrappedData);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WrappedObjectContextBase<T>);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ WrappedData.GetHashCode();
        }

        #endregion
    }
}