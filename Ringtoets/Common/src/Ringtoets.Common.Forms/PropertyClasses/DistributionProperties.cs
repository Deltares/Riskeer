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

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Property for probabilistic distribution.
    /// </summary>
    public abstract class DistributionProperties : ObjectProperties<IDistribution>
    {
        private static string meanDisplayName;
        private static string standardDeviationDisplayName;
        protected readonly bool IsVariationCoefficientReadOnly;
        private readonly bool isMeanReadOnly;
        private readonly bool isStandardDeviationReadOnly;
        protected IObservable Observerable;

        protected DistributionProperties(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            isStandardDeviationReadOnly = propertiesReadOnly == DistributionPropertiesReadOnly.All || propertiesReadOnly == DistributionPropertiesReadOnly.StandardDeviation;
            isMeanReadOnly = propertiesReadOnly == DistributionPropertiesReadOnly.All || propertiesReadOnly == DistributionPropertiesReadOnly.Mean;
            IsVariationCoefficientReadOnly = propertiesReadOnly == DistributionPropertiesReadOnly.All || propertiesReadOnly == DistributionPropertiesReadOnly.VariationCoefficient;

            meanDisplayName = TypeUtils.GetMemberName<DistributionProperties>(rd => rd.Mean);
            standardDeviationDisplayName = TypeUtils.GetMemberName<DistributionProperties>(rd => rd.StandardDeviation);
        }

        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), "Distribution_DestributionType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Distribution_DestributionType_Description")]
        public abstract string DistributionType { get; }

        [PropertyOrder(2)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_Mean_DisplayName")]
        [ResourcesDescription(typeof(Resources), "NormalDistribution_Mean_Description")]
        public virtual RoundedDouble Mean
        {
            get
            {
                return data.Mean;
            }
            set
            {
                if (isMeanReadOnly)
                {
                    throw new ArgumentException("Mean is set to be read-only.");
                }
                if (Observerable == null)
                {
                    throw new ArgumentException("No observerable object set.");
                }
                data.Mean = value;
                Observerable.NotifyObservers();
            }
        }

        [PropertyOrder(3)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_StandardDeviation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "NormalDistribution_StandardDeviation_Description")]
        public virtual RoundedDouble StandardDeviation
        {
            get
            {
                return data.StandardDeviation;
            }
            set
            {
                if (isStandardDeviationReadOnly)
                {
                    throw new ArgumentException("StandardDeviation is set to be read-only.");
                }
                if (Observerable == null)
                {
                    throw new ArgumentException("No observerable object set.");
                }
                data.StandardDeviation = value;
                Observerable.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == meanDisplayName)
            {
                return isMeanReadOnly;
            }
            if (propertyName == standardDeviationDisplayName)
            {
                return isStandardDeviationReadOnly;
            }
            if (propertyName == "VariationCoefficient")
            {
                return IsVariationCoefficientReadOnly;
            }
            return false;
        }

        public override string ToString()
        {
            return data == null ? Resources.NormalDistribution_StandardDeviation_DisplayName :
                       string.Format("{0} ({1} = {2})", Mean, Resources.NormalDistribution_StandardDeviation_DisplayName, StandardDeviation);
        }
    }
}