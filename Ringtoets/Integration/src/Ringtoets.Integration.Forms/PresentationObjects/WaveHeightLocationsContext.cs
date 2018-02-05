// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an enumeration of <see cref="HydraulicBoundaryLocation"/> 
    /// with a wave height calculation result.
    /// </summary>
    public class WaveHeightLocationsContext : ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryLocation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationsContext"/>.
        /// </summary>
        /// <param name="wrappedData">The locations that the <see cref="WaveHeightLocationsContext"/> belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the <see cref="WaveHeightLocationsContext"/> belongs to.</param>
        /// <param name="getCalculationFunc"><see cref="Func{T,TResult}"/> for obtaining a <see cref="HydraulicBoundaryLocationCalculation"/>
        /// based on <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/>, <paramref name="assessmentSection"/>
        /// or <paramref name="getCalculationFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public WaveHeightLocationsContext(ObservableList<HydraulicBoundaryLocation> wrappedData,
                                          IAssessmentSection assessmentSection,
                                          Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc,
                                          string categoryBoundaryName)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getCalculationFunc == null)
            {
                throw new ArgumentNullException(nameof(getCalculationFunc));
            }

            if (string.IsNullOrEmpty(categoryBoundaryName))
            {
                throw new ArgumentException($"'{nameof(categoryBoundaryName)}' must have a value.");
            }

            AssessmentSection = assessmentSection;
            GetCalculationFunc = getCalculationFunc;
            CategoryBoundaryName = categoryBoundaryName;
        }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the <see cref="Func{T,TResult}"/> for obtaining a <see cref="HydraulicBoundaryLocationCalculation"/>
        /// based on <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> GetCalculationFunc { get; }

        /// <summary>
        /// Gets the name of the category boundary.
        /// </summary>
        public string CategoryBoundaryName { get; }
    }
}