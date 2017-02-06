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
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Components.Gis.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of background layers. Upon
    /// closing of the dialog, the selected background layer can be obtained.
    /// </summary>
    public partial class BackgroundMapDataSelectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMetaSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> is <c>null</c>.</exception>
        public BackgroundMapDataSelectionDialog(IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 500, 350)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the <see cref="MapData"/> from the selected row in the <see cref="DataGridViewControl"/>.
        /// </summary>
        public object SelectedMapData { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override Button GetCancelButton()
        {
            return null;
        }
    }
}