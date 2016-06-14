using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Piping.Data;

using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="PipingFailureMechanismSectionResult"/>, which takes care of the
    /// representation of properties in a grid.
    /// </summary>
    internal class PipingFailureMechanismSectionResultRow
    {
        private readonly IEnumerable<PipingCalculationScenario> calculations;
        private const double tolerance = 1e-6;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="PipingFailureMechanismSectionResult"/> that is 
        ///     the source of this row.</param>
        /// <param name="calculations"></param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="sectionResult"/> is
        /// <c>null</c>.</exception>
        public PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult, IEnumerable<PipingCalculationScenario> calculations)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            SectionResult = sectionResult;
            this.calculations = calculations;
        }

        /// <summary>
        /// Gets the name of the failure mechanism section.
        /// </summary>
        public string Name
        {
            get
            {
                return SectionResult.Section.Name;
            }
        }

        /// <summary>
        /// Gets or sets the value representing whether the section passed the layer 0 assessment.
        /// </summary>
        public bool AssessmentLayerOne
        {
            get
            {
                return SectionResult.AssessmentLayerOne;
            }
            set
            {
                SectionResult.AssessmentLayerOne = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the value representing the result of the layer 2a assessment.
        /// </summary>
        public string AssessmentLayerTwoA
        {
            get
            {
                var relevantScenarios = SectionResult.GetCalculationScenarios(calculations).ToArray();
                bool relevantScenarioAvailable = relevantScenarios.Length != 0;

                if (relevantScenarioAvailable && Math.Abs(SectionResult.GetTotalContribution(calculations) - 1.0) > tolerance)
                {
                    return string.Format("{0}", double.NaN);
                }

                if (!relevantScenarioAvailable || SectionResult.GetCalculationScenarioStatus(calculations) != CalculationScenarioStatus.Done)
                {
                    return Resources.FailureMechanismSectionResultRow_AssessmentLayerTwoA_No_result_dash;
                }

                var layerTwoA = SectionResult.GetAssessmentLayerTwoA(calculations).Value;

                return string.Format(CommonBaseResources.ProbabilityPerYearFormat, layerTwoA);
            }
        }

        /// <summary>
        /// Gets or sets the value representing the result of the layer 3 assessment.
        /// </summary>
        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                SectionResult.AssessmentLayerThree = value;
            }
        }

        public PipingFailureMechanismSectionResult SectionResult { get; private set; }
    }
}