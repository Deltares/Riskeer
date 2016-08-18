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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.TypeConverters;
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
        private const double tolerance = 1e-6;
        private readonly IEnumerable<PipingCalculationScenario> calculations;

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
        [TypeConverter(typeof(FailureMechanismSectionResultNoProbabilityValueDoubleConverter))]
        public double AssessmentLayerTwoA
        {
            get
            {
                var relevantScenarios = SectionResult.GetCalculationScenarios(calculations).ToArray();
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
        /// Gets or sets the value representing the result of the layer 3 assessment.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
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