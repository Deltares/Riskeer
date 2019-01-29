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

using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Interface for contexts that wrap a calculation item.
    /// </summary>
    /// <typeparam name="TCalculationBase">The type of the wrapped calculation item.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism the wrapped calculation item belongs to.</typeparam>
    public interface ICalculationContext<out TCalculationBase, out TFailureMechanism> : IObservable
        where TCalculationBase : ICalculationBase
        where TFailureMechanism : IFailureMechanism
    {
        /// <summary>
        /// Gets the wrapped calculation item.
        /// </summary>
        TCalculationBase WrappedData { get; }

        /// <summary>
        /// Gets the calculation group that owns the wrapped calculation item.
        /// </summary>
        CalculationGroup Parent { get; }

        /// <summary>
        /// Gets the failure mechanism the wrapped calculation item belongs to.
        /// </summary>
        TFailureMechanism FailureMechanism { get; }
    }
}