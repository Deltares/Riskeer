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
using System.IO;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Properties;
using Riskeer.Integration.IO.Importers;
using CoreCommonControlsResources = Core.Common.Controls.Properties.Resources;
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
        /// <param name="settings">The settings to use for initialization of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> or
        /// <paramref name="inquiryHelper"/> is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseImporterSettingsDialog(IWin32Window dialogParent,
                                                               IInquiryHelper inquiryHelper,
                                                               HydraulicBoundaryDatabaseImporterSettings settings = null)
            : base(dialogParent, RiskeerCommonFormsResources.DatabaseIcon, 600, 250)
        {
            this.inquiryHelper = inquiryHelper ?? throw new ArgumentNullException(nameof(inquiryHelper));

            InitializeComponent();

            InitializeDialog(settings);

            UpdateButtonConnect();
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        private void InitializeDialog(HydraulicBoundaryDatabaseImporterSettings settings)
        {
            errorProvider.SetIconPadding(buttonConnect, 3);
            errorProvider.SetIconAlignment(buttonConnect, ErrorIconAlignment.MiddleLeft);

            // TODO: Improve resource texts
            toolTipHlcd.SetToolTip(pictureBoxHlcd, Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_Tooltip_Hlcd);
            toolTipHrd.SetToolTip(pictureBoxHrd, Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_Tooltip_Hrd);
            toolTipLocations.SetToolTip(pictureBoxLocations, Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_Tooltip_Locations);

            textBoxHlcd.Text = string.IsNullOrEmpty(settings?.HlcdFilePath)
                                   ? CoreCommonControlsResources.DisplayName_None
                                   : settings.HlcdFilePath;
            textBoxHrd.Text = string.IsNullOrEmpty(settings?.HrdDirectoryPath)
                                  ? CoreCommonControlsResources.DisplayName_None
                                  : settings.HrdDirectoryPath;
            textBoxLocations.Text = string.IsNullOrEmpty(settings?.LocationsFilePath)
                                        ? CoreCommonControlsResources.DisplayName_None
                                        : settings.LocationsFilePath;
        }

        private void UpdateButtonConnect()
        {
            string errorMessage = ValidateInput();

            if (string.IsNullOrEmpty(errorMessage))
            {
                buttonConnect.Enabled = true;
                errorProvider.SetError(buttonConnect, "");
            }
            else
            {
                buttonConnect.Enabled = false;
                errorProvider.SetError(buttonConnect, errorMessage);
            }
        }

        private string ValidateInput()
        {
            if (textBoxHlcd.Text.Equals(CoreCommonControlsResources.DisplayName_None))
            {
                return Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_ValidateInput_No_Hlcd_selected;
            }

            if (!File.Exists(textBoxHlcd.Text))
            {
                return Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_ValidateInput_Hlcd_does_not_exist;
            }

            if (textBoxHrd.Text.Equals(CoreCommonControlsResources.DisplayName_None))
            {
                return Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_ValidateInput_No_Hrd_selected;
            }

            if (!Directory.Exists(textBoxHrd.Text))
            {
                return Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_ValidateInput_Hrd_does_not_exist;
            }

            if (textBoxLocations.Text.Equals(CoreCommonControlsResources.DisplayName_None))
            {
                return Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_ValidateInput_No_locations_selected;
            }

            if (!File.Exists(textBoxLocations.Text))
            {
                return Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_ValidateInput_Locations_does_not_exist;
            }

            return string.Empty;
        }

        private void OnButtonHlcdClick(object sender, EventArgs e)
        {
            string sourceFileLocation = inquiryHelper.GetSourceFileLocation(Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_FileFilter_Hlcd);

            if (sourceFileLocation != null)
            {
                textBoxHlcd.Text = sourceFileLocation;

                UpdateButtonConnect();
            }
        }

        private void OnButtonHrdClick(object sender, EventArgs e)
        {
            string targetFolderLocation = inquiryHelper.GetTargetFolderLocation();

            if (targetFolderLocation != null)
            {
                textBoxHrd.Text = targetFolderLocation;

                UpdateButtonConnect();
            }
        }

        private void OnButtonLocationsClick(object sender, EventArgs e)
        {
            string sourceFileLocation = inquiryHelper.GetSourceFileLocation(Resources.HydraulicBoundaryDatabaseImporterSettingsDialog_FileFilter_Locations);

            if (sourceFileLocation != null)
            {
                textBoxLocations.Text = sourceFileLocation;

                UpdateButtonConnect();
            }
        }
    }
}