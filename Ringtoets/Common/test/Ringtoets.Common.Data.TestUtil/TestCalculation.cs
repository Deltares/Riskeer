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
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Simple calculation that can be used in tests.
    /// </summary>
    public class TestCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Creates a new <see cref="TestCalculation"/>.
        /// </summary>
        /// <param name="name">The name of the calculation.</param>
        public TestCalculation(string name = "Nieuwe berekening")
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets an object that represents some output of this calculation.
        /// </summary>
        public object Output { get; set; }

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

        public Comment Comments { get; }

        public void ClearOutput()
        {
            Output = null;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}