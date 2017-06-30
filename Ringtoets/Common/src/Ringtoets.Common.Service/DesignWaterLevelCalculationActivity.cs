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
        private readonly DesignWaterLevelCalculation designWaterLevelCalculation;
        private readonly double norm;
        private readonly string hydraulicBoundaryDatabaseFilePath;
        private readonly ICalculationMessageProvider messageProvider;
        private readonly DesignWaterLevelCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationActivity"/>.
        /// </summary>
        /// <param name="designWaterLevelCalculation">The <see cref="DesignWaterLevelCalculation"/> to perform the calculation for.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The provider of the messages to use during the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designWaterLevelCalculation"/> or 
        /// <paramref name="messageProvider"/>is <c>null</c>.</exception>
        public DesignWaterLevelCalculationActivity(DesignWaterLevelCalculation designWaterLevelCalculation,
                                                   string hydraulicBoundaryDatabaseFilePath,
                                                   double norm,
                                                   ICalculationMessageProvider messageProvider)
        {
            if (designWaterLevelCalculation == null)
            {
                throw new ArgumentNullException(nameof(designWaterLevelCalculation));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            this.designWaterLevelCalculation = designWaterLevelCalculation;
            this.messageProvider = messageProvider;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.norm = norm;

            calculationService = new DesignWaterLevelCalculationService();

            Description = messageProvider.GetActivityDescription(designWaterLevelCalculation.GetName());
        }

        protected override bool Validate()
        {
            if (AlreadyCalculated)
            {
                State = ActivityState.Skipped;
                return true;
            }

            return DesignWaterLevelCalculationService.Validate(hydraulicBoundaryDatabaseFilePath);
        }

        protected override void PerformCalculation()
        {
            if (State != ActivityState.Skipped)
            {
                calculationService.Calculate(
                    designWaterLevelCalculation,
                    hydraulicBoundaryDatabaseFilePath,
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
            designWaterLevelCalculation.GetObservableObject().NotifyObservers();
        }

        private bool AlreadyCalculated
        {
            get
            {
                return designWaterLevelCalculation.IsCalculated();
            }
        }
    }
}