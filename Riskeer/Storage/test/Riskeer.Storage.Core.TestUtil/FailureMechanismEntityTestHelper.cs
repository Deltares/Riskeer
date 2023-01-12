// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil
{
    /// <summary>
    /// This class contains methods for testing <see cref="IFailureMechanismEntity"/>.
    /// </summary>
    public static class FailureMechanismEntityTestHelper
    {
        /// <summary>
        /// Asserts the <see cref="IFailureMechanism"/> against an <see cref="IFailureMechanismEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IFailureMechanismEntity"/> to assert with.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism{T}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when the properties do not match.</exception>
        public static void AssertIFailureMechanismEntityProperties(IFailureMechanismEntity entity, IFailureMechanism failureMechanism)
        {
            var inAssembly = Convert.ToBoolean(entity.InAssembly);

            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);

            var probabilityResultType = (FailureMechanismAssemblyProbabilityResultType) entity.FailureMechanismAssemblyResultProbabilityResultType;
            FailureMechanismAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            Assert.AreEqual(probabilityResultType, assemblyResult.ProbabilityResultType);
            Assert.AreEqual(entity.FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability, assemblyResult.ManualFailureMechanismAssemblyProbability);
        }
    }
}