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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="FailureMechanismContribution"/> instances that can be used
    /// for testing.
    /// </summary>
    public static class FailureMechanismContributionTestFactory
    {
        /// <summary>
        /// Creates a new <see cref="FailureMechanismContribution"/>.
        /// </summary>
        /// <returns>The created <see cref="FailureMechanismContribution"/>.</returns>
        public static FailureMechanismContribution CreateFailureMechanismContribution()
        {
            return new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(),
                                                    1,
                                                    1.0 / 30000,
                                                    1.0 / 30000);
        }

        /// <summary>
        /// Creates a new <see cref="FailureMechanismContribution"/>.
        /// </summary>
        /// <param name="failureMechanisms">The failure mechanisms on which to base the <see cref="FailureMechanismContribution"/>.</param>
        /// <returns>The created <see cref="FailureMechanismContribution"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/> is <c>null</c>.</exception>
        public static FailureMechanismContribution CreateFailureMechanismContribution(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            return new FailureMechanismContribution(failureMechanisms,
                                                    1,
                                                    1.0 / 30000,
                                                    1.0 / 30000);
        }
    }
}