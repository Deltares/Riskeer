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
using Core.Common.Base.Data;
using Core.Common.Base.Properties;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Forms.Views.SectionResultRows
{
    /// <summary>
    /// Class for displaying <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/>  as a row in a grid view.
    /// </summary>
    public class MacrostabilityOutwardsSectionResultRow : FailureMechanismSectionResultRow<MacrostabilityOutwardsFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacrostabilityOutwardsSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public MacrostabilityOutwardsSectionResultRow(MacrostabilityOutwardsFailureMechanismSectionResult sectionResult) : base(sectionResult) { }

        /// <summary>
        /// Gets or sets the value representing the result of the <see cref="MacrostabilityOutwardsFailureMechanismSectionResult.AssessmentLayerOne"/>.
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
        /// Gets or sets the value representing the result of the <see cref="MacrostabilityOutwardsFailureMechanismSectionResult.AssessmentLayerTwoA"/>.
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
        /// Gets or sets the value representing the result of the <see cref="MacrostabilityOutwardsFailureMechanismSectionResult.AssessmentLayerThree"/>.
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