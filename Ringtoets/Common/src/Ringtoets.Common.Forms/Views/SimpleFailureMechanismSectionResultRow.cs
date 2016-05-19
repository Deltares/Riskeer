using System;
using System.Reflection;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.Views
{
    public class SimpleFailureMechanismSectionResultRow
    {
        public SimpleFailureMechanismSectionResultRow(SimpleFailureMechanismSectionResult failureMechanismSectionResult)
        {
            FailureMechanismSectionResult = failureMechanismSectionResult;
        }

        private SimpleFailureMechanismSectionResult FailureMechanismSectionResult { get; set; }

        public string Name
        {
            get
            {
                return FailureMechanismSectionResult.Section.Name;
            }
        }

        public bool AssessmentLayerOne
        {
            get
            {
                return FailureMechanismSectionResult.AssessmentLayerOne;
            }
            set
            {
                FailureMechanismSectionResult.AssessmentLayerOne = value;
                FailureMechanismSectionResult.NotifyObservers();
            }
        }

        public AssessmentLayerTwoAResult AssessmentLayerTwoA
        {
            get
            {
                return FailureMechanismSectionResult.AssessmentLayerTwoA;
            }
            set
            {
                FailureMechanismSectionResult.AssessmentLayerTwoA = value;
            }
        }

        public RoundedDouble AssessmentLayerTwoB
        {
            get
            {
                return FailureMechanismSectionResult.AssessmentLayerTwoB;
            }
            set
            {
                FailureMechanismSectionResult.AssessmentLayerTwoB = value;
            }
        }

        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return FailureMechanismSectionResult.AssessmentLayerThree;
            }
            set
            {
                FailureMechanismSectionResult.AssessmentLayerThree = value;
            }
        }
    }
}