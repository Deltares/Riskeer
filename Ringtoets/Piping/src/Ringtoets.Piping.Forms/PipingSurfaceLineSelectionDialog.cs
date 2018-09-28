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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of <see cref="PipingSurfaceLine"/>. Upon
    /// closing of the dialog, the selected <see cref="PipingSurfaceLine"/> can be obtained.
    /// </summary>
    public class PipingSurfaceLineSelectionDialog : SelectionDialogBase<PipingSurfaceLine>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="surfaceLines">The collection of <see cref="PipingSurfaceLine"/> to show in the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingSurfaceLineSelectionDialog(IWin32Window dialogParent, IEnumerable<PipingSurfaceLine> surfaceLines)
            : base(dialogParent)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException(nameof(surfaceLines));
            }

            Text = Resources.PipingSurfaceLineSelectionDialog_Select_SurfaceLines;
            InitializeDataGridView(RingtoetsCommonFormsResources.SurfaceLine_DisplayName);

            SetDataSource(surfaceLines.Select(sl => new SelectableRow<PipingSurfaceLine>(sl, sl.Name)).ToArray());
        }
    }
}