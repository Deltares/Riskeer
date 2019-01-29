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

using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.Observers
{
    /// <summary>
    /// Class that observes all objects in an <typeparamref name="TFailureMechanism"/>
    /// related to its section results.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism to listen to.</typeparam>
    /// <typeparam name="TSectionResult">The type of the failure mechanism section results in the <typeparamref name="TFailureMechanism"/>.</typeparam>
    /// <typeparam name="TCalculation">The type of the calculations in the <typeparamref name="TFailureMechanism"/>.</typeparam>
    public class CalculatableFailureMechanismResultObserver<TFailureMechanism, TSectionResult, TCalculation>
        : FailureMechanismResultObserver<TFailureMechanism, TSectionResult>
        where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>, ICalculatableFailureMechanism
        where TSectionResult : FailureMechanismSectionResult
        where TCalculation : ICalculation<ICalculationInput>
    {
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="CalculatableFailureMechanismResultObserver{TFailureMechanism,TSectionResult,TCalculation}"/>.
        /// </summary>
        public CalculatableFailureMechanismResultObserver(TFailureMechanism failureMechanism)
            : base(failureMechanism)
        {
            calculationObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                NotifyObservers,
                c => c.Children);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                NotifyObservers,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<TCalculation>()
                                                   .Select(c => c.InputParameters)));

            CalculationGroup observableGroup = failureMechanism.CalculationsGroup;
            calculationObserver.Observable = observableGroup;
            calculationInputObserver.Observable = observableGroup;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                calculationObserver.Dispose();
                calculationInputObserver.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}