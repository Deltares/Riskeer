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

using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>,
    /// which takes care of the representation of properties in a grid.
    /// </summary>
    internal class GrassCoverErosionInwardsSectionResultRow
    {
        private readonly GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsSectionResultRow"/> class.
        /// </summary>
        /// <param name="sectionResult">The section result.</param>
        internal GrassCoverErosionInwardsSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            this.sectionResult = sectionResult;
        }

        /// <summary>
        /// Gets the name of the <see cref="FailureMechanismSection"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return sectionResult.Section.Name;
            }
        }

        /// <summary>
        /// Gets or sets the normative calculation for the section.
        /// </summary>
        public GrassCoverErosionInwardsCalculation Calculation
        {
            get
            {
                return sectionResult.Calculation;
            }
            set
            {
                sectionResult.Calculation = value;
            }
        }
    }
}