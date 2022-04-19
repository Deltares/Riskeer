﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.Common.Data.AssessmentSection
{
    /// <summary>
    /// Wrapper class representing the reference line with meta data used as a basis for assessment.
    /// </summary>
    public class ReferenceLineMeta
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMeta"/>.
        /// </summary>
        public ReferenceLineMeta()
        {
            ReferenceLine = new ReferenceLine();
        }

        /// <summary>
        /// Gets or sets the unique identifier within a registration.
        /// </summary>
        public string AssessmentSectionId { get; set; }

        /// <summary>
        /// Gets or sets the signal flooding probability for the assessment section.
        /// </summary>
        public int? SignalFloodingProbability { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable flooding probability for the assessment section.
        /// </summary>
        public int MaximumAllowableFloodingProbability { get; set; }

        /// <summary>
        /// Gets the reference line.
        /// </summary>
        public ReferenceLine ReferenceLine { get; }
    }
}