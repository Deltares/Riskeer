// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Creates a simple <see cref="ICalculation"/> implementation, which
    /// can have an object set as output.
    /// </summary>
    public class TestCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Gets or sets an object that represents some output of this calculation.
        /// </summary>
        public object Output { get; set; }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public void ClearOutput()
        {
            Output = null;
        }

        #region Irrelevant for test

        public string Name { get; set; }
        public Comment Comments { get; }

        #endregion
    }
}