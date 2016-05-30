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

using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ProbabilisticOutput"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsOutputProperties : ObjectProperties<ProbabilisticOutput>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), "Categories_GrassCoverErosionInwards")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsOutput_RequiredProbability_Displayname")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsOutput_RequiredProbability_Description")]
        public string RequiredProbability
        {
            get
            {
                return ToProbabilityFormat(data.RequiredProbability);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), "Categories_GrassCoverErosionInwards")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsOutput_RequiredReliability_Displayname")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsOutput_RequiredReliability_Description")]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), "Categories_GrassCoverErosionInwards")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsOutput_Probability_Displayname")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsOutput_Probability_Description")]
        public string Probability
        {
            get
            {
                return ToProbabilityFormat(data.Probability);
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), "Categories_GrassCoverErosionInwards")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsOutput_Reliability_Displayname")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsOutput_Reliability_Description")]
        public RoundedDouble Reliability
        {
            get
            {
                return data.Reliability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), "Categories_GrassCoverErosionInwards")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsOutput_FactorOfSafety_Displayname")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsOutput_FactorOfSafety_Description")]
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return data.FactorOfSafety;
            }
        }

        private static string ToProbabilityFormat(RoundedDouble probability)
        {
            return string.Format(Core.Common.Base.Properties.Resources.ProbabilityPerYearFormat, probability);
        }
    }
}