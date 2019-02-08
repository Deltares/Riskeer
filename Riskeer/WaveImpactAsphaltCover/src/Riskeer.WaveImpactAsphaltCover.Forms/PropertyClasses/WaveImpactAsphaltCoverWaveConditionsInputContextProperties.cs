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
using Core.Common.Base.Data;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Revetment.Forms.PropertyClasses;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.Properties;

namespace Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveImpactAsphaltCoverWaveConditionsInputContext"/> for properties panel.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsInputContextProperties
        : AssessmentSectionCategoryWaveConditionsInputContextProperties<WaveImpactAsphaltCoverWaveConditionsInputContext, string>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverWaveConditionsInputContextProperties"/>.
        /// </summary>
        /// <param name="context">The <see cref="WaveImpactAsphaltCoverWaveConditionsInputContext"/> for which 
        /// the properties are shown.</param>
        /// <param name="getAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the assessment level.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverWaveConditionsInputContextProperties(WaveImpactAsphaltCoverWaveConditionsInputContext context,
                                                                          Func<RoundedDouble> getAssessmentLevelFunc,
                                                                          IObservablePropertyChangeHandler propertyChangeHandler)
            : base(context, getAssessmentLevelFunc, propertyChangeHandler) {}

        public override string RevetmentType
        {
            get
            {
                return Resources.WaveImpactAsphaltCoverWaveConditionsInputContextProperties_RevetmentType;
            }
        }
    }
}