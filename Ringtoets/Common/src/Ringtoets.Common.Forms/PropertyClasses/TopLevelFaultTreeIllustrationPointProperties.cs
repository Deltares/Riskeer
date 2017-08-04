﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties for the fault tree illustration points.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TopLevelFaultTreeIllustrationPointProperties : ObjectProperties<TopLevelFaultTreeIllustrationPoint>
    {
        private readonly IEnumerable<string> closingSituations;

        /// <summary>
        /// Creates a new instance of <see cref="TopLevelFaultTreeIllustrationPointProperties"/>.
        /// </summary>
        /// <param name="faultTreeData">The data to use for the properties. </param>
        /// <param name="allClosingSituations">All closing situations in the tree</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public TopLevelFaultTreeIllustrationPointProperties(
            TopLevelFaultTreeIllustrationPoint faultTreeData, IEnumerable<string> allClosingSituations)
        {
            if (faultTreeData == null)
            {
                throw new ArgumentNullException(nameof(faultTreeData));
            }
            if (allClosingSituations == null)
            {
                throw new ArgumentNullException(nameof(allClosingSituations));
            }
            data = faultTreeData;
            closingSituations = allClosingSituations;
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(data.FaultTreeNodeRoot.Data.Beta);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble Reliability
        {
            get
            {
                return data.FaultTreeNodeRoot.Data.Beta;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.WindDirection.Name;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_Description))]
        public string ClosingSituation
        {
            get
            {
                return data.ClosingSituation;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return ((FaultTreeIllustrationPoint) data.FaultTreeNodeRoot.Data).Stochasts.ToArray();
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return ((FaultTreeIllustrationPoint) data.FaultTreeNodeRoot.Data).Stochasts.ToArray();
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public IllustrationPointProperties[] IllustrationPoints
        {
            get
            {
                var points = new List<IllustrationPointProperties>();
                foreach (IllustrationPointNode illustrationPointNode in data.FaultTreeNodeRoot.Children)
                {
                    if (illustrationPointNode.Data is FaultTreeIllustrationPoint)
                    {
                        points.Add(new FaultTreeIllustrationPointProperties(illustrationPointNode, WindDirection));
                        continue;
                    }

                    if (illustrationPointNode.Data is SubMechanismIllustrationPoint)
                    {
                        points.Add(new SubMechanismIllustrationPointProperties(illustrationPointNode, WindDirection));
                        continue;
                    }

                    // If type is not supported, throw exception (currently not possible, safeguard for future)
                    throw new NotSupportedException($"IllustrationPointNode of type {nameof(FaultTreeIllustrationPoint)} is not supported. Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}");
                }
                return points.ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(ClosingSituation))
            {
                return !(closingSituations.Distinct().Count() < 2);
            }

            return false;
        }

        public override string ToString()
        {
            return $"{WindDirection}";
        }
    }
}