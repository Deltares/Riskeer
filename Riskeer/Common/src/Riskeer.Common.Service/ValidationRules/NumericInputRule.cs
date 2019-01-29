// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Base.Data;
using Riskeer.Common.Service.Properties;

namespace Riskeer.Common.Service.ValidationRules
{
    /// <summary>
    /// Validation rule to validate a numeric input.
    /// </summary>
    public class NumericInputRule : ValidationRule
    {
        private readonly RoundedDouble numericInput;
        private readonly string parameterName;

        /// <summary>
        /// Creates a new instance of <see cref="NumericInputRule"/> to validate a numeric input.
        /// </summary>
        /// <param name="numericInput">The numeric input to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The validation errors found. Collection is empty if <paramref name="numericInput"/> is valid.</returns>
        public NumericInputRule(RoundedDouble numericInput, string parameterName)
        {
            this.numericInput = numericInput;
            this.parameterName = parameterName;
        }

        public override IEnumerable<string> Validate()
        {
            if (IsNotConcreteNumber(numericInput))
            {
                yield return string.Format(Resources.NumericInputRule_Value_of_0_must_be_a_valid_number, parameterName);
            }
        }
    }
}