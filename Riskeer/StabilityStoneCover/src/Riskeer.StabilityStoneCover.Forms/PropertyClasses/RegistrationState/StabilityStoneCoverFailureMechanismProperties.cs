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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.StabilityStoneCover.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Forms.PropertyClasses.RegistrationState
{
    /// <summary>
    /// Registration state related ViewModel of <see cref="StabilityStoneCoverFailureMechanism"/> for properties panel.
    /// </summary>
    public class StabilityStoneCoverFailureMechanismProperties : StabilityStoneCoverFailureMechanismPropertiesBase
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int inAssemblyPropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public StabilityStoneCoverFailureMechanismProperties(StabilityStoneCoverFailureMechanism data)
            : base(data, new ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex
            }) {}

        #region General

        [PropertyOrder(inAssemblyPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_InAssembly_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_InAssembly_Description))]
        public bool InAssembly
        {
            get
            {
                return data.InAssembly;
            }
        }

        #endregion
    }
}