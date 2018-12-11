﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.MessageProviders;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a water height calculation.
    /// </summary>
    internal class WaveHeightCalculationActivity : CalculatableActivity
    {
        private readonly double norm;
        private readonly HydraulicBoundaryCalculationSettings calculationSettings;
        private readonly ICalculationMessageProvider messageProvider;
        private readonly WaveHeightCalculationService calculationService;
        private readonly HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculationActivity"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation to perform.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> containing all data
        /// to perform a hydraulic boundary calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="categoryBoundaryName">The category boundary name of the calculation.</param>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public WaveHeightCalculationActivity(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                             HydraulicBoundaryCalculationSettings calculationSettings,
                                             double norm,
                                             string categoryBoundaryName)
            : base(hydraulicBoundaryLocationCalculation)
        {
            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            messageProvider = new WaveHeightCalculationMessageProvider(categoryBoundaryName);

            this.hydraulicBoundaryLocationCalculation = hydraulicBoundaryLocationCalculation;
            this.calculationSettings = calculationSettings;
            this.norm = norm;

            calculationService = new WaveHeightCalculationService();

            Description = messageProvider.GetActivityDescription(hydraulicBoundaryLocationCalculation.HydraulicBoundaryLocation.Name);
        }

        protected override bool Validate()
        {
            return calculationService.Validate(calculationSettings.HydraulicBoundaryDatabaseFilePath,
                                               calculationSettings.PreprocessorDirectory,
                                               norm);
        }

        protected override void PerformCalculation()
        {
            calculationService.Calculate(hydraulicBoundaryLocationCalculation,
                                         calculationSettings,
                                         norm,
                                         messageProvider);
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish()
        {
            hydraulicBoundaryLocationCalculation.NotifyObservers();
        }
    }
}