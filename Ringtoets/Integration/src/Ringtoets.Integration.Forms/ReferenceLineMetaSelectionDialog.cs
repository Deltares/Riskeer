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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Forms.Views;
using CommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection form a given set of <see cref="ReferenceLineMeta"/>. Upon
    /// closing of the dialog, the selected <see cref="ReferenceLineMeta"/> can be obtained.
    /// </summary>
    public partial class ReferenceLineMetaSelectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMetaSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="referenceLineMetas">A list of <see cref="ReferenceLineMeta"/> the user can select.</param>
        public ReferenceLineMetaSelectionDialog(IWin32Window dialogParent, IEnumerable<ReferenceLineMeta> referenceLineMetas)
            : base(dialogParent, CommonFormsResources.SelectionDialogIcon, 300, 400)
        {
            InitializeComponent();
            SelectionView = new ReferenceLineMetaSelectionView(referenceLineMetas)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(SelectionView);
        }

        public ReferenceLineMeta SelectedReferenceLineMeta { get; private set; }

        private ReferenceLineMetaSelectionView SelectionView { get; set; }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}