﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Properties;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="CustomProbabilityFailureMechanismSectionResult"/>, which takes care of the
    /// representation of properties in a grid.
    /// </summary>
    public class CustomProbabilityFailureMechanismSectionResultRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="CustomProbabilityFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="CustomProbabilityFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="sectionResult"/> is
        /// <c>null</c>.</exception>
        public CustomProbabilityFailureMechanismSectionResultRow(CustomProbabilityFailureMechanismSectionResult sectionResult)
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
        /// Gets or sets the value representing whether the section passed the layer 0 assessment.
        /// </summary>
        public bool AssessmentLayerOne
        {
            get
            {
                return SectionResult.AssessmentLayerOne;
            }
            set
            {
                SectionResult.AssessmentLayerOne = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the layer 2a assessment.
        /// </summary>
        public string AssessmentLayerTwoA
        {
            get
            {
                var d = (RoundedDouble) (1/SectionResult.AssessmentLayerTwoA);
                return string.Format(Resources.ProbabilityPerYearFormat, d);
            }
            set
            {
                SectionResult.AssessmentLayerTwoA = (RoundedDouble) double.Parse(value);
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the layer 2b assessment.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoB
        {
            get
            {
                return SectionResult.AssessmentLayerTwoB;
            }
            set
            {
                SectionResult.AssessmentLayerTwoB = value;
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the layer 3 assessment.
        /// </summary>
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

        private CustomProbabilityFailureMechanismSectionResult SectionResult { get; set; }
    }
}