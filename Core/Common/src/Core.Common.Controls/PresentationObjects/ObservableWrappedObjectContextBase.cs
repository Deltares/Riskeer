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
using System.Collections.Generic;
using Core.Common.Base;

namespace Core.Common.Controls.PresentationObjects
{
    /// <summary>
    /// This abstract class provides common boilerplate implementations for observable
    /// presentation objects based on a single observable object that needs additional
    /// dependencies or behavior for the UI layer of the application.
    /// </summary>
    /// <typeparam name="T">The observable object type of the wrapped instance.</typeparam>
    public abstract class ObservableWrappedObjectContextBase<T> : WrappedObjectContextBase<T>, IObservable where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableWrappedObjectContextBase{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/> is <c>null</c>.</exception>
        protected ObservableWrappedObjectContextBase(T wrappedData) : base(wrappedData) {}

        #region IObservable implementation

        public IEnumerable<IObserver> Observers
        {
            get
            {
                return WrappedData.Observers;
            }
        }

        public void Attach(IObserver observer)
        {
            WrappedData.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedData.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedData.NotifyObservers();
        }

        #endregion
    }
}