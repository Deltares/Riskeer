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
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for <see cref="IFailureMechanism.Sections"/>.
    /// </summary>
    public class FailureMechanismSectionsContext : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSectionsContext"/> class.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism whose sections to wrap.</param>
        /// <param name="assessmentSection">The owning assessment section of <paramref name="failureMechanism"/>.</param>
        /// <exception cref="System.ArgumentNullException">When any input argument is null.</exception>
        public FailureMechanismSectionsContext(IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            ParentFailureMechanism = failureMechanism;
            ParentAssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the sequence of <see cref="FailureMechanismSection"/> instances available 
        /// on the wrapped failure mechanism.
        /// </summary>
        public IEnumerable<FailureMechanismSection> WrappedData
        {
            get
            {
                return ParentFailureMechanism.Sections;
            }
        }

        /// <summary>
        /// Gets failure mechanism that owns the sequence exposed by <see cref="WrappedData"/>.
        /// </summary>
        public IFailureMechanism ParentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the assessment section that owns <see cref="ParentFailureMechanism"/>.
        /// </summary>
        public IAssessmentSection ParentAssessmentSection { get; private set; }

        #region IObservable

        public void Attach(IObserver observer)
        {
            ParentFailureMechanism.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            ParentFailureMechanism.Detach(observer);
        }

        public void NotifyObservers()
        {
            ParentFailureMechanism.NotifyObservers();
        }

        #endregion

        #region Equatable

        private bool Equals(FailureMechanismSectionsContext other)
        {
            return Equals(ParentFailureMechanism, other.ParentFailureMechanism);
        }

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
            return Equals((FailureMechanismSectionsContext) obj);
        }

        public override int GetHashCode()
        {
            return ParentFailureMechanism.GetHashCode();
        }

        #endregion
    }
}