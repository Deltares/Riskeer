using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data
{
    public static class PipingFailureMechanismSection2aAssessmentResultExtensions
    {
        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static RoundedDouble GetAssessmentLayerTwoA(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult, IEnumerable<PipingCalculationScenario> calculations)
        {
            var calculationScenarios = pipingFailureMechanismSectionResult
                .GetCalculationScenarios(calculations)
                .Where(cs => cs.Status == CalculationScenarioStatus.Done)
                .ToList();

            return calculationScenarios.Any()
                       ? (RoundedDouble) (1.0/calculationScenarios.Sum(scenario => (scenario.Probability.Value)*scenario.Contribution.Value))
                       : (RoundedDouble) 0.0;
        }

        /// <summary>
        /// Gets the contribution of all relevant <see cref="GetCalculationScenarios"/> together.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static RoundedDouble GetTotalContribution(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult, IEnumerable<PipingCalculationScenario> calculations)
        {
            return (RoundedDouble) pipingFailureMechanismSectionResult
                .GetCalculationScenarios(calculations)
                .Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
        }

        /// <summary>
        /// Gets a list of the relevant <see cref="ICalculationScenario"/>.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static IEnumerable<PipingCalculationScenario> GetCalculationScenarios(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult, IEnumerable<PipingCalculationScenario> calculations)
        {
            var lineSegments = Math2D.ConvertLinePointsToLineSegments(pipingFailureMechanismSectionResult.Section.Points);

            return calculations
                .Where(pc => pc.IsRelevant && pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        /// <summary>
        /// Gets the status of the section result depending on the relevant calculation scenarios.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static CalculationScenarioStatus GetCalculationScenarioStatus(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult, IEnumerable<PipingCalculationScenario> calculations)
        {
            bool failed = false;
            bool notCalculated = false;
            foreach (var calculationScenario in pipingFailureMechanismSectionResult.GetCalculationScenarios(calculations).Where(cs => cs.IsRelevant))
            {
                switch (calculationScenario.Status)
                {
                    case CalculationScenarioStatus.Failed:
                        failed = true;
                        break;
                    case CalculationScenarioStatus.NotCalculated:
                        notCalculated = true;
                        break;
                    case CalculationScenarioStatus.Done:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (failed)
            {
                return CalculationScenarioStatus.Failed;
            }

            if (notCalculated)
            {
                return CalculationScenarioStatus.NotCalculated;
            }

            return CalculationScenarioStatus.Done;
        }
    }
}