// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// Interface for an object that can properly change the <see cref="FailureMechanismContribution.Norm"/>
    /// of an <see cref="IAssessmentSection"/>.
    /// </summary>
    public interface IFailureMechanismContributionNormChangeHandler
    {
        /// <summary>
        /// Change the normative norm type.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/>
        /// is <c>null</c>.</exception>
        void ChangeNormativeNormType(Action action);

        /// <summary>
        /// Change the normative norm.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/>
        /// is <c>null</c>.</exception>
        void ChangeNormativeNorm(Action action);

        /// <summary>
        /// Change the norm.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/>
        /// is <c>null</c>.</exception>
        void ChangeNorm(Action action);
    }
}