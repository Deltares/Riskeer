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
using Core.Common.Base.Service;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Activities;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a design water level calculation.
    /// </summary>
    public class DesignWaterLevelCalculationActivity : HydraRingActivityBase
    {
        private readonly double norm;
        private readonly string hydraulicBoundaryDatabaseFilePath;
        private readonly string preprocessorDirectory;
        private readonly ICalculationMessageProvider messageProvider;
        private readonly DesignWaterLevelCalculationService calculationService;
        private readonly HydraulicBoundaryCalculationWrapper calculationWrapper;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationActivity"/>.
        /// </summary>
        /// <param name="calculationWrapper">The <see cref="HydraulicBoundaryCalculationWrapper"/> to perform the calculation for.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The provider of the messages to use during the calculation.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/> equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationWrapper"/> or
        /// <paramref name="messageProvider"/>is <c>null</c>.</exception>
        public DesignWaterLevelCalculationActivity(HydraulicBoundaryCalculationWrapper calculationWrapper,
                                                   string hydraulicBoundaryDatabaseFilePath,
                                                   string preprocessorDirectory,
                                                   double norm,
                                                   ICalculationMessageProvider messageProvider)
        {
            if (calculationWrapper == null)
            {
                throw new ArgumentNullException(nameof(calculationWrapper));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            this.calculationWrapper = calculationWrapper;
            this.messageProvider = messageProvider;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.preprocessorDirectory = preprocessorDirectory;
            this.norm = norm;

            calculationService = new DesignWaterLevelCalculationService();

            Description = messageProvider.GetActivityDescription(calculationWrapper.Name);
        }

        protected override bool Validate()
        {
            if (AlreadyCalculated)
            {
                State = ActivityState.Skipped;
                return true;
            }

            return DesignWaterLevelCalculationService.Validate(hydraulicBoundaryDatabaseFilePath, preprocessorDirectory);
        }

        protected override void PerformCalculation()
        {
            if (State != ActivityState.Skipped)
            {
                calculationService.Calculate(calculationWrapper,
                                             hydraulicBoundaryDatabaseFilePath,
                                             preprocessorDirectory,
                                             norm,
                                             messageProvider);
            }
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish()
        {
            calculationWrapper.ObservableObject.NotifyObservers();
        }

        private bool AlreadyCalculated
        {
            get
            {
                return calculationWrapper.IsCalculated();
            }
        }
    }
}