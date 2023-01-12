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
using System.Collections.Generic;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismAssemblyResultRowBaseTest
    {
        private const int probabilityIndex = 2;

        [Test]
        public void ConstructorWithErrorMessage_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismAssemblyResultRow(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ConstructorWithErrorMessage_ErrorMessageNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismAssemblyResultRow(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("errorMessage", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ConstructorWithErrorMessage_WithArguments_ExpectedProperties()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";
            const string errorMessage = "Error";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            mocks.ReplayAll();

            // Call
            var row = new FailureMechanismAssemblyResultRow(failureMechanism, errorMessage);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRow,
                NoProbabilityValueDoubleConverter>(
                nameof(FailureMechanismAssemblyResultRow.Probability));

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(1, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, probabilityIndex);
            Assert.AreEqual(errorMessage, row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.IsNaN(row.Probability);

            mocks.VerifyAll();
        }

        [Test]
        public void ConstructorWithFailureMechanismAssemblyResult_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new FailureMechanismAssemblyResultRow(null, random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ConstructorWithFailureMechanismAssemblyResult_WithArguments_ExpectedProperties()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            mocks.ReplayAll();

            var random = new Random(21);
            double assemblyResult = random.NextDouble();

            // Call
            var row = new FailureMechanismAssemblyResultRow(failureMechanism, assemblyResult);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(1, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, probabilityIndex);
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(assemblyResult, row.Probability);

            mocks.VerifyAll();
        }
    }
}