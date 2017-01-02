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
using Core.Common.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating arrays of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>
    /// (created via <see cref="RingtoetsMapDataFactory"/>).
    /// </summary>
    public static class RingtoetsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create a single <see cref="MapFeature"/> representing a single line.
        /// </summary>
        /// <param name="points">The world map points describing the line.</param>
        /// <returns>The created map feature.</returns>
        /// <exception cref="ArgumentNullException ">Thrown when <paramref name="points"/>
        /// is <c>null</c>.</exception>
        public static MapFeature CreateSingleLineMapFeature(IEnumerable<Point2D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    points
                })
            });
        }

        /// <summary>
        /// Create reference line features based on the provided <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> to create the reference 
        /// line features for.</param>
        /// <param name="id">The id of the <see cref="IAssessmentSection"/>.</param>
        /// <param name="name">The name of the <see cref="IAssessmentSection"/>.</param>
        /// <returns>An array of features or an empty array when <paramref name="referenceLine"/> 
        /// is <c>null</c>.</returns>
        public static MapFeature[] CreateReferenceLineFeatures(ReferenceLine referenceLine, string id, string name)
        {
            if (referenceLine != null)
            {
                MapFeature feature = CreateSingleLineMapFeature(referenceLine.Points);
                feature.MetaData[Resources.MetaData_ID] = id;
                feature.MetaData[Resources.MetaData_Name] = name;
                feature.MetaData[Resources.MetaData_Length] = new RoundedDouble(2, Math2D.Length(referenceLine.Points));

                return new[]
                {
                    feature
                };
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create hydraulic boundary database location features based on the provided <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/>
        /// to create the location features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="hydraulicBoundaryDatabase"/> 
        /// is <c>null</c>.</returns>
        public static MapFeature[] CreateHydraulicBoundaryDatabaseFeatures(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            return CreateHydraulicBoundaryLocationFeatures(hydraulicBoundaryDatabase != null
                                                               ? hydraulicBoundaryDatabase.Locations.ToArray()
                                                               : new HydraulicBoundaryLocation[0],
                                                           Resources.MetaData_DesignWaterLevel,
                                                           Resources.MetaData_WaveHeight);
        }

        /// <summary>
        /// Create hydraulic boundary location features based on the provided <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The locations to create the features for.</param>
        /// <param name="designWaterLevelAttributeName">The name of the design water level attribute.</param>
        /// <param name="waveHeightAttributeName">The name of the wave height attribute.</param>
        /// <returns>An array of features or an empty array when <paramref name="hydraulicBoundaryLocations"/> 
        /// is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static MapFeature[] CreateHydraulicBoundaryLocationFeatures(HydraulicBoundaryLocation[] hydraulicBoundaryLocations,
                                                                           string designWaterLevelAttributeName,
                                                                           string waveHeightAttributeName)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocations");
            }
            if (designWaterLevelAttributeName == null)
            {
                throw new ArgumentNullException("designWaterLevelAttributeName");
            }
            if (waveHeightAttributeName == null)
            {
                throw new ArgumentNullException("waveHeightAttributeName");
            }

            var features = new MapFeature[hydraulicBoundaryLocations.Length];

            for (int i = 0; i < hydraulicBoundaryLocations.Length; i++)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations[i];

                var feature = CreateSinglePointMapFeature(location.Location);
                feature.MetaData[Resources.MetaData_ID] = location.Id;
                feature.MetaData[Resources.MetaData_Name] = location.Name;
                feature.MetaData[designWaterLevelAttributeName] = location.DesignWaterLevel;
                feature.MetaData[waveHeightAttributeName] = location.WaveHeight;

                features[i] = feature;
            }

            return features;
        }

        /// <summary>
        /// Create section features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create 
        /// the section features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="sections"/> is 
        /// <c>null</c> or empty.</returns>
        public static MapFeature[] CreateFailureMechanismSectionFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null
                       ? sections.Select(CreateFailureMechanismSectionMapFeature).ToArray()
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create section start point features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create 
        /// the section start point features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="sections"/> is 
        /// <c>null</c> or empty.</returns>
        public static MapFeature[] CreateFailureMechanismSectionStartPointFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new[]
                       {
                           CreateSingleLineMapFeature(sections.Select(sl => sl.GetStart()))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create section end point features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create 
        /// the section end point features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="sections"/> is 
        /// <c>null</c> or empty.</returns>
        public static MapFeature[] CreateFailureMechanismSectionEndPointFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new[]
                       {
                           CreateSingleLineMapFeature(sections.Select(sl => sl.GetLast()))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="dikeProfiles"/>.
        /// </summary>
        /// <param name="dikeProfiles">The profiles to create features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="dikeProfiles"/> is 
        /// <c>null</c> or empty.</returns>
        public static MapFeature[] CreateDikeProfilesFeatures(IEnumerable<DikeProfile> dikeProfiles)
        {
            if (dikeProfiles != null)
            {
                DikeProfile[] sourceDikeProfiles = dikeProfiles.ToArray();
                var mapFeatures = new MapFeature[sourceDikeProfiles.Length];
                for (int i = 0; i < sourceDikeProfiles.Length; i++)
                {
                    DikeProfile profile = sourceDikeProfiles[i];

                    MapFeature feature = CreateSingleLineMapFeature(GetWorldPoints(profile));
                    feature.MetaData[Resources.MetaData_Name] = profile.Name;

                    mapFeatures[i] = feature;
                }
                return mapFeatures;
            }
            return new MapFeature[0];
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="foreshoreProfiles"/>.
        /// </summary>
        /// <param name="foreshoreProfiles">The profiles to create features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="foreshoreProfiles"/>
        /// is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateForeshoreProfilesFeatures(IEnumerable<ForeshoreProfile> foreshoreProfiles)
        {
            if (foreshoreProfiles != null)
            {
                ForeshoreProfile[] sourceForeshoreProfiles = foreshoreProfiles.ToArray();
                var mapFeatures = new MapFeature[sourceForeshoreProfiles.Length];
                for (int i = 0; i < sourceForeshoreProfiles.Length; i++)
                {
                    ForeshoreProfile profile = sourceForeshoreProfiles[i];

                    MapFeature feature = CreateSingleLineMapFeature(GetWorldPoints(profile));
                    feature.MetaData[Resources.MetaData_Name] = profile.Name;

                    mapFeatures[i] = feature;
                }
                return mapFeatures;
            }
            return new MapFeature[0];
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="structures"/>.
        /// </summary>
        /// <param name="structures">The profiles to create features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="structures"/> is 
        /// <c>null</c> or empty.</returns>
        public static MapFeature[] CreateStructuresFeatures(IEnumerable<StructureBase> structures)
        {
            if (structures != null)
            {
                StructureBase[] sourceStructures = structures.ToArray();
                var mapFeatures = new MapFeature[sourceStructures.Length];
                for (int i = 0; i < sourceStructures.Length; i++)
                {
                    StructureBase structure = sourceStructures[i];

                    MapFeature feature = CreateSinglePointMapFeature(structure.Location);
                    feature.MetaData[Resources.MetaData_Name] = structure.Name;

                    mapFeatures[i] = feature;
                }
                return mapFeatures;
            }
            return new MapFeature[0];
        }

        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="StructuresCalculation{T}"/> to create the 
        /// calculation features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="calculations"/> is <c>null</c> 
        /// or empty.</returns>
        public static MapFeature[] CreateStructureCalculationsFeatures<T, U>(IEnumerable<StructuresCalculation<T>> calculations)
            where T : StructuresInputBase<U>, new()
            where U : StructureBase
        {
            if ((calculations != null && calculations.Any()))
            {
                MapCalculationData[] calculationData = Enumerable.ToArray(calculations.Where(CalculationHasStructureAndHydraulicBoundaryLocation<T, U>)
                                                                                      .Select(CreatemapCalculationData<T, U>));

                return CreateCalculationFeatures(calculationData);
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create calculation features based on the provided <see cref="MapCalculationData"/>.
        /// </summary>
        /// <param name="calculationData">The collection of <see cref="MapCalculationData"/> to create the 
        /// calculation features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="calculationData"/> is <c>null</c> 
        /// or empty.</returns>
        public static MapFeature[] CreateCalculationFeatures(MapCalculationData[] calculationData)
        {
            if (calculationData != null && calculationData.Any())
            {
                var features = new MapFeature[calculationData.Length];

                for (int i = 0; i < calculationData.Length; i++)
                {
                    MapCalculationData calculationItem = calculationData[i];
                    MapFeature feature = CreateSingleLineMapFeature(new[]
                    {
                        calculationItem.CalculationLocation,
                        calculationItem.HydraulicBoundaryLocation.Location
                    });

                    feature.MetaData[Resources.MetaData_Name] = calculationItem.Name;
                    feature.MetaData[Resources.MetaData_Couple_distance] =
                        calculationItem.CalculationLocation.GetEuclideanDistanceTo(
                            calculationItem.HydraulicBoundaryLocation.Location);

                    features[i] = feature;
                }
                return features;
            }

            return new MapFeature[0];
        }

        private static MapCalculationData CreatemapCalculationData<T, U>(StructuresCalculation<T> calculation)
            where T : StructuresInputBase<U>, new()
            where U : StructureBase
        {
            return new MapCalculationData(
                calculation.Name,
                calculation.InputParameters.Structure.Location,
                calculation.InputParameters.HydraulicBoundaryLocation);
        }

        private static bool CalculationHasStructureAndHydraulicBoundaryLocation<T, U>(StructuresCalculation<T> calculation)
            where T : StructuresInputBase<U>, new()
            where U : StructureBase
        {
            return calculation.InputParameters.Structure != null &&
                   calculation.InputParameters.HydraulicBoundaryLocation != null;
        }

        private static MapFeature CreateFailureMechanismSectionMapFeature(FailureMechanismSection section)
        {
            var feature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    section.Points.Select(p => new Point2D(p.X, p.Y))
                })
            });

            feature.MetaData[Resources.MetaData_Name] = section.Name;
            feature.MetaData[Resources.MetaData_Length] = new RoundedDouble(2, Math2D.Length(section.Points));

            return feature;
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

        private static MapFeature CreateSinglePointMapFeature(Point2D point)
        {
            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        point
                    }
                })
            });
        }
    }
}