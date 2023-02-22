﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service.MessageProviders;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a water height calculation.
    /// </summary>
    internal class WaveHeightCalculationActivity : CalculatableActivity
    {
        private readonly double targetProbability;
        private readonly HydraulicBoundaryCalculationSettings calculationSettings;
        private readonly ICalculationMessageProvider messageProvider;
        private readonly WaveHeightCalculationService calculationService;
        private readonly HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculationActivity"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation to perform.</param>
        /// <param name="calculationSettings">The hydraulic boundary calculation settings.</param>
        /// <param name="targetProbability">The target probability to use during the calculation.</param>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        public WaveHeightCalculationActivity(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                             HydraulicBoundaryCalculationSettings calculationSettings,
                                             double targetProbability,
                                             string calculationIdentifier)
            : base(hydraulicBoundaryLocationCalculation)
        {
            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            messageProvider = new WaveHeightCalculationMessageProvider(calculationIdentifier);

            this.hydraulicBoundaryLocationCalculation = hydraulicBoundaryLocationCalculation;
            this.calculationSettings = calculationSettings;
            this.targetProbability = targetProbability;

            calculationService = new WaveHeightCalculationService();

            Description = messageProvider.GetActivityDescription(hydraulicBoundaryLocationCalculation.HydraulicBoundaryLocation.Name);
        }

        protected override bool Validate()
        {
            return calculationService.Validate(calculationSettings);
        }

        protected override void PerformCalculation()
        {
            calculationService.Calculate(hydraulicBoundaryLocationCalculation,
                                         calculationSettings,
                                         targetProbability,
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