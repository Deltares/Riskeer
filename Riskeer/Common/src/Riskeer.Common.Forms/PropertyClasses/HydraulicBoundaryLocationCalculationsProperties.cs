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

using System;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="HydraulicBoundaryLocationCalculation"/> for properties panel.
    /// </summary>
    public abstract class HydraulicBoundaryLocationCalculationsProperties : ObjectProperties<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>, IDisposable
    {
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculations">The collection of hydraulic boundary location calculations to set as data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculations"/> is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationCalculationsProperties(IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations)
        {
            if (hydraulicBoundaryLocationCalculations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocationCalculations));
            }

            hydraulicBoundaryLocationCalculationsObserver = new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(OnRefreshRequired, hblc => hblc)
            {
                Observable = hydraulicBoundaryLocationCalculations
            };

            Data = hydraulicBoundaryLocationCalculations;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                hydraulicBoundaryLocationCalculationsObserver.Dispose();
            }
        }
    }
}