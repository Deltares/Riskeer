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
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Piping.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingScenariosView"/>.
    /// </summary>
    internal class PipingScenarioRow
    {
        private readonly PipingCalculationScenario pipingCalculation;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="pipingCalculation">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingCalculation"/> is <c>null</c>.</exception>
        public PipingScenarioRow(PipingCalculationScenario pipingCalculation)
        {
            if (pipingCalculation == null)
            {
                throw new ArgumentNullException("pipingCalculation");
            }

            this.pipingCalculation = pipingCalculation;
        }

        /// <summary>
        /// Gets the <see cref="PipingCalculationScenario"/> this row contains.
        /// </summary>
        public PipingCalculationScenario PipingCalculation
        {
            get
            {
                return pipingCalculation;
            }
        }

        /// <summary>
        /// Gets and sets the <see cref="PipingCalculationScenario"/> is relevant.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return pipingCalculation.IsRelevant;
            }
            set
            {
                pipingCalculation.IsRelevant = value;
                pipingCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets and sets the contribution of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble Contribution
        {
            get
            {
                return new RoundedDouble(0, pipingCalculation.Contribution*100);
            }
            set
            {
                pipingCalculation.Contribution = (RoundedDouble) (value/100);
                pipingCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the name of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return pipingCalculation.Name;
            }
        }

        /// <summary>
        /// Gets failure probabilty of piping of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(FailureMechanismSectionResultNoProbabilityValueDoubleConverter))]
        public string FailureProbabilityPiping
        {
            get
            {
                if (pipingCalculation.SemiProbabilisticOutput == null)
                {
                    return RingtoetsCommonFormsResources.FailureMechanismSectionResult_No_result_dash;
                }
                return ProbabilityFormattingHelper.Format(pipingCalculation.SemiProbabilisticOutput.PipingProbability);
            }
        }

        /// <summary>
        /// Gets failure probabilty of uplift sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(FailureMechanismSectionResultNoProbabilityValueDoubleConverter))]
        public string FailureProbabilityUplift
        {
            get
            {
                if (pipingCalculation.SemiProbabilisticOutput == null)
                {
                    return RingtoetsCommonFormsResources.FailureMechanismSectionResult_No_result_dash;
                }
                return ProbabilityFormattingHelper.Format(pipingCalculation.SemiProbabilisticOutput.UpliftProbability);
            }
        }

        /// <summary>
        /// Gets failure probabilty of heave sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(FailureMechanismSectionResultNoProbabilityValueDoubleConverter))]
        public string FailureProbabilityHeave
        {
            get
            {
                if (pipingCalculation.SemiProbabilisticOutput == null)
                {
                    return RingtoetsCommonFormsResources.FailureMechanismSectionResult_No_result_dash;
                }
                return ProbabilityFormattingHelper.Format(pipingCalculation.SemiProbabilisticOutput.HeaveProbability);
            }
        }

        /// <summary>
        /// Gets failure probabilty of sellmeijer sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(FailureMechanismSectionResultNoProbabilityValueDoubleConverter))]
        public string FailureProbabilitySellmeijer
        {
            get
            {
                if (pipingCalculation.SemiProbabilisticOutput == null)
                {
                    return RingtoetsCommonFormsResources.FailureMechanismSectionResult_No_result_dash;
                }
                return ProbabilityFormattingHelper.Format(pipingCalculation.SemiProbabilisticOutput.SellmeijerProbability);
            }
        }
    }
}