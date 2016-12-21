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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// Class for displaying <see cref="DuneErosionFailureMechanismSectionResult"/> as a row in a grid view.
    /// </summary>
    public class DuneErosionSectionResultRow : FailureMechanismSectionResultRow<DuneErosionFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="DuneErosionFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public DuneErosionSectionResultRow(DuneErosionFailureMechanismSectionResult sectionResult) : base(sectionResult) {}

        /// <summary>
        /// Gets the assessment layer two a of the <see cref="Ringtoets.DuneErosion.Data.DuneErosionFailureMechanismSectionResult"/>.
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
    }
}