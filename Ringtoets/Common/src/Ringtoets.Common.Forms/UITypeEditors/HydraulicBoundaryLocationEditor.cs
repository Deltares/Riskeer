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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// hydraulic boundary location from a collection.
    /// </summary>
    public class HydraulicBoundaryLocationEditor : SelectionEditor<IHasHydraulicBoundaryLocationProperty,
                                                       HydraulicBoundaryLocation, SelectableHydraulicBoundaryLocation>
    {
        protected override IEnumerable<SelectableHydraulicBoundaryLocation> GetAvailableOptions(ITypeDescriptorContext context)
        {
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations =
                GetPropertiesObject(context).GetHydraulicBoundaryLocations();
            Point2D referencePoint = GetPropertiesObject(context).GetReferenceLocation();

            return hydraulicBoundaryLocations.Select(hbl =>
                                                     new SelectableHydraulicBoundaryLocation(hbl, referencePoint))
                                             .OrderBy(hbl => hbl.Distance.Value)
                                             .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
        }

        protected override SelectableHydraulicBoundaryLocation GetCurrentOption(ITypeDescriptorContext context)
        {
            return new SelectableHydraulicBoundaryLocation(GetPropertiesObject(context).SelectedHydraulicBoundaryLocation,
                                                           GetPropertiesObject(context).GetReferenceLocation());
        }

        protected override HydraulicBoundaryLocation ConvertToDomainType(object selectedItem)
        {
            var selected = (SelectableHydraulicBoundaryLocation) selectedItem;
            return selected.HydraulicBoundaryLocation;
        }
    }
}