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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="FailureMechanismSectionConfiguration"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FailureMechanismSectionConfigurationProperties : FailureMechanismSectionProperties
    {
        protected readonly FailureMechanismSectionConfiguration SectionConfiguration;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionConfigurationProperties"/>.
        /// </summary>
        /// <param name="sectionConfiguration">The section configuration to show the properties for.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfiguration"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismSectionConfigurationProperties(FailureMechanismSectionConfiguration sectionConfiguration, double sectionStart, double sectionEnd, double b)
            : base(sectionConfiguration?.Section ?? throw new ArgumentNullException(nameof(sectionConfiguration)), sectionStart, sectionEnd)
        {
            SectionConfiguration = sectionConfiguration;
            LengthEffectNRounded = new RoundedDouble(2, SectionConfiguration.GetN(b));
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionConfiguration_Parameter_A_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSectionConfiguration_Parameter_A_Description))]
        public virtual RoundedDouble ParameterA
        {
            get
            {
                return SectionConfiguration.A;
            }
            set
            {
                SectionConfiguration.A = value;
                SectionConfiguration.NotifyObservers();
            }
        }

        [PropertyOrder(8)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LengthEffectNRounded_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LengthEffectNRounded_Description))]
        public RoundedDouble LengthEffectNRounded { get; }
    }
}