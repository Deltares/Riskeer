// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Properties;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GeneralWaveConditionsInput"/> for properties panel.
    /// </summary>
    public class GeneralWaveConditionsInputProperties : ObjectProperties<GeneralWaveConditionsInput>
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralWaveConditionsInput_A_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralWaveConditionsInput_A_Description))]
        public RoundedDouble A
        {
            get
            {
                return data.A;
            }
        }

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralWaveConditionsInput_B_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralWaveConditionsInput_B_Description))]
        public RoundedDouble B
        {
            get
            {
                return data.B;
            }
        }

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralWaveConditionsInput_C_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralWaveConditionsInput_C_Description))]
        public RoundedDouble C
        {
            get
            {
                return data.C;
            }
        }

        public override string ToString()
        {
            return "";
        }
    }
}