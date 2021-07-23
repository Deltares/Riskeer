﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.DuneErosion.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// Hydraulic loads related ViewModel of <see cref="DuneErosionFailureMechanism"/> for properties panel.
    /// </summary>
    public class DuneErosionHydraulicLoadsProperties : DuneErosionFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int nPropertyIndex = 5;

        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionHydraulicLoadsProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public DuneErosionHydraulicLoadsProperties(DuneErosionFailureMechanism data,
                                                 IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism> handler) : base(data, new ConstructionProperties
        {
            NamePropertyIndex = namePropertyIndex,
            CodePropertyIndex = codePropertyIndex,
            GroupPropertyIndex = groupPropertyIndex,
            ContributionPropertyIndex = contributionPropertyIndex,
            NPropertyIndex = nPropertyIndex
        }, handler) {}
    }
}