﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Attributes;
using Core.Common.Util.Enums;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of <see cref="HydraulicBoundaryLocationCalculation"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class HydraulicBoundaryLocationCalculationBaseProperties : ObjectProperties<HydraulicBoundaryLocationCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationBaseProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationCalculationBaseProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
        {
            if (hydraulicBoundaryLocationCalculation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocationCalculation));
            }

            Data = hydraulicBoundaryLocationCalculation;
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public virtual RoundedDouble Result
        {
            get
            {
                return data.Output?.Result ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                return data.Output?.TargetProbability ?? double.NaN;
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                return data.Output?.TargetReliability ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(8)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return data.Output?.CalculatedProbability ?? double.NaN;
            }
        }

        [PropertyOrder(9)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.Output?.CalculatedReliability ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(10)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_Convergence_DisplayName))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public virtual CalculationConvergence Convergence
        {
            get
            {
                return data.Output?.CalculationConvergence ?? CalculationConvergence.NotCalculated;
            }
        }

        [PropertyOrder(11)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ShouldIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldIllustrationPointsBeCalculated
        {
            get
            {
                return data.InputParameters.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                data.InputParameters.ShouldIllustrationPointsBeCalculated = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(12)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_GoverningWindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_GoverningWindDirection_Description))]
        public string GoverningWindDirection
        {
            get
            {
                return GetGeneralResult().GoverningWindDirection.Name;
            }
        }

        [PropertyOrder(13)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return GetStochasts();
            }
        }

        [PropertyOrder(14)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return GetStochasts();
            }
        }

        [PropertyOrder(15)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public IEnumerable<TopLevelSubMechanismIllustrationPointProperties> IllustrationPoints
        {
            get
            {
                IEnumerable<TopLevelSubMechanismIllustrationPoint> topLevelIllustrationPoints = GetTopLevelIllustrationPoints();
                IEnumerable<string> closingSituations = topLevelIllustrationPoints.Select(s => s.ClosingSituation);
                return GetTopLevelSubMechanismIllustrationPointProperties(topLevelIllustrationPoints, closingSituations);
            }
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

        private static TopLevelSubMechanismIllustrationPointProperties[] GetTopLevelSubMechanismIllustrationPointProperties(IEnumerable<TopLevelSubMechanismIllustrationPoint> topLevelIllustrationPoints, IEnumerable<string> closingSituations)
        {
            return topLevelIllustrationPoints.Select(p => new TopLevelSubMechanismIllustrationPointProperties(p, closingSituations))
                                             .ToArray();
        }

        private Stochast[] GetStochasts()
        {
            return GetGeneralResult().Stochasts.ToArray();
        }

        private TopLevelSubMechanismIllustrationPoint[] GetTopLevelIllustrationPoints()
        {
            return GetGeneralResult().TopLevelIllustrationPoints.ToArray();
        }

        private GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult()
        {
            return data.Output?.GeneralResult;
        }
    }
}