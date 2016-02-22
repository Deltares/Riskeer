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
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;

namespace Ringtoets.Integration.Data.Map
{
    /// <summary>
    /// The data object with the <see cref="MapData"/> for the <see cref="AssessmentSectionBase"/>.
    /// </summary>
    public class AssessmentSectionMapData : MapDataCollection, IEquatable<AssessmentSectionMapData>
    {
        public AssessmentSectionBase AssessmentSection { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMapData"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSectionBase"/> that links to this <see cref="AssessmentSectionMapData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public AssessmentSectionMapData(AssessmentSectionBase assessmentSection) : base(new List<MapData>())
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }
            
            AssessmentSection = assessmentSection;

            CheckHydraulicBoundaryDatabaseLocationData();
        }

        public void UpdateHydraulicBoundaryDatabaseMap()
        {
            List.Clear();

            CheckHydraulicBoundaryDatabaseLocationData();
        }

        private void CheckHydraulicBoundaryDatabaseLocationData()
        {
            var locationData = GetHydraulicBoundaryDatabaseLocationData();
            if (locationData.Count > 0)
            {
                CreateMapPointData(locationData);
            }
        }

        private List<Point2D> GetHydraulicBoundaryDatabaseLocationData()
        {
            return AssessmentSection.HydraulicBoundaryDatabase.Locations.Select(hydraulicBoundaryLocation => hydraulicBoundaryLocation.Location).ToList();
        }

        private void CreateMapPointData(IEnumerable<Point2D> locationData)
        {
            List.Add(new MapPointData(locationData));
        }

        #region IEquatable implementation

        public bool Equals(AssessmentSectionMapData other)
        {
            return other != null && Equals(other.AssessmentSection, AssessmentSection);
        }

        public override bool Equals(object other)
        {
            return Equals(other as AssessmentSectionMapData);
        }

        public override int GetHashCode()
        {
            return AssessmentSection.GetHashCode();
        }

        #endregion
    }
}