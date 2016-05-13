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
using Core.Common.Base.Service;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Activities
{
    /// <summary>
    /// <see cref="Activity"/> for running a type I calculation via Hydra-Ring:
    /// Given a set of random variables, compute the probability of failure.
    /// </summary>
    public class ExceedanceProbabilityCalculationActivity : HydraRingActivity
    {
        private readonly string name;
        private readonly string hlcdDirectory;
        private readonly string ringId;
        private readonly HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType;
        private readonly HydraRingUncertaintiesType uncertaintiesType;
        private readonly ExceedanceProbabilityCalculationInput exceedanceProbabilityCalculationInput;
        private readonly Action<ExceedanceProbabilityCalculationOutput> handleCalculationOutputAction;
        private readonly HydraRingCalculationService hydraRingCalculationService;
        private ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput;

        /// <summary>
        /// Creates a new instance of the <see cref="ExceedanceProbabilityCalculationActivity"/> class.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="exceedanceProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <param name="beforeRunAction">The action to perform before running the calculation (like clearing output, validation, etc.).</param>
        /// <param name="handleCalculationOutputAction">The action to perform after the calculation is performed.</param>
        /// <param name="hydraRingCalculationService">The service to use for performing the calculation.</param>
        internal ExceedanceProbabilityCalculationActivity(
            string name,
            string hlcdDirectory,
            string ringId,
            HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
            HydraRingUncertaintiesType uncertaintiesType,
            ExceedanceProbabilityCalculationInput exceedanceProbabilityCalculationInput,
            Action beforeRunAction,
            Action<ExceedanceProbabilityCalculationOutput> handleCalculationOutputAction,
            HydraRingCalculationService hydraRingCalculationService)
            : base(beforeRunAction)
        {
            this.name = name;
            this.hlcdDirectory = hlcdDirectory;
            this.ringId = ringId;
            this.timeIntegrationSchemeType = timeIntegrationSchemeType;
            this.uncertaintiesType = uncertaintiesType;
            this.exceedanceProbabilityCalculationInput = exceedanceProbabilityCalculationInput;
            this.handleCalculationOutputAction = handleCalculationOutputAction;
            this.hydraRingCalculationService = hydraRingCalculationService;
        }

        public override string Name
        {
            get
            {
                return name;
            }
        }

        protected override void OnRun()
        {
            base.OnRun();

            exceedanceProbabilityCalculationOutput = hydraRingCalculationService.PerformCalculation(
                hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType,
                exceedanceProbabilityCalculationInput);
        }

        protected override void OnCancel()
        {
            hydraRingCalculationService.CancelRunningCalculation();
        }

        protected override void OnFinish()
        {
            if (State == ActivityState.Executed)
            {
                handleCalculationOutputAction(exceedanceProbabilityCalculationOutput);
            }
        }
    }
}