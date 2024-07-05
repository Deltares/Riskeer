// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanismSectionConfiguration"/> for properties panel.
    /// </summary>
    public class PipingFailureMechanismSectionConfigurationProperties : FailureMechanismSectionConfigurationProperties
    {
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionConfigurationProperties"/>.
        /// </summary>
        /// <param name="sectionConfiguration">The section configuration to show the properties for.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfiguration"/> or <paramref name="propertyChangeHandler"/>
        /// is <c>null</c>.</exception>
        public PipingFailureMechanismSectionConfigurationProperties(PipingFailureMechanismSectionConfiguration sectionConfiguration, double sectionStart, double sectionEnd, double b,
                                                                    IObservablePropertyChangeHandler propertyChangeHandler)
            : base(sectionConfiguration, sectionStart, sectionEnd, b)
        {
            if (propertyChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(propertyChangeHandler));
            }

            this.propertyChangeHandler = propertyChangeHandler;
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSectionConfiguration_Parameter_A_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSectionConfiguration_Parameter_A_Description))]
        public override RoundedDouble ParameterA
        {
            get
            {
                return base.ParameterA;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => SectionConfiguration.A = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(9)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSensitiveSectionLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSensitiveSectionLength_Description))]
        public RoundedDouble FailureMechanismSensitiveSectionLength
        {
            get
            {
                return new RoundedDouble(2, SectionConfiguration.GetFailureMechanismSensitiveSectionLength());
            }
        }
    }
}