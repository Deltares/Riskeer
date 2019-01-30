// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// Object properties class for <see cref="CalculationGroup"/>.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationGroupContextProperties_DisplayName))]
    public class CalculationGroupContextProperties : ObjectProperties<ICalculationContext<CalculationGroup, IFailureMechanism>>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CalculationGroupContextProperties"/> class.
        /// </summary>
        /// <param name="calculationContext">The calculation context to use for the properties.</param>
        public CalculationGroupContextProperties(ICalculationContext<CalculationGroup, IFailureMechanism> calculationContext)
        {
            if (calculationContext == null)
            {
                throw new ArgumentNullException(nameof(calculationContext));
            }

            Data = calculationContext;
        }

        [DynamicReadOnly]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationGroup_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationGroup_Name_Description))]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
            set
            {
                data.WrappedData.Name = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidator(string propertyName)
        {
            return data.Parent == null;
        }
    }
}