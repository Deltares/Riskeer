// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Rhino.Mocks;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Common.Data.Test.FailurePath
{
    [TestFixture]
    public class FailurePathAssemblyHelperTest
    {
        [Test]
        public void AssembleFailurePath_FailurePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailurePathAssemblyHelper.AssembleFailurePath(null, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failurePath", exception.ParamName);
        }

        [Test]
        public void AssembleFailurePath_PerformFailurePathAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            void Call() => FailurePathAssemblyHelper.AssembleFailurePath(failurePath, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performFailurePathAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void AssembleFailurePath_WithFailurePathAssemblyProbabilityResultTypeManual_ReturnsExpectedResult()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailurePathAssemblyResult
                       {
                           ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual,
                           ManualFailurePathAssemblyProbability = probability
                       });
            mocks.ReplayAll();

            // Call
            double assemblyResult = FailurePathAssemblyHelper.AssembleFailurePath(failurePath, () => double.NaN);

            // Assert
            Assert.AreEqual(probability, assemblyResult);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailurePath_WithFailurePathAssemblyProbabilityResultTypeAutomatic_ReturnsExpectedResult()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailurePathAssemblyResult
                       {
                           ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic,
                           ManualFailurePathAssemblyProbability = double.NaN
                       });
            mocks.ReplayAll();

            // Call
            double assemblyResult = FailurePathAssemblyHelper.AssembleFailurePath(failurePath, () => probability);

            // Assert
            Assert.AreEqual(probability, assemblyResult);
            mocks.VerifyAll();
        }
    }
}