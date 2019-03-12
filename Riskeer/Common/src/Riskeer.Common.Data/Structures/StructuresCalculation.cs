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

using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Structures
{
    /// <summary>
    /// This class holds information about a calculation for a structures failure mechanism.
    /// </summary>
    /// <typeparam name="T">The type of input contained by the calculation.</typeparam>
    public class StructuresCalculation<T> : CloneableObservable, IStructuresCalculation, ICalculation<T> where T : IStructuresCalculationInput, new()
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
        /// Gets or sets the <see cref="StructuresOutput"/>, 
        /// which contains the output of a structures calculation.
        /// </summary>
        public StructuresOutput Output { get; set; }

        public string Name { get; set; }

        public bool ShouldCalculate
        {
            get
            {
                return !HasOutput || InputParameters.ShouldIllustrationPointsBeCalculated != Output.HasGeneralResult;
            }
        }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public Comment Comments { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public void ClearIllustrationPoints()
        {
            Output?.ClearIllustrationPoints();
        }

        public override object Clone()
        {
            var clone = (StructuresCalculation<T>) base.Clone();

            clone.InputParameters = (T) InputParameters.Clone();
            clone.Comments = (Comment) Comments.Clone();

            if (Output != null)
            {
                clone.Output = (StructuresOutput) Output.Clone();
            }

            return clone;
        }

        public void ClearOutput()
        {
            Output = null;
        }
    }
}