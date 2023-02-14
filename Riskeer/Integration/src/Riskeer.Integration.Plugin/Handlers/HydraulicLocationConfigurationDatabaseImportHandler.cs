// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Service;
using Core.Gui.Forms;
using Core.Gui.Forms.ProgressDialog;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Importers;
using Riskeer.Integration.Plugin.Properties;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly import <see cref="HydraulicLocationConfigurationSettings"/>.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseImportHandler : IHydraulicLocationConfigurationDatabaseImportHandler
    {
        private readonly IViewParent viewParent;
        private readonly IHydraulicLocationConfigurationDatabaseUpdateHandler updateHandler;
        private readonly HydraulicBoundaryData hydraulicBoundaryData;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseImportHandler"/>.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <param name="updateHandler">The object responsible for updating the <see cref="HydraulicLocationConfigurationSettings"/>.</param>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to import the data to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        public HydraulicLocationConfigurationDatabaseImportHandler(IViewParent viewParent,
                                                                   IHydraulicLocationConfigurationDatabaseUpdateHandler updateHandler,
                                                                   HydraulicBoundaryData hydraulicBoundaryData)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException(nameof(viewParent));
            }

            if (updateHandler == null)
            {
                throw new ArgumentNullException(nameof(updateHandler));
            }

            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            this.viewParent = viewParent;
            this.updateHandler = updateHandler;
            this.hydraulicBoundaryData = hydraulicBoundaryData;
        }

        public void ImportHydraulicLocationConfigurationSettings(HydraulicLocationConfigurationSettings hydraulicLocationConfigurationSettings, string hlcdFilePath)
        {
            if (hydraulicLocationConfigurationSettings == null)
            {
                throw new ArgumentNullException(nameof(hydraulicLocationConfigurationSettings));
            }

            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            var importSettingsActivity = new FileImportActivity(
                new HydraulicLocationConfigurationDatabaseImporter(hydraulicLocationConfigurationSettings,
                                                                   updateHandler,
                                                                   hydraulicBoundaryData,
                                                                   hlcdFilePath),
                Resources.HydraulicLocationConfigurationDatabaseImportHandler_ImportHydraulicLocationConfigurationSettings_Description);
            ActivityProgressDialogRunner.Run(viewParent, importSettingsActivity);
        }
    }
}