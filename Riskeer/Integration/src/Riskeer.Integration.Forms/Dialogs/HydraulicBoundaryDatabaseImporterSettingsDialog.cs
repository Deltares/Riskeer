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
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Dialogs
{
    /// <summary>
    /// A dialog which allows the user to select all required hydraulic file/directory paths. Upon
    /// closing of the dialog, the related <see cref="HydraulicBoundaryDatabase"/> can be constructed.
    /// </summary>
    public partial class HydraulicBoundaryDatabaseImporterSettingsDialog : DialogBase
    {
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseImporterSettingsDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which this dialog should be shown on top.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseImporterSettingsDialog(IWin32Window dialogParent,
                                                               IInquiryHelper inquiryHelper) : base(dialogParent, RiskeerCommonFormsResources.DatabaseIcon, 600, 250)
        {
            this.inquiryHelper = inquiryHelper ?? throw new ArgumentNullException(nameof(inquiryHelper));

            InitializeComponent();

            toolTipHlcd.SetToolTip(pictureBoxHlcd, Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_Tooltip_Hlcd); // TODO: Improve resource text
            toolTipHrd.SetToolTip(pictureBoxHrd, Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_Tooltip_Hrd); // TODO: Improve resource text
            toolTipLocations.SetToolTip(pictureBoxLocations, Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_Tooltip_Locations); // TODO: Improve resource text

            buttonConnect.Enabled = false;
            errorProvider.SetIconPadding(buttonConnect, 3);
            errorProvider.SetIconAlignment(buttonConnect, ErrorIconAlignment.MiddleLeft);
            errorProvider.SetError(buttonConnect, "Kan niet koppelen aan database: er is geen HLCD bestand geselecteerd.");
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        private void OnButtonHlcdClick(object sender, EventArgs e)
        {
            string sourceFileLocation = inquiryHelper.GetSourceFileLocation(Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_FileFilter_Hlcd);

            if (sourceFileLocation != null)
            {
                textBoxHlcd.Text = sourceFileLocation;
            }
        }

        private void OnButtonHrdClick(object sender, EventArgs e)
        {
            string targetFolderLocation = inquiryHelper.GetTargetFolderLocation();

            if (targetFolderLocation != null)
            {
                textBoxHrd.Text = targetFolderLocation;
            }
        }

        private void OnButtonLocationsClick(object sender, EventArgs e)
        {
            string sourceFileLocation = inquiryHelper.GetSourceFileLocation(Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_FileFilter_Locations);

            if (sourceFileLocation != null)
            {
                textBoxLocations.Text = sourceFileLocation;
            }
        }
    }
}