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

using System;
using Core.Common.Base;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Base class that holds information about a calculation for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    /// <typeparam name="TPipingInput">The type of calculation input.</typeparam>
    /// <typeparam name="TPipingOutput">The type of calculation output.</typeparam>
    public abstract class PipingCalculation<TPipingInput, TPipingOutput> : CloneableObservable, IPipingCalculation<TPipingInput, TPipingOutput>
        where TPipingInput : PipingInput
        where TPipingOutput : PipingOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculation{TPipingInput,TPipingOutput}"/> with default values set for some of
        /// the parameters.
        /// </summary>
        /// <param name="pipingInput">The input parameters to perform the piping calculation with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/>
        /// is <c>null</c>.</exception>
        protected PipingCalculation(TPipingInput pipingInput)
        {
            if (pipingInput == null)
            {
                throw new ArgumentNullException(nameof(pipingInput));
            }

            Name = RiskeerCommonDataResources.Calculation_DefaultName;
            InputParameters = pipingInput;
            Comments = new Comment();
        }

        /// <summary>
        /// Gets or sets the results of the piping calculation.
        /// </summary>
        public TPipingOutput Output { get; set; }

        /// <summary>
        /// Gets the input parameters to perform the piping calculation with.
        /// </summary>
        public TPipingInput InputParameters { get; private set; }

        public string Name { get; set; }

        public bool ShouldCalculate
        {
            get
            {
                return !HasOutput;
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

        public void ClearOutput()
        {
            Output = null;
        }

        public override object Clone()
        {
            var clone = (PipingCalculation<TPipingInput, TPipingOutput>) base.Clone();

            clone.Comments = (Comment) Comments.Clone();
            clone.InputParameters = (TPipingInput) InputParameters.Clone();

            if (Output != null)
            {
                clone.Output = (TPipingOutput) Output.Clone();
            }

            return clone;
        }
    }
}