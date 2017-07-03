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

using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Forms.TypeConverters;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="TopLevelSubMechanismIllustrationPoint"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TopLevelSubMechanismIllustrationPointProperties : ObjectProperties<TopLevelSubMechanismIllustrationPoint>
    {
        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(data.SubMechanismIllustrationPoint.Beta);

            }
        }

        [PropertyOrder(2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Beta;
            }
        }

        [PropertyOrder(3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.WindDirection.Name;
            }
        }

        [PropertyOrder(4)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_ClosingSituation_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_ClosingSituation_Description))]
        public string ClosingSituation
        {
            get
            {
                return data.ClosingSituation;
            }
        }

        [PropertyOrder(5)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(SubMechanismIllustrationPointStochast.Name), nameof(SubMechanismIllustrationPointStochast.Alpha))]
        public SubMechanismIllustrationPointStochast[] AlphaValues
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(6)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Durations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(SubMechanismIllustrationPointStochast.Name), nameof(SubMechanismIllustrationPointStochast.Duration))]
        public SubMechanismIllustrationPointStochast[] Durations
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(7)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_Values_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_Values_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(IllustrationPointResult.Description), nameof(IllustrationPointResult.Value))]
        public IllustrationPointResult[] IllustrationPointResults
        {
            get
            {
                return data.SubMechanismIllustrationPoint.IllustrationPointResults.ToArray();
            }
        }

        public override string ToString()
        {
            return data.SubMechanismIllustrationPoint.Name;
        }
    }
}