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

using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Structures
{
    /// <summary>
    /// This class holds information about a calculation for a structures failure mechanism.
    /// </summary>
    public class StructuresCalculation<T> : Observable, ICalculation where T : ICalculationInput, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructuresCalculation{T}"/> class.
        /// </summary>
        public StructuresCalculation()
        {
            InputParameters = new T();
            Name = Resources.Calculation_DefaultName;
            Comments = new Comment();
        }

        /// <summary>
        /// Gets the input parameters to perform a structures calculation with.
        /// </summary>
        public T InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="ProbabilityAssessmentOutput"/>, 
        /// which contains the output of a structures calculation.
        /// </summary>
        public ProbabilityAssessmentOutput Output { get; set; }

        public string Name { get; set; }

        public Comment Comments { get; private set; }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void ClearOutput()
        {
            Output = null;
        }
    }
}