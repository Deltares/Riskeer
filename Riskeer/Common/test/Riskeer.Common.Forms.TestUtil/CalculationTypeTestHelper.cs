// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.Common.Forms.TreeNodeInfos;
using RiskeerFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Helper class to get test cases when dealing with <see cref="CalculationType"/>.
    /// </summary>
    public static class CalculationTypeTestHelper
    {
        /// <summary>
        /// Gets a collection of test cases to test the images belonging to various
        /// <see cref="CalculationType"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> CalculationTypeWithImageCases
        {
            get
            {
                return new[]
                {
                    new TestCaseData(CalculationType.Hydraulic, RiskeerFormsResources.HydraulicCalculationIcon),
                    new TestCaseData(CalculationType.Probabilistic, RiskeerFormsResources.ProbabilisticCalculationIcon),
                    new TestCaseData(CalculationType.SemiProbabilistic, RiskeerFormsResources.SemiProbabilisticCalculationIcon)
                };
            }
        }
    }
}