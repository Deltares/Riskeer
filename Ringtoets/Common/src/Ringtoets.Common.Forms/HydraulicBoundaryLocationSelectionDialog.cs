// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of <see cref="HydraulicBoundaryLocation"/>. Upon
    /// closing of the dialog, the selected <see cref="HydraulicBoundaryLocation"/> can be obtained.
    /// </summary>
    public class HydraulicBoundaryLocationSelectionDialog : SelectionDialogBase<HydraulicBoundaryLocation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="hydraulicBoundaryLocations">The collection of <see cref="HydraulicBoundaryLocation"/> to show in the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryLocationSelectionDialog(IWin32Window dialogParent, IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
            : base(dialogParent)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            Text = Resources.HydraulicBoundaryLocationSelectionDialog_Select_HydraulicBoundaryLocations;
            InitializeDataGridView(Resources.HydraulicBoundaryLocation_DisplayName);

            SetDataSource(hydraulicBoundaryLocations.Select(loc => new SelectableRow<HydraulicBoundaryLocation>(loc, loc.Name)).ToArray());
        }
    }
}