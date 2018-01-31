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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Calculation.Activities;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a height structures calculation.
    /// </summary>
    public class HeightStructuresCalculationActivity : HydraRingActivityBase
    {
        private readonly StructuresCalculation<HeightStructuresInput> calculation;
        private readonly string hydraulicBoundaryDatabaseFilePath;
        private readonly HeightStructuresFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private readonly HeightStructuresCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The height structures data used for the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public HeightStructuresCalculationActivity(StructuresCalculation<HeightStructuresInput> calculation, string hydraulicBoundaryDatabaseFilePath,
                                                   HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (hydraulicBoundaryDatabaseFilePath == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabaseFilePath));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.calculation = calculation;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            Description = string.Format(RingtoetsCommonServiceResources.Perform_calculation_with_name_0_, calculation.Name);

            calculationService = new HeightStructuresCalculationService();
        }

        protected override bool Validate()
        {
            return HeightStructuresCalculationService.Validate(calculation, assessmentSection);
        }

        protected override void PerformCalculation()
        {
            calculation.ClearOutput();

            calculationService.Calculate(calculation,
                                         failureMechanism.GeneralInput,
                                         hydraulicBoundaryDatabaseFilePath,
                                         assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }
    }
}