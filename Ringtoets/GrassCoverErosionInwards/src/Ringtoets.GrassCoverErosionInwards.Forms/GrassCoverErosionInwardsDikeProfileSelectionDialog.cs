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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using CommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection form a given set of <see cref="GrassCoverErosionInwardsDikeProfileSelectionDialog"/>. Upon
    /// closing of the dialog, the selected <see cref="DikeProfile"/> can be obtained.
    /// </summary>
    public partial class GrassCoverErosionInwardsDikeProfileSelectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsDikeProfileSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="dikeProfiles">The collection of <see cref="DikeProfile"/> to show in the dialog.</param>
        public GrassCoverErosionInwardsDikeProfileSelectionDialog(IWin32Window dialogParent, IEnumerable<DikeProfile> dikeProfiles)
            : base(dialogParent, CommonFormsResources.GenerateScenariosIcon, 300, 400)
        {
            InitializeComponent();

            DikeProfileSelectionView = new GrassCoverErosionInwardsDikeProfileSelectionView(dikeProfiles)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(DikeProfileSelectionView);
            SelectedDikeProfiles = new List<DikeProfile>();
        }

        /// <summary>
        /// Gets a collection of selected <see cref="DikeProfile"/> if they were selected
        /// in the dialog and a confirmation was given. If no confirmation was given or no 
        /// <see cref="DikeProfile"/> was selected, then an empty collection is returned.
        /// </summary>
        public IEnumerable<DikeProfile> SelectedDikeProfiles { get; private set; }

        protected override Button GetCancelButton()
        {
            return CustomCancelButton;
        }

        private GrassCoverErosionInwardsDikeProfileSelectionView DikeProfileSelectionView { get; set; }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            SelectedDikeProfiles = DikeProfileSelectionView.GetSelectedDikeProfiles();
            Close();
        }

        private void CancelButtonOnClick(object sender, EventArgs eventArgs)
        {
            Close();
        }
    }
}