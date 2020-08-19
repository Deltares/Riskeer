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
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Test calculation row that can be used in testing.
    /// </summary>
    public class TestCalculationRow : CalculationRow<TestCalculation>
    {
        public event EventHandler HydraulicBoundaryLocationChanged;

        /// <summary>
        /// Creates a new instance of <see cref="TestCalculationRow"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="TestCalculation"/> this row contains.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public TestCalculationRow(TestCalculation calculation, IObservablePropertyChangeHandler propertyChangeHandler)
            : base(calculation, propertyChangeHandler) {}

        public override Point2D GetCalculationLocation()
        {
            return new Point2D(0, 0);
        }

        protected override HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get => Calculation.InputParameters.HydraulicBoundaryLocation;
            set
            {
                Calculation.InputParameters.HydraulicBoundaryLocation = value;

                HydraulicBoundaryLocationChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}