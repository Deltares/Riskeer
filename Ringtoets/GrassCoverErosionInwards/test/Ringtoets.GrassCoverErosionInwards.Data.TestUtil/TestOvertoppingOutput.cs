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

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Simple implementation of a <see cref="OvertoppingOutput"/>, which can be
    /// used in tests where actual output values are not important.
    /// </summary>
    public class TestOvertoppingOutput : OvertoppingOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestOvertoppingOutput"/>.
        /// </summary>
        /// <param name="reliability">The reliability to set to the output.</param>
        public TestOvertoppingOutput(double reliability)
            : base(1, true, reliability, null) {}
    }
}