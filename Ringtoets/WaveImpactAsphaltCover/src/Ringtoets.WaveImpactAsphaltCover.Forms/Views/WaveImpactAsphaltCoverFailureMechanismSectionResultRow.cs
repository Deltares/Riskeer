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
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Views
{
    /// <summary>
    /// Class for displaying <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult"/>  as a row in a grid view.
    /// </summary>
    internal class WaveImpactAsphaltCoverFailureMechanismSectionResultRow
    {
        private readonly WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult;

        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverFailureMechanismSectionResultRow(WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            this.sectionResult = sectionResult;
        }

        /// <summary>
        /// Gets the name of the failure mechanism section.
        /// </summary>
        public string Name
        {
            get
            {
                return sectionResult.Section.Name;
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult.AssessmentLayerOne"/>.
        /// </summary>
        public bool AssessmentLayerOne
        {
            get
            {
                return sectionResult.AssessmentLayerOne;
            }
            set
            {
                sectionResult.AssessmentLayerOne = value;
                sectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult.AssessmentLayerTwoA"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble AssessmentLayerTwoA
        {
            get
            {
                return sectionResult.AssessmentLayerTwoA;
            }
            set
            {
                sectionResult.AssessmentLayerTwoA = value;
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult.AssessmentLayerThree"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return sectionResult.AssessmentLayerThree;
            }
            set
            {
                sectionResult.AssessmentLayerThree = value;
            }
        }
    }
}