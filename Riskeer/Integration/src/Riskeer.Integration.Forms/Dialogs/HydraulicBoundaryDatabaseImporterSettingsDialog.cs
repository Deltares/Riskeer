﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Riskeer.Common.Data.Hydraulics;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Dialogs
{
    /// <summary>
    /// A dialog which allows the user to select all required hydraulic file/directory paths. Upon
    /// closing of the dialog, the related <see cref="HydraulicBoundaryDatabase"/> can be constructed.
    /// </summary>
    public partial class HydraulicBoundaryDatabaseImporterSettingsDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseImporterSettingsDialog"/>.
        /// </summary>
        public HydraulicBoundaryDatabaseImporterSettingsDialog(IWin32Window dialogParent) : base(dialogParent, RiskeerCommonFormsResources.DatabaseIcon, 600, 200)
        {
            InitializeComponent();

            buttonConnect.Enabled = false;
            errorProvider.SetIconPadding(buttonConnect, 3);
            errorProvider.SetIconAlignment(buttonConnect, ErrorIconAlignment.MiddleLeft);
            errorProvider.SetError(buttonConnect, "Kan niet koppelen aan database: er is geen HLCD-bestand geselecteerd.");
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }
    }
}
