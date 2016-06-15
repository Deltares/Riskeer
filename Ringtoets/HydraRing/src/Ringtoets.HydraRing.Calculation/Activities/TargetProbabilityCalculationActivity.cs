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
using Core.Common.Base.Service;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Activities
{
    /// <summary>
    /// <see cref="Activity"/> for running a type II calculation via Hydra-Ring:
    /// Iterate towards a target probability, provided as reliability index.
    /// </summary>
    public class TargetProbabilityCalculationActivity : HydraRingActivity
    {
        private readonly string name;
        private readonly string hlcdDirectory;
        private readonly string ringId;
        private readonly HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType;
        private readonly HydraRingUncertaintiesType uncertaintiesType;
        private readonly TargetProbabilityCalculationInput targetProbabilityCalculationInput;
        private readonly Action<TargetProbabilityCalculationOutput> handleCalculationOutputAction;
        private readonly HydraRingCalculationService hydraRingCalculationService;
        private TargetProbabilityCalculationOutput targetProbabilityCalculationOutput;

        /// <summary>
        /// Creates a new instance of the <see cref="TargetProbabilityCalculationActivity"/> class.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="targetProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <param name="beforeRunAction">The action to perform before running the calculation (like clearing output, validation, etc.).</param>
        /// <param name="handleCalculationOutputAction">The action to perform after the calculation is performed.</param>
        /// <param name="hydraRingCalculationService">The service to use for performing the calculation.</param>
        internal TargetProbabilityCalculationActivity(
            string name,
            string hlcdDirectory,
            string ringId,
            HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
            HydraRingUncertaintiesType uncertaintiesType,
            TargetProbabilityCalculationInput targetProbabilityCalculationInput,
            Action beforeRunAction,
            Action<TargetProbabilityCalculationOutput> handleCalculationOutputAction,
            HydraRingCalculationService hydraRingCalculationService)
            : base(beforeRunAction)
        {
            this.name = name;
            this.hlcdDirectory = hlcdDirectory;
            this.ringId = ringId;
            this.timeIntegrationSchemeType = timeIntegrationSchemeType;
            this.uncertaintiesType = uncertaintiesType;
            this.targetProbabilityCalculationInput = targetProbabilityCalculationInput;
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

            targetProbabilityCalculationOutput = HydraRingCalculationService.PerformCalculation(hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType, targetProbabilityCalculationInput);
        }

        protected override void OnCancel()
        {
            HydraRingCalculationService.CancelRunningCalculation();
        }

        protected override void OnFinish()
        {
            if (State == ActivityState.Executed)
            {
                handleCalculationOutputAction(targetProbabilityCalculationOutput);
            }
        }
    }
}