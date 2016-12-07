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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Service;
using Core.Common.Gui.Forms.ProgressDialog;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;

namespace Ringtoets.Common.Forms.GuiServices
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
        /// <exception cref="ArgumentNullException">Thrown when the input parameter is <c>null</c>.</exception>
        public HydraulicBoundaryLocationCalculationGuiService(IWin32Window viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException("viewParent");
            }
            this.viewParent = viewParent;
        }

        public bool CalculateDesignWaterLevels(string hydraulicBoundaryDatabasePath,
                                               IEnumerable<HydraulicBoundaryLocation> locations,
                                               string ringId, double norm,
                                               ICalculationMessageProvider messageProvider)
        {
            if (messageProvider == null)
            {
                throw new ArgumentNullException("messageProvider");
            }

            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }
            var activities = locations.Select(location => new DesignWaterLevelCalculationActivity(location,
                                                                                                  hydraulicBoundaryDatabasePath,
                                                                                                  ringId,
                                                                                                  norm, messageProvider)).ToArray();
            return RunActivities(hydraulicBoundaryDatabasePath, activities);
        }

        public bool CalculateWaveHeights(string hydraulicBoundaryDatabasePath,
                                         IEnumerable<HydraulicBoundaryLocation> locations,
                                         string ringId, double norm,
                                         ICalculationMessageProvider messageProvider)
        {
            if (messageProvider == null)
            {
                throw new ArgumentNullException("messageProvider");
            }

            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }
            var activities = locations.Select(location => new WaveHeightCalculationActivity(location,
                                                                                            hydraulicBoundaryDatabasePath,
                                                                                            ringId,
                                                                                            norm, messageProvider)).ToArray();
            return RunActivities(hydraulicBoundaryDatabasePath, activities);
        }

        private bool RunActivities<TActivity>(string hydraulicBoundaryDatabasePath, IList<TActivity> activities) where TActivity : Activity
        {
            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabasePath);
            if (string.IsNullOrEmpty(validationProblem))
            {
                ActivityProgressDialogRunner.Run(viewParent, activities);

                return true;
            }
            log.ErrorFormat(Resources.CalculateHydraulicBoundaryLocation_ContextMenuStrip_Start_calculation_failed_0_,
                            validationProblem);
            return false;
        }
    }
}