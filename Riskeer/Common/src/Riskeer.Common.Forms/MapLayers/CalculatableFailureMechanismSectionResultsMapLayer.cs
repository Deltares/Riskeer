// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.MapLayers
{
    /// <summary>
    /// Map layer to show section results for <see cref="ICalculatableFailureMechanism"/>.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
    /// <typeparam name="TSectionResult">The type of section result.</typeparam>
    /// <typeparam name="TCalculationInput">The type of calculation input.</typeparam>
    public class CalculatableFailureMechanismSectionResultsMapLayer<TFailureMechanism, TSectionResult, TCalculationInput> : NonCalculatableFailureMechanismSectionResultsMapLayer<TSectionResult>
        where TFailureMechanism : IFailureMechanism<TSectionResult>, ICalculatableFailureMechanism
        where TSectionResult : FailureMechanismSectionResult
        where TCalculationInput : class, ICalculationInput
    {
        private readonly RecursiveObserver<CalculationGroup, TCalculationInput> calculationInputsObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupsObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationScenario> calculationScenariosObserver;

        /// <summary>
        /// Creates a new instance of <see cref="CalculatableFailureMechanismSectionResultsMapLayer{TFailureMechanism,TSectionResult,TCalculationInput}"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to get the data from.</param>
        /// <param name="performAssemblyFunc">The <see cref="Func{T,T2}"/> used to assemble the result of a section result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public CalculatableFailureMechanismSectionResultsMapLayer(
            TFailureMechanism failureMechanism, Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
            : base(failureMechanism, performAssemblyFunc)
        {
            calculationGroupsObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateFeatures, pcg => pcg.Children)
            {
                Observable = failureMechanism.CalculationsGroup
            };

            calculationInputsObserver = new RecursiveObserver<CalculationGroup, TCalculationInput>(
                UpdateFeatures, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<ICalculation<TCalculationInput>>().Select(pc => pc.InputParameters)))
            {
                Observable = failureMechanism.CalculationsGroup
            };

            calculationScenariosObserver = new RecursiveObserver<CalculationGroup, ICalculationScenario>(UpdateFeatures, pcg => pcg.Children)
            {
                Observable = failureMechanism.CalculationsGroup
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                calculationInputsObserver.Dispose();
                calculationGroupsObserver.Dispose();
                calculationScenariosObserver.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}