// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Groups;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Groups
{
    /// <summary>
    /// Assessment section assembly group boundaries calculator stub for testing purposes.
    /// </summary>
    public class AssessmentSectionAssemblyGroupBoundariesCalculatorStub : IAssessmentSectionAssemblyGroupBoundariesCalculator
    {
        /// <summary>
        /// Gets the signal flooding probability that is used in the calculation.
        /// </summary>
        public double SignalFloodingProbability { get; private set; }

        /// <summary>
        /// Gets the maximum allowable flooding probability that is used in the calculation.
        /// </summary>
        public double MaximumAllowableFloodingProbability { get; private set; }

        /// <summary>
        /// Gets or sets the output of the <see cref="AssessmentSectionAssemblyGroupBoundaries"/> calculation.
        /// </summary>
        public IEnumerable<AssessmentSectionAssemblyGroupBoundaries> AssessmentSectionAssemblyGroupBoundariesOutput { get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public IEnumerable<AssessmentSectionAssemblyGroupBoundaries> CalculateAssessmentSectionAssemblyGroupBoundaries(double signalFloodingProbability, double maximumAllowableFloodingProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyGroupBoundariesCalculatorException("Message", new Exception());
            }

            SignalFloodingProbability = signalFloodingProbability;
            MaximumAllowableFloodingProbability = maximumAllowableFloodingProbability;

            return AssessmentSectionAssemblyGroupBoundariesOutput
                   ?? (AssessmentSectionAssemblyGroupBoundariesOutput = new[]
                          {
                              new AssessmentSectionAssemblyGroupBoundaries(1, 2, AssessmentSectionAssemblyGroup.A),
                              new AssessmentSectionAssemblyGroupBoundaries(2.01, 3, AssessmentSectionAssemblyGroup.B),
                              new AssessmentSectionAssemblyGroupBoundaries(3.01, 4, AssessmentSectionAssemblyGroup.C)
                          });
        }
    }
}