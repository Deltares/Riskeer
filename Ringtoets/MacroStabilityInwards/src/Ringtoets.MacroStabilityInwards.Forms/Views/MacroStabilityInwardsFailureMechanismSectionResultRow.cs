// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>, which takes care of the
    /// representation of properties in a grid.
    /// </summary>
    internal class MacroStabilityInwardsFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<MacroStabilityInwardsFailureMechanismSectionResult>
    {
        private const double tolerance = 1e-6;
        private readonly IEnumerable<MacroStabilityInwardsCalculationScenario> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculations"></param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismSectionResultRow(MacroStabilityInwardsFailureMechanismSectionResult sectionResult, IEnumerable<MacroStabilityInwardsCalculationScenario> calculations) : base(sectionResult)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }
            this.calculations = calculations;
        }

        /// <summary>
        /// Gets or sets the value representing the result of the layer 3 assessment.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerThree
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

        /// <summary>
        /// Gets the assessment layer two a of the <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerTwoA
        {
            get
            {
                MacroStabilityInwardsCalculationScenario[] relevantScenarios = SectionResult.GetCalculationScenarios(calculations).ToArray();
                bool relevantScenarioAvailable = relevantScenarios.Length != 0;

                if (relevantScenarioAvailable && Math.Abs(SectionResult.GetTotalContribution(relevantScenarios) - 1.0) > tolerance)
                {
                    return double.NaN;
                }

                if (!relevantScenarioAvailable || SectionResult.GetCalculationScenarioStatus(relevantScenarios) != CalculationScenarioStatus.Done)
                {
                    return double.NaN;
                }

                return SectionResult.GetAssessmentLayerTwoA(relevantScenarios);
            }
        }

        /// <summary>
        /// Gets the <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/> that is the source of this row.
        /// </summary>
        public MacroStabilityInwardsFailureMechanismSectionResult GetSectionResult
        {
            get
            {
                return SectionResult;
            }
        }
    }
}