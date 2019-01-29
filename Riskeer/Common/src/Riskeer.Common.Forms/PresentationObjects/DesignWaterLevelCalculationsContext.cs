// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an enumeration of <see cref="HydraulicBoundaryLocationCalculation"/>
    /// with a design water level calculation result.
    /// </summary>
    public class DesignWaterLevelCalculationsContext : ObservableWrappedObjectContextBase<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculations that the <see cref="DesignWaterLevelCalculationsContext"/> belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the <see cref="DesignWaterLevelCalculationsContext"/> belongs to.</param>
        /// <param name="getNormFunc"><see cref="Func{TResult}"/> for obtaining the norm to use during calculations.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/>, <paramref name="assessmentSection"/> or
        /// <paramref name="getNormFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public DesignWaterLevelCalculationsContext(IObservableEnumerable<HydraulicBoundaryLocationCalculation> wrappedData,
                                                   IAssessmentSection assessmentSection,
                                                   Func<double> getNormFunc,
                                                   string categoryBoundaryName)
            : base(wrappedData)
        {
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
            GetNormFunc = getNormFunc;
            CategoryBoundaryName = categoryBoundaryName;
        }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the <see cref="Func{TResult}"/> for obtaining the norm to use during calculations.
        /// </summary>
        public Func<double> GetNormFunc { get; }

        /// <summary>
        /// Gets the name of the category boundary.
        /// </summary>
        public string CategoryBoundaryName { get; }

        public override bool Equals(WrappedObjectContextBase<IObservableEnumerable<HydraulicBoundaryLocationCalculation>> other)
        {
            return base.Equals(other)
                   && other is DesignWaterLevelCalculationsContext
                   && CategoryBoundaryName.Equals(((DesignWaterLevelCalculationsContext) other).CategoryBoundaryName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DesignWaterLevelCalculationsContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ CategoryBoundaryName.GetHashCode();
        }
    }
}