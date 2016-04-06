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
using Ringtoets.Common.Data;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for a <see cref="IFailureMechanism"/> instance.
    /// </summary>
    public abstract class FailureMechanismContext<T> where T : IFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismContext{T}"/> class.
        /// </summary>
        /// <param name="wrapped">The failure mechanism.</param>
        /// <param name="parent">The parent of <paramref name="wrapped"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrapped"/> or <paramref name="parent"/> are <c>null</c>.</exception>
        protected FailureMechanismContext(T wrapped, IAssessmentSection parent)
        {
            AssertInputsAreNotNull(wrapped, parent);

            WrappedData = wrapped;
            Parent = parent;
        }

        /// <summary>
        /// Gets the parent of <see cref="WrappedData"/>.
        /// </summary>
        public IAssessmentSection Parent { get; private set; }

        /// <summary>
        /// Gets the wrapped failure mechanism.
        /// </summary>
        public T WrappedData { get; private set; }

        private void AssertInputsAreNotNull(T wrappedFailureMechanism, IAssessmentSection parent)
        {
            if (wrappedFailureMechanism == null)
            {
                throw new ArgumentNullException("wrappedFailureMechanism", "Failure mechanism cannot be null.");
            }

            if (parent == null)
            {
                throw new ArgumentNullException("parent", "The assessment section cannot be null.");
            }
        }

        #region Equals implementation

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((FailureMechanismContext<T>) obj);
        }

        public override int GetHashCode()
        {
            return WrappedData.GetHashCode();
        }

        private bool Equals(FailureMechanismContext<T> other)
        {
            return WrappedData.Equals(other.WrappedData);
        }

        #endregion
    }
}