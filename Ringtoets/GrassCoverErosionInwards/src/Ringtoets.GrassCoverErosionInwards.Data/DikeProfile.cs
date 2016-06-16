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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Definition for a dike profile for the Grass Cover Erosion Inwards failure mechanism.
    /// </summary>
    public class DikeProfile
    {
        private readonly List<ProfileSection> foreshoreGeometry;
        private readonly List<RoughnessProfileSection> dikeGeometry;
        private RoundedDouble orientation;
        private RoundedDouble crestLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DikeProfile"/> class.
        /// </summary>
        /// <param name="worldCoordinate">The value for <see cref="WorldReferencePoint"/>.</param>
        public DikeProfile(Point2D worldCoordinate)
        {
            if (worldCoordinate == null)
            {
                throw new ArgumentNullException("worldCoordinate");
            }

            orientation = new RoundedDouble(2);
            crestLevel = new RoundedDouble(2);

            Name = Resources.DikeProfile_DefaultName;
            Memo = "";
            dikeGeometry = new List<RoughnessProfileSection>();
            foreshoreGeometry = new List<ProfileSection>();
            WorldReferencePoint = worldCoordinate;
        }

        /// <summary>
        /// Gets or sets the orientation of the dike profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="value"/> is
        /// not in range [0, 360].</exception>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                if (value < 0.0 || value > 360.0)
                {
                    string message = string.Format(Resources.DikeProfile_Orientation_Value_0_should_be_in_interval, value.Value);
                    throw new ArgumentOutOfRangeException("value", message);
                }

                orientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets the reference point in world coordinates corresponding to the local coordinate <see cref="X0"/>.
        /// </summary>
        public Point2D WorldReferencePoint { get; private set; }

        /// <summary>
        /// Gets or sets the local x-coordinate corresponding to the world reference point <see cref="WorldReferencePoint"/>.
        /// </summary>
        public double X0 { get; set; }

        /// <summary>
        /// Gets or sets the name of the dike profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the break water object of the dike profile, if any.
        /// </summary>
        public BreakWater BreakWater { get; set; }

        /// <summary>
        /// Gets the geometry of the dike with roughness data.
        /// </summary>
        public IEnumerable<RoughnessProfileSection> DikeGeometry
        {
            get
            {
                return dikeGeometry;
            }
        }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        public IEnumerable<ProfileSection> ForeshoreGeometry
        {
            get
            {
                return foreshoreGeometry;
            }
        }

        /// <summary>
        /// Gets or sets the height of the dike [m+NAP].
        /// </summary>
        public RoundedDouble CrestLevel
        {
            get
            {
                return crestLevel;
            }
            set
            {
                crestLevel = value.ToPrecision(crestLevel.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the optional notes about this instance.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Indicates if there is a break water object available for this instance or not.
        /// </summary>
        public bool HasBreakWater
        {
            get
            {
                return BreakWater != null;
            }
        }

        /// <summary>
        /// Adds a geometry section to <see cref="ForeshoreGeometry"/>.
        /// </summary>
        /// <param name="section">The new section to add.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="section"/>
        /// is either not connected to <see cref="ForeshoreGeometry"/> or is connected but
        /// has an incorrect orientation.</exception>
        public void AddForshoreGeometrySection(ProfileSection section)
        {
            AddProfileSection(foreshoreGeometry, section);
        }

        /// <summary>
        /// Adds a geometry section to <see cref="DikeGeometry"/>.
        /// </summary>
        /// <param name="roughnessProfileSection">The new section to add.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="roughnessProfileSection"/>
        /// is either not connected to <see cref="DikeGeometry"/> or is connected but has
        /// an incorrect orientation.</exception>
        public void AddDikeGeometrySection(RoughnessProfileSection roughnessProfileSection)
        {
            AddProfileSection(dikeGeometry, roughnessProfileSection);
        }

        /// <summary>
        /// Adds a profile section to an existing collection profile section, but only if
        /// it can be connected properly.
        /// </summary>
        /// <typeparam name="T">The type of profile section.</typeparam>
        /// <param name="profileSections">Collection of already defined geometry sections.</param>
        /// <param name="section">The section to add to <paramref name="profileSections"/>.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="section"/>
        /// is either not connected to <paramref name="profileSections"/> or is connected
        /// but has an incorrect orientation.</exception>
        private static void AddProfileSection<T>(IList<T> profileSections, T section) where T : ProfileSection
        {
            if (profileSections.Count == 0)
            {
                profileSections.Add(section);
                return;
            }

            ProfileSection startingSection = profileSections.First();
            ProfileSection endingSection = profileSections.Last();
            if (section.StartingPoint.Equals(startingSection.StartingPoint) ||
                section.EndingPoint.Equals(endingSection.EndingPoint))
            {
                throw new ArgumentException(Resources.DikeProfile_AddProfileSection_New_segment_connected_with_incorrect_orientation);
            }

            if (section.EndingPoint.Equals(startingSection.StartingPoint))
            {
                profileSections.Insert(0, section);
            }
            else if (section.StartingPoint.Equals(endingSection.EndingPoint))
            {
                profileSections.Add(section);
            }
            else
            {
                throw new ArgumentException(Resources.DikeProfile_AddProfileSection_New_segment_not_connected);
            }
        }
    }
}