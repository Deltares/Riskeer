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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Util;
using RingtoetsCommonUtilResources = Ringtoets.Common.Util.Properties.Resources;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>
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
                throw new ArgumentNullException(nameof(points));
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
        /// <returns>A collection of features or an empty collection when <paramref name="referenceLine"/> 
        /// has no geometry.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateReferenceLineFeatures(ReferenceLine referenceLine, string id, string name)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            if (referenceLine.Points.Any())
            {
                MapFeature feature = CreateSingleLineMapFeature(referenceLine.Points);
                feature.MetaData[RingtoetsCommonUtilResources.MetaData_ID] = id;
                feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = name;
                feature.MetaData[Resources.MetaData_Length_Rounded] = new RoundedDouble(2, referenceLine.Length);

                return new[]
                {
                    feature
                };
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create hydraulic boundary location features based on the provided <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to create the location features for.</param>
        /// <returns>A collection of features.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateHydraulicBoundaryLocationFeatures(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(assessmentSection)
                                                             .Select(HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature)
                                                             .ToArray();
        }

        /// <summary>
        /// Create section features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create 
        /// the section features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="sections"/> is 
        /// <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateFailureMechanismSectionFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections?.Select(CreateFailureMechanismSectionMapFeature).ToArray() ?? new MapFeature[0];
        }

        /// <summary>
        /// Create section start point features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create 
        /// the section start point features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="sections"/> is 
        /// <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateFailureMechanismSectionStartPointFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new[]
                       {
                           CreateSingleLineMapFeature(sections.Select(sl => sl.StartPoint))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create section end point features based on the provided <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create 
        /// the section end point features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="sections"/> is 
        /// <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateFailureMechanismSectionEndPointFeatures(IEnumerable<FailureMechanismSection> sections)
        {
            return sections != null && sections.Any()
                       ? new[]
                       {
                           CreateSingleLineMapFeature(sections.Select(sl => sl.EndPoint))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="dikeProfiles"/>.
        /// </summary>
        /// <param name="dikeProfiles">The profiles to create features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="dikeProfiles"/> is 
        /// <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateDikeProfilesFeatures(IEnumerable<DikeProfile> dikeProfiles)
        {
            if (dikeProfiles != null)
            {
                int nrOfElements = dikeProfiles.Count();
                var mapFeatures = new MapFeature[nrOfElements];

                var i = 0;
                foreach (DikeProfile dikeProfile in dikeProfiles)
                {
                    MapFeature feature = CreateSingleLineMapFeature(GetWorldPoints(dikeProfile));
                    feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = dikeProfile.Name;

                    mapFeatures[i] = feature;
                    i++;
                }

                return mapFeatures;
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create features for the geometry of the <paramref name="foreshoreProfiles"/>.
        /// </summary>
        /// <param name="foreshoreProfiles">The profiles to create features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="foreshoreProfiles"/>
        /// is <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateForeshoreProfilesFeatures(IEnumerable<ForeshoreProfile> foreshoreProfiles)
        {
            if (foreshoreProfiles != null)
            {
                ForeshoreProfile[] foreShoreProfilesWithGeometry = foreshoreProfiles.Where(fp => fp.Geometry != null && fp.Geometry.Any()).ToArray();

                int nrOfElements = foreShoreProfilesWithGeometry.Length;
                var mapFeatures = new MapFeature[nrOfElements];

                for (var i = 0; i < nrOfElements; i++)
                {
                    ForeshoreProfile foreshoreProfile = foreShoreProfilesWithGeometry[i];
                    MapFeature feature = CreateSingleLineMapFeature(GetWorldPoints(foreshoreProfile));
                    feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = foreshoreProfile.Name;

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
        /// <returns>A collection of features or an empty collection when <paramref name="structures"/> is 
        /// <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateStructuresFeatures(IEnumerable<StructureBase> structures)
        {
            if (structures != null)
            {
                int nrOfElements = structures.Count();
                var mapFeatures = new MapFeature[nrOfElements];

                var i = 0;
                foreach (StructureBase structure in structures)
                {
                    MapFeature feature = RingtoetsMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(structure.Location);
                    feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = structure.Name;

                    mapFeatures[i] = feature;
                    i++;
                }

                return mapFeatures;
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculations"/>.
        /// </summary>
        /// <typeparam name="TStructuresInput">The type of the <see cref="StructuresInputBase{T}"/>.</typeparam>
        /// <typeparam name="TStructure">The type of the <see cref="StructureBase"/>.</typeparam>
        /// <param name="calculations">The collection of <see cref="StructuresCalculation{T}"/> to create the 
        /// calculation features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="calculations"/> is <c>null</c> 
        /// or empty.</returns>
        public static IEnumerable<MapFeature> CreateStructureCalculationsFeatures<TStructuresInput, TStructure>(
            IEnumerable<StructuresCalculation<TStructuresInput>> calculations)
            where TStructuresInput : StructuresInputBase<TStructure>, new()
            where TStructure : StructureBase
        {
            if (calculations != null && calculations.Any())
            {
                MapCalculationData[] calculationData = calculations.Where(CalculationHasStructureAndHydraulicBoundaryLocation<TStructuresInput, TStructure>)
                                                                   .Select(CreateMapCalculationData<TStructuresInput, TStructure>).ToArray();

                return CreateCalculationFeatures(calculationData);
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create calculation features based on the provided <see cref="MapCalculationData"/>.
        /// </summary>
        /// <param name="calculationData">The collection of <see cref="MapCalculationData"/> to create the 
        /// calculation features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="calculationData"/> is <c>null</c> 
        /// or empty.</returns>
        public static IEnumerable<MapFeature> CreateCalculationFeatures(IEnumerable<MapCalculationData> calculationData)
        {
            if (calculationData != null && calculationData.Any())
            {
                var features = new MapFeature[calculationData.Count()];

                for (var i = 0; i < calculationData.Count(); i++)
                {
                    MapCalculationData calculationItem = calculationData.ElementAt(i);
                    MapFeature feature = CreateSingleLineMapFeature(new[]
                    {
                        calculationItem.CalculationLocation,
                        calculationItem.HydraulicBoundaryLocation.Location
                    });

                    feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = calculationItem.Name;
                    feature.MetaData[Resources.MetaData_Couple_distance] =
                        calculationItem.CalculationLocation.GetEuclideanDistanceTo(
                            calculationItem.HydraulicBoundaryLocation.Location);

                    features[i] = feature;
                }

                return features;
            }

            return new MapFeature[0];
        }

        private static MapCalculationData CreateMapCalculationData<TStructuresInput, TStructure>(
            StructuresCalculation<TStructuresInput> calculation)
            where TStructuresInput : StructuresInputBase<TStructure>, new()
            where TStructure : StructureBase
        {
            return new MapCalculationData(
                calculation.Name,
                calculation.InputParameters.Structure.Location,
                calculation.InputParameters.HydraulicBoundaryLocation);
        }

        private static bool CalculationHasStructureAndHydraulicBoundaryLocation<TStructuresInput, TStructure>(
            StructuresCalculation<TStructuresInput> calculation)
            where TStructuresInput : StructuresInputBase<TStructure>, new()
            where TStructure : StructureBase
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
                    section.Points.Select(p => new Point2D(p)).ToArray()
                })
            });

            feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = section.Name;
            feature.MetaData[Resources.MetaData_Length_Rounded] = new RoundedDouble(2, section.Length);

            return feature;
        }

        private static IEnumerable<Point2D> GetWorldPoints(DikeProfile dikeProfile)
        {
            return AdvancedMath2D.FromXToXY(
                dikeProfile.DikeGeometry.Select(p => -p.Point.X),
                dikeProfile.WorldReferencePoint,
                -dikeProfile.X0,
                dikeProfile.Orientation);
        }

        private static IEnumerable<Point2D> GetWorldPoints(ForeshoreProfile foreshoreProfile)
        {
            return AdvancedMath2D.FromXToXY(
                foreshoreProfile.Geometry.Select(p => -p.X),
                foreshoreProfile.WorldReferencePoint,
                -foreshoreProfile.X0,
                foreshoreProfile.Orientation);
        }
    }
}