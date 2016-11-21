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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating arrays of <see cref="MapFeature"/> to use in <see cref="MapData"/>
    /// (created via <see cref="RingtoetsMapDataFactory"/>).
    /// </summary>
    public static class RingtoetsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create reference line features based on the provided <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> to create the reference line features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="referenceLine"/> is <c>null</c>.</returns>
        public static MapFeature[] CreateReferenceLineFeatures(ReferenceLine referenceLine)
        {
            return referenceLine != null
                       ? new[]
                       {
                           GetAsSingleMapFeature(referenceLine.Points)
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create hydraulic boundary database location features based on the provided <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> to create the location features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</returns>
        public static MapFeature[] CreateHydraulicBoundaryDatabaseFeatures(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var features = new List<MapFeature>();

            if (hydraulicBoundaryDatabase != null)
            {
                foreach (var location in hydraulicBoundaryDatabase.Locations)
                {
                    var feature = GetAsSingleMapFeature(location.Location);

                    feature.MetaData["ID"] = location.Id;
                    feature.MetaData["Naam"] = location.Name;
                    feature.MetaData["Toetspeil"] = location.DesignWaterLevel;
                    feature.MetaData["Golfhoogte"] = location.WaveHeight;

                    features.Add(feature);
                }
            }

            return features.ToArray();
        }

        /// <summary>
        /// Create section features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create the section features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="sections"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateFailureMechanismSectionFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new[]
                       {
                           new MapFeature(sections.Select(section => new MapGeometry(new[]
                           {
                               section.Points.Select(p => new Point2D(p.X, p.Y))
                           })))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create section start point features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create the section start point features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="sections"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateFailureMechanismSectionStartPointFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new[]
                       {
                           GetAsSingleMapFeature(sections.Select(sl => sl.GetStart()))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create section end point features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create the section end point features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="sections"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateFailureMechanismSectionEndPointFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new []
                       {
                           GetAsSingleMapFeature(sections.Select(sl => sl.GetLast()))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="dikeProfiles"/>.
        /// </summary>
        /// <param name="dikeProfiles">The profiles to create features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="dikeProfiles"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateDikeProfilesFeatures(IEnumerable<DikeProfile> dikeProfiles)
        {
            if (dikeProfiles == null || !dikeProfiles.Any())
            {
                return new MapFeature[0];
            }

            return dikeProfiles.Select(dikeProfile => GetAsSingleMapFeature(GetWorldPoints(dikeProfile))).ToArray();
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="foreshoreProfiles"/>.
        /// </summary>
        /// <param name="foreshoreProfiles">The profiles to create features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="foreshoreProfiles"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateForeshoreProfilesFeatures(IEnumerable<ForeshoreProfile> foreshoreProfiles)
        {
            if (foreshoreProfiles == null || !foreshoreProfiles.Any())
            {
                return new MapFeature[0];
            }

            return foreshoreProfiles.Select(foreshoreProfile => GetAsSingleMapFeature(GetWorldPoints(foreshoreProfile))).ToArray();
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="structures"/>.
        /// </summary>
        /// <param name="structures">The profiles to create features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="structures"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateStructuresFeatures(IEnumerable<StructureBase> structures)
        {
            if (structures == null || !structures.Any())
            {
                return new MapFeature[0];
            }

            return structures.Select(structure => GetAsSingleMapFeature(structure.Location)).ToArray();
        }

        private static Point2D[] GetWorldPoints(DikeProfile dikeProfile)
        {
            return AdvancedMath2D.FromXToXY(
                    dikeProfile.DikeGeometry.Select(p => -p.Point.X).ToArray(),
                    dikeProfile.WorldReferencePoint,
                    -dikeProfile.X0,
                    dikeProfile.Orientation);
        }

        private static Point2D[] GetWorldPoints(ForeshoreProfile foreshoreProfile)
        {
            return AdvancedMath2D.FromXToXY(
                foreshoreProfile.Geometry.Select(p => -p.X).ToArray(),
                foreshoreProfile.WorldReferencePoint,
                -foreshoreProfile.X0,
                foreshoreProfile.Orientation);
        }

        private static MapFeature GetAsSingleMapFeature(IEnumerable<Point2D> points)
        {
            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    points
                })
            });
        }

        private static MapFeature GetAsSingleMapFeature(Point2D point)
        {
            return new MapFeature(new[] 
            { 
                new MapGeometry(new[] 
                { 
                    new [] { point }
                }) 
            });
        } 
    }
}