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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HeightStructuresFailureMechanismSectionResult"/>.
    /// </summary>
    internal class HeightStructuresFailureMechanismSectionResultRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="HeightStructuresFailureMechanismSectionResult"/> this row contains.</param>
        public HeightStructuresFailureMechanismSectionResultRow(HeightStructuresFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            SectionResult = sectionResult;
        }

        /// <summary>
        /// Gets the name of the <see cref="FailureMechanismSection"/> name.
        /// </summary>
        public string Name
        {
            get
            {
                return SectionResult.Section.Name;
            }
        }

        /// <summary>
        /// Gets or sets the assessment layer one of the <see cref="SectionResult"/>.
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
        /// Gets the assessment layer two a of the <see cref="SectionResult"/>.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoA
        {
            get
            {
                return SectionResult.AssessmentLayerTwoA;
            }
        }

        /// <summary>
        /// Gets or sets the assessment layer two b of the <see cref="SectionResult"/>.
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
        /// Gets or sets the assessment layer three of the <see cref="SectionResult"/>.
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

        private HeightStructuresFailureMechanismSectionResult SectionResult { get; set; }
    }
}