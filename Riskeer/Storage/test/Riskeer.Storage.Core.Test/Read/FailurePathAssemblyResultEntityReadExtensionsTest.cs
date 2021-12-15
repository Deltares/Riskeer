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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class FailurePathAssemblyResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var assemblyResult = new FailurePathAssemblyResult();

            // Call
            void Call() => ((IHasFailurePathAssemblyResultEntity) null).Read(assemblyResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_AssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var entity = mocks.Stub<IHasFailurePathAssemblyResultEntity>();
            mocks.ReplayAll();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assemblyResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Read_EntityWithValues_SetsAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var resultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>();
            double probability = random.NextDouble();

            var mocks = new MockRepository();
            var entity = mocks.Stub<IHasFailurePathAssemblyResultEntity>();
            mocks.ReplayAll();

            entity.FailurePathAssemblyProbabilityResultType = Convert.ToByte(resultType);
            entity.ManualFailurePathAssemblyProbability = probability;

            var assemblyResult = new FailurePathAssemblyResult();

            // Call
            entity.Read(assemblyResult);

            // Assert
            Assert.AreEqual(resultType, assemblyResult.ProbabilityResultType);
            Assert.AreEqual(probability, assemblyResult.ManualFailurePathAssemblyProbability);
        }
    }
}