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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
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
    /// Properties for the Fault tree of Illustration Points
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FaultTreeIllustrationPointBaseProperty : ObjectProperties<TopLevelFaultTreeIllustrationPoint>
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointProperty"/>.
        /// </summary>
        /// <param name="faultTreeData">The data to use for the properties. </param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public FaultTreeIllustrationPointBaseProperty(
            TopLevelFaultTreeIllustrationPoint faultTreeData)
        {
            if (faultTreeData == null)
            {
                throw new ArgumentNullException(nameof(faultTreeData));
            }
            data = faultTreeData;
        }

        [ReadOnly(true)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_Description))]
        public double CalculatedProbability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(data.FaultTreeNodeRoot.Data.Beta);
            }
        }

        [ReadOnly(true)]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                return data.FaultTreeNodeRoot.Data.Beta;
            }
        }

        [ReadOnly(true)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.WindDirection.Name;
            }
        }

        [ReadOnly(true)]
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

        [ReadOnly(true)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [KeyValueElement(nameof(WindDirection), "")]
        public IllustrationPointChildProperty[] IllustrationPoints
        {
            get
            {
                var points = new List<IllustrationPointChildProperty>();
                foreach (IllustrationPointNode illustrationPointNode in data.FaultTreeNodeRoot.Children)
                {
                    var faultTree = illustrationPointNode.Data as FaultTreeIllustrationPoint;
                    if (faultTree != null)
                    {
                        points.Add(new FaultTreeIllustrationPointChildProperty(illustrationPointNode, WindDirection));
                        continue;
                    }

                    var subMechanism = illustrationPointNode.Data as SubMechanismIllustrationPoint;
                    if (subMechanism != null)
                    {
                        points.Add(new SubMechanismIllustrationPointChildProperty(illustrationPointNode, WindDirection));
                        continue;
                    }
                    
                    // If type is not supported, throw exception (currently not possible, safeguard for future)
                    throw new NotSupportedException("IllustrationPointNode is not of a supported type (FaultTree/SubMechanism)");


                }
                return points.ToArray();
            }
        }

        public override string ToString()
        {
            return $"{WindDirection}";
        }
    }
}