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
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// The presentation object for <see cref="PipingFailureMechanism.StochasticSoilModels"/>.
    /// </summary>
    public class StochasticSoilModelContext : IObservable
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelContext"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to wrap.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public StochasticSoilModelContext(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        public PipingFailureMechanism FailureMechanism { get; private set; }
        public IAssessmentSection AssessmentSection { get; private set; }

        #region IObservable

        public void Attach(IObserver observer)
        {
            FailureMechanism.StochasticSoilModels.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            FailureMechanism.StochasticSoilModels.Detach(observer);
        }

        public void NotifyObservers()
        {
            FailureMechanism.StochasticSoilModels.NotifyObservers();
        }

        #endregion

        #region Equatible

        private bool Equals(StochasticSoilModelContext other)
        {
            return Equals(FailureMechanism, other.FailureMechanism);
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
            return Equals((StochasticSoilModelContext) obj);
        }

        public override int GetHashCode()
        {
            return FailureMechanism.GetHashCode();
        }

        #endregion
    }
}