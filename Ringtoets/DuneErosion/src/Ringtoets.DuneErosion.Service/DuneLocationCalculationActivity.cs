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
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.DuneErosion.Data;

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
        private readonly ICalculationMessageProvider messageProvider;
        private readonly DuneLocationCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationActivity"/>.
        /// </summary>
        /// <param name="duneLocationCalculation">The <see cref="DuneLocationCalculation"/> to perform.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that 
        /// should be used for performing the calculation.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocationCalculation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public DuneLocationCalculationActivity(DuneLocationCalculation duneLocationCalculation,
                                               string hydraulicBoundaryDatabaseFilePath,
                                               string preprocessorDirectory,
                                               double norm,
                                               string categoryBoundaryName)
            : base(duneLocationCalculation)
        {
            this.duneLocationCalculation = duneLocationCalculation;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.preprocessorDirectory = preprocessorDirectory;
            this.norm = norm;

            messageProvider = new DuneLocationCalculationMessageProvider(categoryBoundaryName);

            DuneLocation duneLocation = duneLocationCalculation.DuneLocation;
            Description = messageProvider.GetActivityDescription(duneLocation.Name);

            calculationService = new DuneLocationCalculationService();
        }

        protected override bool Validate()
        {
            return calculationService.Validate(hydraulicBoundaryDatabaseFilePath,
                                               preprocessorDirectory,
                                               norm);
        }

        protected override void PerformCalculation()
        {
            calculationService.Calculate(duneLocationCalculation,
                                         norm,
                                         hydraulicBoundaryDatabaseFilePath,
                                         preprocessorDirectory,
                                         messageProvider);
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish()
        {
            duneLocationCalculation.NotifyObservers();
        }
    }
}