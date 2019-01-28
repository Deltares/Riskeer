// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container for Hydra-Ring (cross) section data.
    /// </summary>
    public class HydraRingSection
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingSection"/> class.
        /// </summary>
        /// <param name="sectionId">The unique identifier of the section.</param>
        /// <param name="sectionLength">The length of the section.</param>
        /// <param name="crossSectionNormal">The normal of the cross section.</param>
        public HydraRingSection(int sectionId, double sectionLength, double crossSectionNormal)
        {
            SectionId = sectionId;
            SectionLength = sectionLength;
            CrossSectionNormal = crossSectionNormal;
        }

        /// <summary>
        /// Gets the id of the section.
        /// </summary>
        public int SectionId { get; }

        /// <summary>
        /// Gets the length of the section.
        /// </summary>
        public double SectionLength { get; }

        /// <summary>
        /// Gets the normal of the cross section.
        /// </summary>
        /// <remarks>
        /// The normal corresponds to the angle of the straight line perpendicular to the stretch at the location of the cross section (with respect to the north).
        /// </remarks>
        public double CrossSectionNormal { get; }
    }
}