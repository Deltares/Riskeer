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

using System.Collections.Generic;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Class that can be used to generate invalid ids for the serializable components.
    /// </summary>
    public static class InvalidIdTestHelper
    {
        /// <summary>
        /// Gets a collection of <see cref="TestCaseData"/> with invalid ids.
        /// </summary>
        public static IEnumerable<TestCaseData> InvalidIdCases
        {
            get
            {
                yield return new TestCaseData(null);
                yield return new TestCaseData("");
                yield return new TestCaseData("   ");
                yield return new TestCaseData("1nvalidId");
                yield return new TestCaseData("invalidId#");
                yield return new TestCaseData("invalid\rId");
            }
        }
    }
}