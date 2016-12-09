// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.KernelWrapper.TestUtil
{
    /// <summary>
    /// A <see cref="PipingOutput"/> configured to be used immediately for testing purposes.
    /// </summary>
    /// <seealso cref="Ringtoets.Piping.Data.PipingOutput" />
    public class TestPipingOutput : PipingOutput
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TestPipingOutput"/> class.
        /// </summary>
        public TestPipingOutput() : base(new ConstructionProperties
        {
            HeaveZValue = 1.1,
            HeaveFactorOfSafety = 1.50,
            UpliftZValue = 6.51,
            UpliftFactorOfSafety = 3.75,
            SellmeijerZValue = 8.92,
            SellmeijerFactorOfSafety = 6.3,
            HeaveGradient = 8.4,
            SellmeijerCreepCoefficient = 9.9,
            SellmeijerCriticalFall = 4.1,
            SellmeijerReducedFall = 2.21
        }) {}
    }
}