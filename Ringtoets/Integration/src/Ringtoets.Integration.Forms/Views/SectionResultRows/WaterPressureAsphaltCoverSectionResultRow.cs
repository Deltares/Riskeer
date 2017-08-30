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
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Forms.Views.SectionResultRows
{
    /// <summary>
    /// Class for displaying <see cref="WaterPressureAsphaltCoverFailureMechanismSectionResult"/>  as a row in a grid view.
    /// </summary>
    public class WaterPressureAsphaltCoverSectionResultRow : FailureMechanismSectionResultRow<WaterPressureAsphaltCoverFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaterPressureAsphaltCoverSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="WaterPressureAsphaltCoverFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public WaterPressureAsphaltCoverSectionResultRow(WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult) : base(sectionResult) {}

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                SectionResult.AssessmentLayerThree = value.ToPrecision(SectionResult.AssessmentLayerThree.NumberOfDecimalPlaces);
            }
        }
    }
}