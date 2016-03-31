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

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container for Hydra-Ring (cross) section data.
    /// </summary>
    public class HydraRingSection
    {
        private readonly int sectionId;
        private readonly string sectionName;
        private readonly double sectionBeginCoordinate;
        private readonly double sectionEndCoordinate;
        private readonly double sectionLength;
        private readonly double crossSectionXCoordinate;
        private readonly double crossSectionYCoordinate;
        private readonly double crossSectionNormal;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingSection"/> class.
        /// </summary>
        /// <param name="sectionId">The id of the section.</param>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="sectionBeginCoordinate">The begin coordinate of the section.</param>
        /// <param name="sectionEndCoordinate">The end coordinate of the section.</param>
        /// <param name="sectionLength">The length of the section.</param>
        /// <param name="crossSectionXCoordinate">The x coordinate of the cross section.</param>
        /// <param name="crossSectionYCoordinate">the y coordinate of the cross section.</param>
        /// <param name="crossSectionNormal">The normal of the cross section.</param>
        public HydraRingSection(int sectionId, string sectionName, double sectionBeginCoordinate, double sectionEndCoordinate, double sectionLength, double crossSectionXCoordinate, double crossSectionYCoordinate, double crossSectionNormal)
        {
            this.sectionId = sectionId;
            this.sectionName = sectionName;
            this.sectionBeginCoordinate = sectionBeginCoordinate;
            this.sectionEndCoordinate = sectionEndCoordinate;
            this.sectionLength = sectionLength;
            this.crossSectionXCoordinate = crossSectionXCoordinate;
            this.crossSectionYCoordinate = crossSectionYCoordinate;
            this.crossSectionNormal = crossSectionNormal;
        }

        /// <summary>
        /// Gets the id of the section.
        /// </summary>
        public int SectionId
        {
            get
            {
                return sectionId;
            }
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <remarks>
        /// This name is used in the input file and the output file of Hydra-Ring calculations.
        /// </remarks>
        public string SectionName
        {
            get
            {
                return sectionName;
            }
        }

        /// <summary>
        /// Gets the begin coordinate of the section.
        /// </summary>
        /// <remarks>
        /// If only a cross section is considered, this property should be set equal to <see cref="CrossSectionXCoordinate"/>.
        /// </remarks>
        public double SectionBeginCoordinate
        {
            get
            {
                return sectionBeginCoordinate;
            }
        }

        /// <summary>
        /// Gets the end coordinate of the section.
        /// </summary>
        /// <remarks>
        /// If only a cross section is considered, this property should be set equal to <see cref="CrossSectionYCoordinate"/>.
        /// </remarks>
        public double SectionEndCoordinate
        {
            get
            {
                return sectionEndCoordinate;
            }
        }

        /// <summary>
        /// Gets the length of the section.
        /// </summary>
        public double SectionLength
        {
            get
            {
                return sectionLength;
            }
        }

        /// <summary>
        /// Gets the x coordinate of the cross section.
        /// </summary>
        public double CrossSectionXCoordinate
        {
            get
            {
                return crossSectionXCoordinate;
            }
        }

        /// <summary>
        /// Gets the y coordinate of the cross section.
        /// </summary>
        public double CrossSectionYCoordinate
        {
            get
            {
                return crossSectionYCoordinate;
            }
        }

        /// <summary>
        /// Gets the normal of the cross section.
        /// </summary>
        /// <remarks>
        /// The normal corresponds to the angle of the straight line perpendicular to the stretch at the location of the dike cross section (with respect to the north).
        /// </remarks>
        public double CrossSectionNormal
        {
            get
            {
                return crossSectionNormal;
            }
        }
    }
}