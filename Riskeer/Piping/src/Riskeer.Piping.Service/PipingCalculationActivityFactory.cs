﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Service.Probabilistic;
using Riskeer.Piping.Service.SemiProbabilistic;

namespace Riskeer.Piping.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// piping calculations.
    /// </summary>
    public static class PipingCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> based on the calculations in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing the calculations to create activities for.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="failureMechanism"/> contains calculations of a type
        /// that is not supported.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(PipingFailureMechanism failureMechanism,
                                                                                    IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return CreateCalculationActivities(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> based on the calculations in <paramref name="calculationGroup"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to create activities for.</param>
        /// <param name="failureMechanism">The failure mechanism the <paramref name="calculationGroup"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculationGroup"/> belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="calculationGroup"/> contains calculations of a type
        /// that is not supported.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(CalculationGroup calculationGroup,
                                                                                    PipingFailureMechanism failureMechanism,
                                                                                    IAssessmentSection assessmentSection)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return calculationGroup.GetCalculations()
                                   .Cast<IPipingCalculation<PipingInput>>()
                                   .Select(calc => CreateCalculationActivity(calc, failureMechanism, assessmentSection))
                                   .ToArray();
        }

        /// <summary>
        /// Creates a <see cref="CalculatableActivity"/> based on the given <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create an activity for.</param>
        /// <param name="generalPipingInput">The general piping input that is used during the calculation.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculation"/> belongs to.</param>
        /// <returns>A <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static CalculatableActivity CreateSemiProbabilisticPipingCalculationActivity(SemiProbabilisticPipingCalculation calculation,
                                                                                            GeneralPipingInput generalPipingInput,
                                                                                            IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalPipingInput == null)
            {
                throw new ArgumentNullException(nameof(generalPipingInput));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new SemiProbabilisticPipingCalculationActivity(calculation,
                                                                  generalPipingInput,
                                                                  assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation));
        }

        /// <summary>
        /// Creates a <see cref="CalculatableActivity"/> based on the given <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create an activity for.</param>
        /// <param name="failureMechanism">The failure mechanism the <paramref name="calculation"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculation"/> belongs to.</param>
        /// <returns>A <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static CalculatableActivity CreateProbabilisticPipingCalculationActivity(ProbabilisticPipingCalculation calculation,
                                                                                        PipingFailureMechanism failureMechanism,
                                                                                        IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ProbabilisticPipingCalculationActivity(calculation, failureMechanism, assessmentSection);
        }

        /// <summary>
        /// Creates a <see cref="CalculatableActivity"/> based on the provided <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a <see cref="CalculatableActivity"/> for.</param>
        /// <param name="failureMechanism">The failure mechanism the <paramref name="calculation"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculation"/> belongs to.</param>
        /// <returns>A <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <see cref="calculation"/> is of a type that is not supported.</exception>
        private static CalculatableActivity CreateCalculationActivity(IPipingCalculation<PipingInput> calculation,
                                                                      PipingFailureMechanism failureMechanism,
                                                                      IAssessmentSection assessmentSection)
        {
            switch (calculation)
            {
                case SemiProbabilisticPipingCalculation semiProbabilisticPipingCalculation:
                    return CreateSemiProbabilisticPipingCalculationActivity(semiProbabilisticPipingCalculation,
                                                                            failureMechanism.GeneralInput,
                                                                            assessmentSection);
                case ProbabilisticPipingCalculation probabilisticPipingCalculation:
                    return CreateProbabilisticPipingCalculationActivity(probabilisticPipingCalculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}