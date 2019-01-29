// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Core.Common.Base
{
    /// <summary>
    /// Class that implements <see cref="IObserver"/> in a way that a single <see cref="IObservable"/> can be observed.
    /// </summary>
    /// <remarks>
    /// The <see cref="Observable"/> being observed by instances of this class can be dynamically changed.
    /// </remarks>
    public class Observer : IObserver, IDisposable
    {
        private readonly Action updateObserverAction;
        private IObservable observable;

        /// <summary>
        /// Creates a new instance of the <see cref="Observer"/> class.
        /// </summary>
        /// <param name="updateObserverAction">The <see cref="UpdateObserver"/> action to perform on notifications coming from <see cref="Observable"/>.</param>
        public Observer(Action updateObserverAction)
        {
            this.updateObserverAction = updateObserverAction;
        }

        /// <summary>
        /// Gets or sets the object to observe.
        /// </summary>
        public IObservable Observable
        {
            get
            {
                return observable;
            }
            set
            {
                observable?.Detach(this);

                observable = value;

                observable?.Attach(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateObserver()
        {
            updateObserverAction();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Observable = null;
            }
        }
    }
}