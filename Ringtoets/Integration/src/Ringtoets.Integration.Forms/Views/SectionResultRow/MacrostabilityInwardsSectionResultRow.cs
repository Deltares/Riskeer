using System;
using Core.Common.Base.Data;
using Core.Common.Base.Properties;
using Ringtoets.Integration.Data.StandAlone.SectionResult;

namespace Ringtoets.Integration.Forms.Views.SectionResultRow
{
    public class MacrostabilityInwardsSectionResultRow : FailureMechanismSectionResultRow<MacrostabilityInwardsFailureMechanismSectionResult>
    {
        public MacrostabilityInwardsSectionResultRow(MacrostabilityInwardsFailureMechanismSectionResult sectionResult) : base(sectionResult) { }

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
        /// Gets or sets the value representing the result of the layer 2a assessment.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="value"/> is not in the range [0,1].</item>
        /// <item><paramref name="value"/> doesn't represent a value which can be parsed to a double value.</item>
        /// </list>
        /// </exception>
        public string AssessmentLayerTwoA
        {
            get
            {
                var d = (RoundedDouble) (1/SectionResult.AssessmentLayerTwoA);
                return string.Format(Resources.ProbabilityPerYearFormat, d);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value",Common.Forms.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResultRow_AssessmentLayerTwoA_Value_cannot_be_null);
                }
                try
                {
                    SectionResult.AssessmentLayerTwoA = (RoundedDouble) double.Parse(value);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException(Common.Forms.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResultRow_AssessmentLayerTwoA_Value_too_large);
                }
                catch (FormatException)
                {
                    throw new ArgumentException(Common.Forms.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResultRow_AssessmentLayerTwoA_Could_not_parse_string_to_double_value);
                }
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
    }
}