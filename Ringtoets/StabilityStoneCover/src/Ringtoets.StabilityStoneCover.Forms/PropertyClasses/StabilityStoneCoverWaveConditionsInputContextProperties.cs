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

using System;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.Properties;

namespace Ringtoets.StabilityStoneCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityStoneCoverWaveConditionsInputContext"/> for properties panel.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsInputContextProperties
        : WaveConditionsInputContextProperties<StabilityStoneCoverWaveConditionsInputContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsInputContextProperties"/>.
        /// </summary>
        /// <param name="context">The <see cref="StabilityStoneCoverWaveConditionsInputContext"/> for which 
        /// the properties are shown.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsInputContextProperties(StabilityStoneCoverWaveConditionsInputContext context, ICalculationInputPropertyChangeHandler handler)
            : base(context, handler) { }

        public override string RevetmentType
        {
            get
            {
                return Resources.StabilityStoneCoverWaveConditionsInputContextPorperties_RevetmentType;
            }
        }
    }
}