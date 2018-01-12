﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.Forms.ProgressDialog;
using log4net;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for calculating the <see cref="DuneLocation"/>.
    /// </summary>
    public class DuneLocationCalculationGuiService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneLocationCalculationGuiService));
        private readonly IWin32Window viewParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuneLocationCalculationGuiService"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input parameter is <c>null</c>.</exception>
        public DuneLocationCalculationGuiService(IWin32Window viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException(nameof(viewParent));
            }

            this.viewParent = viewParent;
        }

        /// <summary>
        /// Performs the calculation for all <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The <see cref="DuneLocation"/> objects to perform the calculation for.</param>
        /// <param name="getCalculationFunc"><see cref="Func{T,TResult}"/> for obtaining a <see cref="DuneLocationCalculation"/>
        /// based on <see cref="DuneLocation"/>.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file 
        /// that should be used for performing the calculation.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> or
        /// <paramref name="getCalculationFunc"/> is <c>null</c>.</exception>
        public void Calculate(IEnumerable<DuneLocation> locations,
                              Func<DuneLocation, DuneLocationCalculation> getCalculationFunc,
                              string hydraulicBoundaryDatabaseFilePath,
                              string preprocessorDirectory,
                              double norm)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (getCalculationFunc == null)
            {
                throw new ArgumentNullException(nameof(getCalculationFunc));
            }

            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabaseFilePath,
                                                                                                   preprocessorDirectory);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                log.ErrorFormat(RingtoetsCommonFormsResources.CalculateHydraulicBoundaryLocation_Start_calculation_failed_0_,
                                validationProblem);
                return;
            }

            ActivityProgressDialogRunner.Run(
                viewParent,
                locations.Select(l => new DuneErosionBoundaryCalculationActivity(l,
                                                                                 hydraulicBoundaryDatabaseFilePath,
                                                                                 preprocessorDirectory,
                                                                                 norm)).ToArray());
        }
    }
}