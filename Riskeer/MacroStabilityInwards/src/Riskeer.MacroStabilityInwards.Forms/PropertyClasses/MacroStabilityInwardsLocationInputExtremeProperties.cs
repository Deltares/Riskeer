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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stresses properties in <see cref="MacroStabilityInwardsLocationInputExtreme"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsLocationInputExtremeProperties : MacroStabilityInwardsLocationInputBaseProperties<MacroStabilityInwardsLocationInputExtreme>
    {
        private const int penetrationLengthPropertyIndex = 3;
        private readonly MacroStabilityInwardsInput macroStabilityInwardsInput;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsLocationInputExtremeProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsLocationInputExtremeProperties(MacroStabilityInwardsInput data, IObservablePropertyChangeHandler handler)
            : base((MacroStabilityInwardsLocationInputExtreme) data?.LocationInputExtreme, handler)
        {
            macroStabilityInwardsInput = data;
        }

        [PropertyOrder(penetrationLengthPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PenetrationLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PenetrationLength_Description))]
        public RoundedDouble PenetrationLength
        {
            get
            {
                return data.PenetrationLength;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PenetrationLength = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return propertyName == nameof(PenetrationLength)
                   && macroStabilityInwardsInput.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand;
        }
    }
}