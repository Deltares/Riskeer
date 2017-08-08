// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.Properties;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocation"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class HydraulicBoundaryLocationProperties : ObjectProperties<HydraulicBoundaryLocationContext>
    {
        private readonly Dictionary<string, int> propertyIndexLookup;

        protected HydraulicBoundaryLocationProperties(ConstructionProperties propertyIndexes)
        {
            propertyIndexLookup = new Dictionary<string, int>
            {
                {
                    nameof(Id), propertyIndexes.IdIndex
                },
                {
                    nameof(Name), propertyIndexes.NameIndex
                },
                {
                    nameof(Location), propertyIndexes.LocationIndex
                },
                {
                    nameof(GoverningWindDirection), propertyIndexes.GoverningWindDirectionIndex
                },
                {
                    nameof(AlphaValues), propertyIndexes.StochastsIndex
                },
                {
                    nameof(Durations), propertyIndexes.DurationsIndex
                },
                {
                    nameof(IllustrationPoints), propertyIndexes.IllustrationPointsIndex
                }
            };
        }

        public class ConstructionProperties
        {
            public int IdIndex { get; set; } = 1;
            public int NameIndex { get; set; } = 2;
            public int LocationIndex { get; set; } = 3;
            public int GoverningWindDirectionIndex { get; set; } = 4;
            public int StochastsIndex { get; set; } = 5;
            public int DurationsIndex { get; set; } = 6;
            public int IllustrationPointsIndex { get; set; } = 7;
        }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrderEvaluationMethod(string propertyName)
        {
            int propertyIndex;

            propertyIndexLookup.TryGetValue(propertyName, out propertyIndex);

            return propertyIndex;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            bool hasGeneralIllustrationPointsResult = GetGeneralResult() != null;
            if (propertyName == nameof(GoverningWindDirection)
                || propertyName == nameof(AlphaValues)
                || propertyName == nameof(Durations)
                || propertyName == nameof(IllustrationPoints))
            {
                return hasGeneralIllustrationPointsResult;
            }

            return true;
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_Description))]
        public long Id
        {
            get
            {
                return data.HydraulicBoundaryLocation.Id;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_Description))]
        public string Name
        {
            get
            {
                return data.HydraulicBoundaryLocation.Name;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_Description))]
        public Point2D Location
        {
            get
            {
                return data.HydraulicBoundaryLocation.Location;
            }
        }

        [DynamicPropertyOrder]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_GoverningWindDirection_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_GoverningWindDirection_Description))]
        public string GoverningWindDirection
        {
            get
            {
                return GetGeneralResult().GoverningWindDirection.Name;
            }
        }

        [DynamicPropertyOrder]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return GetGeneralResult().Stochasts.ToArray();
            }
        }

        [DynamicPropertyOrder]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return GetGeneralResult().Stochasts.ToArray();
            }
        }

        [DynamicPropertyOrder]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public IEnumerable<TopLevelSubMechanismIllustrationPointProperties> IllustrationPoints
        {
            get
            {
                IEnumerable<TopLevelSubMechanismIllustrationPoint> topLevelIllustrationPoints =
                    GetGeneralResult().TopLevelIllustrationPoints.ToArray();

                IEnumerable<string> closingSituations = topLevelIllustrationPoints.Select(s => s.ClosingSituation);

                return topLevelIllustrationPoints.Select(p => new TopLevelSubMechanismIllustrationPointProperties(p, closingSituations))
                                                 .ToArray();
            }
        }

        /// <summary>
        /// Gets the general result with the illustration points result.
        /// </summary>
        /// <returns>The general illustration points if it has obtained as part of the calculation, <c>null</c>
        /// otherwise.</returns>
        protected abstract GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult();

        public override string ToString()
        {
            return $"{Name} {Location}";
        }
    }
}