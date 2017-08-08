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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StructuresOutput"/> for properties panel.
    /// </summary>
    public class StructuresOutputProperties : ObjectProperties<StructuresOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructuresOutputProperties"/>.
        /// </summary>
        /// <param name="structuresOutput">The structures output to create the object properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structuresOutput"/> is <c>null</c>.</exception>
        public StructuresOutputProperties(StructuresOutput structuresOutput)
        {
            if (structuresOutput == null)
            {
                throw new ArgumentNullException(nameof(structuresOutput));
            }

            Data = structuresOutput;
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_GoverningWindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_GoverningWindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.GeneralResult?.GoverningWindDirection.Name;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_Description))]
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
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_Description))]
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
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public TopLevelFaultTreeIllustrationPointProperties[] IllustrationPoints
        {
            get
            {
                List<string> listOfClosingSituations = data.GeneralResult.TopLevelIllustrationPoints.Select(topLevelFaultTreeIllustrationPoint =>
                                                                                                                topLevelFaultTreeIllustrationPoint.ClosingSituation).ToList();

                return data.GeneralResult.TopLevelIllustrationPoints.Select(point => new TopLevelFaultTreeIllustrationPointProperties(point, listOfClosingSituations)).ToArray();
            }
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_RequiredProbability_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_RequiredProbability_Description))]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityAssessmentOutput.RequiredProbability);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_RequiredReliability_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_RequiredReliability_Description))]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.ProbabilityAssessmentOutput.RequiredReliability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_Probability_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_Probability_Description))]
        public string Probability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityAssessmentOutput.Probability);
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_Reliability_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_Reliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                return data.ProbabilityAssessmentOutput.Reliability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_FactorOfSafety_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityAssessmentOutput_FactorOfSafety_Description))]
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return data.ProbabilityAssessmentOutput.FactorOfSafety;
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.HasGeneralResult &&
                   (
                       propertyName.Equals(nameof(WindDirection)) ||
                       propertyName.Equals(nameof(AlphaValues)) ||
                       propertyName.Equals(nameof(Durations)) ||
                       propertyName.Equals(nameof(IllustrationPoints))
                   );
        }
    }
}