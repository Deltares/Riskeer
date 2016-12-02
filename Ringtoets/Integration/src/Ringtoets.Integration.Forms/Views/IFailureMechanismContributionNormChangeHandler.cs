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
using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// Interface for an object that can properly change the <see cref="FailureMechanismContribution.Norm"/>
    /// of an <see cref="IAssessmentSection"/>.
    /// </summary>
    public interface IFailureMechanismContributionNormChangeHandler
    {
        /// <summary>
        /// Checks to see if the replacement of the norm variable of the assessment section
        /// should occur or not.
        /// </summary>
        /// <returns><c>true</c> if the change should occur, <c>false</c> otherwise.</returns>
        bool ConfirmNormChange();

        /// <summary>
        /// Replaces the <see cref="FailureMechanismContribution.Norm"/> of the <see cref="FailureMechanismContribution"/>
        /// of the given <see cref="IAssessmentSection"/> and propagates the changes to
        /// underlying data structure.
        /// </summary>
        /// <param name="assessmentSection">The section to be updated.</param>
        /// <param name="newNormValue">The new norm value.</param>
        /// <returns>All objects that have been affected by the change.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="newNormValue"/>
        /// is an invalid norm value.</exception>
        IEnumerable<IObservable> ChangeNorm(IAssessmentSection assessmentSection, double newNormValue);
    }
}