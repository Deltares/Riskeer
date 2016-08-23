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
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.Views
{
    /// <summary>
    /// Class for displaying <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>
    /// as a row in a grid view.
    /// </summary>
    public class StabilityStoneCoverSectionResultRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>
        /// to wrap so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public StabilityStoneCoverSectionResultRow(StabilityStoneCoverFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            SectionResult = sectionResult;
        }

        /// <summary>
        /// Gets the name of the failure mechanism section.
        /// </summary>
        public string Name
        {
            get
            {
                return SectionResult.Section.Name;
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the <see cref="StabilityStoneCoverFailureMechanismSectionResult.AssessmentLayerTwoA"/>.
        /// </summary>
        public AssessmentLayerTwoAResult AssessmentLayerTwoA
        {
            get
            {
                return SectionResult.AssessmentLayerTwoA;
            }
            set
            {
                SectionResult.AssessmentLayerTwoA = value;
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the <see cref="StabilityStoneCoverFailureMechanismSectionResult.AssessmentLayerThree"/>.
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
                SectionResult.AssessmentLayerThree = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="StabilityStoneCoverFailureMechanismSectionResult"/> that is the source of this row.
        /// </summary>
        private StabilityStoneCoverFailureMechanismSectionResult SectionResult { get; set; }
    }
}