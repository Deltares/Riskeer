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

using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.IO
{
    /// <summary>
    /// This class represents settings that are defined at <see cref="IAssessmentSection"/>
    /// level.
    /// </summary>
    public class AssessmentSectionSettings
    {
        private AssessmentSectionSettings(string id, int n, bool isDune)
        {
            AssessmentSectionId = id;
            N = n;
            IsDune = isDune;
        }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        public int N { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a dune assessment section or not.
        /// </summary>
        public bool IsDune { get; }

        /// <summary>
        /// Gets the assessment section identifier, that can be used to match to <see cref="IAssessmentSection.Name"/>.
        /// </summary>
        public string AssessmentSectionId { get; }

        /// <summary>
        /// Creates the settings for a dike assessment section.
        /// </summary>
        /// <param name="id">The identifier of the assessment section.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>A fully configured <see cref="AssessmentSectionSettings"/>.</returns>
        public static AssessmentSectionSettings CreateDikeAssessmentSectionSettings(string id, int n)
        {
            return new AssessmentSectionSettings(id, n, false);
        }

        /// <summary>
        /// Creates the settings for a dune assessment section.
        /// </summary>
        /// <param name="id">The identifier of the assessment section.</param>
        /// <returns>A fully configured <see cref="AssessmentSectionSettings"/>.</returns>
        public static AssessmentSectionSettings CreateDuneAssessmentSectionSettings(string id)
        {
            return new AssessmentSectionSettings(id, 3, true);
        }
    }
}