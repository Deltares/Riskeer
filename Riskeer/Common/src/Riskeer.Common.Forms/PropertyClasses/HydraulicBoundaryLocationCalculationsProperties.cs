﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> for properties panel.
    /// </summary>
    public abstract class HydraulicBoundaryLocationCalculationsProperties : ObjectProperties<HydraulicBoundaryLocationCalculationsForTargetProbability>, IDisposable
    {
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsProperties"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationsForTargetProbability"/> is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationCalculationsProperties(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability)
        {
            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            Data = calculationsForTargetProbability;

            hydraulicBoundaryLocationCalculationsObserver = new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(OnRefreshRequired, hblc => hblc)
            {
                Observable = calculationsForTargetProbability.HydraulicBoundaryLocationCalculations
            };
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