// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Service;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service.Properties;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a dune location calculation.
    /// </summary>
    public class DuneLocationCalculationActivity : CalculatableActivity
    {
        private readonly DuneLocationCalculation duneLocationCalculation;
        private readonly string hydraulicBoundaryDatabaseFilePath;
        private readonly string preprocessorDirectory;
        private readonly double norm;
        private readonly DuneLocationCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationActivity"/>.
        /// </summary>
        /// <param name="duneLocationCalculation">The <see cref="DuneLocationCalculation"/> to perform.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that 
        /// should be used for performing the calculation.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocationCalculation"/> is <c>null</c>.</exception>
        public DuneLocationCalculationActivity(DuneLocationCalculation duneLocationCalculation,
                                               string hydraulicBoundaryDatabaseFilePath,
                                               string preprocessorDirectory,
                                               double norm)
            : base(duneLocationCalculation)
        {
            this.duneLocationCalculation = duneLocationCalculation;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.preprocessorDirectory = preprocessorDirectory;
            this.norm = norm;

            DuneLocation duneLocation = duneLocationCalculation.DuneLocation;
            Description = string.Format(Resources.DuneLocationCalculationActivity_Calculate_hydraulic_boundary_conditions_for_DuneLocation_with_name_0_,
                                        duneLocation.Name);

            calculationService = new DuneLocationCalculationService();
        }

        protected override bool Validate()
        {
            if (AlreadyCalculated)
            {
                State = ActivityState.Skipped;
                return true;
            }

            return calculationService.Validate(hydraulicBoundaryDatabaseFilePath,
                                               preprocessorDirectory,
                                               norm);
        }

        protected override void PerformCalculation()
        {
            if (State != ActivityState.Skipped)
            {
                calculationService.Calculate(duneLocationCalculation,
                                             norm,
                                             hydraulicBoundaryDatabaseFilePath,
                                             preprocessorDirectory);
            }
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish()
        {
            duneLocationCalculation.NotifyObservers();
        }

        private bool AlreadyCalculated
        {
            get
            {
                return duneLocationCalculation.Output != null;
            }
        }
    }
}