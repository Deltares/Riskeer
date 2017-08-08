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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="OvertoppingRateOutput"/> for properties panel.
    /// </summary>
    public class OvertoppingRateOutputProperties : ObjectProperties<OvertoppingRateOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="OvertoppingRateOutputProperties"/>.
        /// </summary>
        /// <param name="overtoppingRateOutput">The overtopping rate output to create the object properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="overtoppingRateOutput"/> is <c>null</c>.</exception>
        public OvertoppingRateOutputProperties(OvertoppingRateOutput overtoppingRateOutput)
        {
            if (overtoppingRateOutput == null)
            {
                throw new ArgumentNullException(nameof(overtoppingRateOutput));
            }

            Data = overtoppingRateOutput;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_OvertoppingRate_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_OvertoppingRate_Description))]
        public RoundedDouble OvertoppingRate
        {
            get
            {
                return new RoundedDouble(2, data.OvertoppingRate * 1000);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double OvertoppingRateTargetProbability
        {
            get
            {
                return data.TargetProbability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble OvertoppingRateTargetReliability
        {
            get
            {
                return data.TargetReliability;
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double OvertoppingRateCalculatedProbability
        {
            get
            {
                return data.CalculatedProbability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble OvertoppingRateCalculatedReliability
        {
            get
            {
                return data.CalculatedReliability;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.OvertoppingRateOutput_Convergence_Description))]
        public string OvertoppingRateConvergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.CalculationConvergence).DisplayName;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_GoverningWindDirection_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_GoverningWindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.GeneralResult?.GoverningWindDirection.Name;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return data.GeneralResult?.Stochasts.ToArray();
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return data.GeneralResult?.Stochasts.ToArray();
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public TopLevelFaultTreeIllustrationPointProperties[] IllustrationPoints
        {
            get
            {
                if (!data.HasGeneralResult)
                {
                    return new TopLevelFaultTreeIllustrationPointProperties[0];
                }

                IEnumerable<string> listOfClosingSituations = data.GeneralResult
                                                                  .TopLevelIllustrationPoints
                                                                  .Select(point => point.ClosingSituation);

                return data.GeneralResult
                           .TopLevelIllustrationPoints
                           .Select(point =>
                                       new TopLevelFaultTreeIllustrationPointProperties(
                                           point, listOfClosingSituations)).ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName.Equals(nameof(WindDirection)) ||
                propertyName.Equals(nameof(AlphaValues)) ||
                propertyName.Equals(nameof(Durations)) ||
                propertyName.Equals(nameof(IllustrationPoints)))
            {
                return data.HasGeneralResult;
            }

            return false;
        }
    }
}