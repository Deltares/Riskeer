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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Integration.Forms.Factories;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Factories
{
    [TestFixture]
    public class FailureMechanismAssemblyResultRowFactoryTest
    {
        private const int probabilityIndex = 2;

        [Test]
        public void CreateRow_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismAssemblyResultRowFactory.CreateRow(null, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateRow_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            void Call() => FailureMechanismAssemblyResultRowFactory.CreateRow(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performAssemblyFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateRow_FailureMechanismNotInAssembly_ReturnsExpectedRow()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var random = new Random(21);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ProbabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>()
            });
            mocks.ReplayAll();

            failureMechanism.InAssembly = false;

            double assemblyResult = random.NextDouble();

            // Call
            FailureMechanismAssemblyResultRow row = FailureMechanismAssemblyResultRowFactory.CreateRow(failureMechanism, () => assemblyResult);

            // Assert
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.IsNaN(row.Probability);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateRow_FailureMechanismInAssemblyWithPerformAssemblyFuncReturningResult_ReturnsExpectedRow()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            });
            mocks.ReplayAll();

            failureMechanism.InAssembly = true;

            var random = new Random(21);
            double assemblyResult = random.NextDouble();

            // Call
            FailureMechanismAssemblyResultRow row = FailureMechanismAssemblyResultRowFactory.CreateRow(failureMechanism, () => assemblyResult);

            // Assert
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(assemblyResult, row.Probability);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateRow_FailureMechanismInAssemblyWithPerformAssemblyFuncThrowingAssemblyException_ReturnsExpectedRow()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";
            const string errorMessage = "I am an error";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            });
            mocks.ReplayAll();

            failureMechanism.InAssembly = true;

            // Call
            FailureMechanismAssemblyResultRow row = FailureMechanismAssemblyResultRowFactory.CreateRow(
                failureMechanism, () => throw new AssemblyException(errorMessage));

            // Assert
            Assert.AreEqual(errorMessage, row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.IsNaN(row.Probability);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateRow_FailureMechanismInAssemblyWithValidManualAssembly_ReturnsExpectedRow()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var random = new Random(21);
            double assemblyResult = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ManualFailurePathAssemblyProbability = assemblyResult,
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual
            });
            mocks.ReplayAll();

            failureMechanism.InAssembly = true;

            // Call
            FailureMechanismAssemblyResultRow row = FailureMechanismAssemblyResultRowFactory.CreateRow(failureMechanism, () => assemblyResult);

            // Assert
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(assemblyResult, row.Probability);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateRow_FailureMechanismInAssemblyWithInvalidManualAssembly_ReturnsExpectedRow()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ManualFailurePathAssemblyProbability = double.NaN,
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual
            });
            mocks.ReplayAll();

            failureMechanism.InAssembly = true;

            var random = new Random(21);

            // Call
            FailureMechanismAssemblyResultRow row = FailureMechanismAssemblyResultRowFactory.CreateRow(failureMechanism, () => random.NextDouble());

            // Assert
            Assert.AreEqual("Er moet een waarde worden ingevuld voor de faalkans.", row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.IsNaN(row.Probability);

            mocks.VerifyAll();
        }
    }
}