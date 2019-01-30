// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for dune location calculations.
    /// </summary>
    public class DuneLocationCalculationsContext : ObservableWrappedObjectContextBase<IObservableEnumerable<DuneLocationCalculation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculations the context belongs to.</param>
        /// <param name="failureMechanism">The dune erosion failure mechanism which the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <param name="getNormFunc"><see cref="Func{TResult}"/> for obtaining the norm to use during calculations.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/>, <paramref name="failureMechanism"/>,
        /// <paramref name="assessmentSection"/> or <paramref name="getNormFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public DuneLocationCalculationsContext(IObservableEnumerable<DuneLocationCalculation> wrappedData,
                                               DuneErosionFailureMechanism failureMechanism,
                                               IAssessmentSection assessmentSection,
                                               Func<double> getNormFunc,
                                               string categoryBoundaryName)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNormFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormFunc));
            }

            if (string.IsNullOrEmpty(categoryBoundaryName))
            {
                throw new ArgumentException($"'{nameof(categoryBoundaryName)}' must have a value.");
            }

            AssessmentSection = assessmentSection;
            FailureMechanism = failureMechanism;
            GetNormFunc = getNormFunc;
            CategoryBoundaryName = categoryBoundaryName;
        }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the <see cref="Func{TResult}"/> for obtaining the norm to use during calculations.
        /// </summary>
        public Func<double> GetNormFunc { get; }

        /// <summary>
        /// Gets the name of the category boundary.
        /// </summary>
        public string CategoryBoundaryName { get; }
    }
}