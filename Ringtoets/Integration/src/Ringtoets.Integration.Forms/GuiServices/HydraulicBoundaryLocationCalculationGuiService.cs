﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Service;
using Core.Common.Gui.Forms.ProgressDialog;
using log4net;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Service;

namespace Ringtoets.Integration.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for calculating the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>
    /// and <see cref="HydraulicBoundaryLocation.WaveHeight"/>.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationGuiService : IHydraulicBoundaryLocationCalculationGuiService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationCalculationGuiService));
        private readonly IWin32Window viewParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HydraulicBoundaryLocationCalculationGuiService"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        public HydraulicBoundaryLocationCalculationGuiService(IWin32Window viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException("viewParent");
            }
            this.viewParent = viewParent;
        }

        public void CalculateDesignWaterLevels(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                               IEnumerable<HydraulicBoundaryLocation> locations, string ringId, int norm)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryDatabase");
            }
            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }
            var activities = locations.Select(hbl => new DesignWaterLevelCalculationActivity(hbl,
                                                                                             hydraulicBoundaryDatabase.FilePath,
                                                                                             ringId,
                                                                                             norm)).ToArray();
            RunActivities(hydraulicBoundaryDatabase, activities);
        }

        public void CalculateWaveHeights(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                         IEnumerable<HydraulicBoundaryLocation> locations, string ringId, int norm)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryDatabase");
            }
            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }
            var activities = locations.Select(hbl => new WaveHeightCalculationActivity(hbl,
                                                                                       hydraulicBoundaryDatabase.FilePath,
                                                                                       ringId,
                                                                                       norm)).ToArray();
            RunActivities(hydraulicBoundaryDatabase, activities);
        }

        private void RunActivities<TActivity>(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                              IList<TActivity> activities) where TActivity : Activity
        {
            var hrdFile = hydraulicBoundaryDatabase.FilePath;
            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hrdFile);
            if (string.IsNullOrEmpty(validationProblem))
            {
                ActivityProgressDialogRunner.Run(viewParent, activities);

                hydraulicBoundaryDatabase.NotifyObservers();
            }
            else
            {
                log.ErrorFormat(Resources.CalculateHydraulicBoundaryLocation_ContextMenuStrip_Start_calculation_failed_0_,
                                validationProblem);
            }
        }
    }
}