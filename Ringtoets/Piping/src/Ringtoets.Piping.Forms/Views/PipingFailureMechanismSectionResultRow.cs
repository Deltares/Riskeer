using System;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Piping.Data;

using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    internal class PipingFailureMechanismSectionResultRow
    {
        private const double tolerance = 1e-6;

        public PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            SectionResult = sectionResult;
        }

        public string Name
        {
            get
            {
                return SectionResult.Section.Name;
            }
        }

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

        public string AssessmentLayerTwoA
        {
            get
            {
                var relevantScenarios = SectionResult.CalculationScenarios.Where(cs => cs.IsRelevant).ToArray();
                bool relevantScenarioAvailable = relevantScenarios.Length != 0;

                if (relevantScenarioAvailable && Math.Abs(SectionResult.TotalContribution - 1.0) > tolerance)
                {
                    return string.Format("{0}", double.NaN);
                }

                if (!relevantScenarioAvailable || SectionResult.CalculationScenarioStatus != CalculationScenarioStatus.Done)
                {
                    return Resources.FailureMechanismSectionResultRow_AssessmentLayerTwoA_No_result_dash;
                }

                var layerTwoA = SectionResult.AssessmentLayerTwoA.Value;

                return string.Format(CommonBaseResources.ProbabilityPerYearFormat, layerTwoA);
            }
        }

        public RoundedDouble AssessmentLayerTwoB
        {
            get
            {
                return SectionResult.AssessmentLayerTwoB;
            }
            set
            {
                SectionResult.AssessmentLayerTwoB = value;
            }
        }

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