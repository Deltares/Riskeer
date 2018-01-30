﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Simple <see cref="StructuresOutput"/> that can be used for tests where actual output
    /// values are not important.
    /// </summary>
    public class TestStructuresOutput : StructuresOutput
    {
        /// <summary>
        /// Creates new instance of <see cref="TestStructuresOutput"/>.
        /// </summary>
        public TestStructuresOutput()
            : this(null) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStructuresOutput"/>.
        /// </summary>
        /// <param name="probability">The probability of failure.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting 
        /// <paramref name="probability"/> that falls  outside the [0.0, 1.0] range 
        /// and isn't <see cref="double.NaN"/>.</exception>
        public TestStructuresOutput(double probability)
            : base(probability, new ProbabilityAssessmentOutput(0, 0, probability, 0, 0), null) {}

        /// <summary>
        /// Creates new instance of <see cref="TestStructuresOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result of this output with the 
        /// fault tree illustration points.</param>
        public TestStructuresOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
            : base(0, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), generalResult) {}
    }
}