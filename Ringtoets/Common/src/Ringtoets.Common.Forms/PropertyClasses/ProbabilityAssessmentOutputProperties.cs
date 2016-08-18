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

using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;
using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ProbabilityAssessmentOutput"/> for properties panel.
    /// </summary>
    public class ProbabilityAssessmentOutputProperties : ObjectProperties<ProbabilityAssessmentOutput>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "ProbabilityAssessmentOutput_RequiredProbability_Displayname")]
        [ResourcesDescription(typeof(Resources), "ProbabilityAssessmentOutput_RequiredProbability_Description")]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.RequiredProbability);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "ProbabilityAssessmentOutput_RequiredReliability_Displayname")]
        [ResourcesDescription(typeof(Resources), "ProbabilityAssessmentOutput_RequiredReliability_Description")]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "ProbabilityAssessmentOutput_Probability_Displayname")]
        [ResourcesDescription(typeof(Resources), "ProbabilityAssessmentOutput_Probability_Description")]
        public string Probability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.Probability);
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "ProbabilityAssessmentOutput_Reliability_Displayname")]
        [ResourcesDescription(typeof(Resources), "ProbabilityAssessmentOutput_Reliability_Description")]
        public RoundedDouble Reliability
        {
            get
            {
                return data.Reliability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "ProbabilityAssessmentOutput_FactorOfSafety_Displayname")]
        [ResourcesDescription(typeof(Resources), "ProbabilityAssessmentOutput_FactorOfSafety_Description")]
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return data.FactorOfSafety;
            }
        }
    }
}