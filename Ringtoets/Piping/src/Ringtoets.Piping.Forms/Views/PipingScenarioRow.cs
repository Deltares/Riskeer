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
                return new RoundedDouble(0, pipingCalculation.Contribution * 100);
            }
            set
            {
                pipingCalculation.Contribution = (RoundedDouble)(value / 100);
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