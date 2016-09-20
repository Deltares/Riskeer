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

using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.Revetment.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="HydraulicBoundaryLocation"/> from a collection.
    /// </summary>
    public class WaveConditionsInputContextHydraulicBoundaryLocationEditor :
        SelectionEditor<WaveConditionsInputContextProperties<WaveConditionsInputContext>, HydraulicBoundaryLocation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputContextHydraulicBoundaryLocationEditor"/>.
        /// </summary>
        public WaveConditionsInputContextHydraulicBoundaryLocationEditor()
        {
            DisplayMember = TypeUtils.GetMemberName<HydraulicBoundaryLocation>(hbl => hbl.Name);
        }

        protected override IEnumerable<HydraulicBoundaryLocation> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableHydraulicBoundaryLocations();
        }

        protected override HydraulicBoundaryLocation GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).HydraulicBoundaryLocation;
        }
    }
}