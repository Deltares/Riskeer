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
using System.Collections.Generic;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Exceptions;
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
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismAssemblyResultRow(null, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismAssemblyResultRow(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performAssemblyFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var random = new Random(21);
            double assemblyResult = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            mocks.ReplayAll();

            // Call
            var row = new FailureMechanismAssemblyResultRow(failureMechanism, () => assemblyResult);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRow,
                NoProbabilityValueDoubleConverter>(
                nameof(FailureMechanismAssemblyResultRow.Probability));

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(1, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, probabilityIndex);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(assemblyResult, row.Probability);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRow_WhenUpdating_ThenAssemblyFuncExecuted()
        {
            // Given
            var random = new Random(21);
            double initialProbability = random.NextDouble();
            double afterUpdateProbability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            int i = 0;
            Func<double> performAssemblyFunc = () =>
            {
                if (i == 0)
                {
                    i++;
                    return initialProbability;
                }

                return afterUpdateProbability;
            };

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, performAssemblyFunc);

            // Precondition 
            Assert.AreEqual(initialProbability, row.Probability);

            // When
            row.Update();

            // Then
            Assert.AreEqual(afterUpdateProbability, row.Probability);
        }

        [Test]
        public void GivenRowWithoutError_WhenUpdatingAndAssemblyThrowsAssemblyException_ThenDefaultValueAndErrorSet()
        {
            // Given
            const string exceptionMessage = "Something went wrong";

            var random = new Random(21);
            double initialProbability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            int i = 0;
            Func<double> performAssemblyFunc = () =>
            {
                if (i == 0)
                {
                    i++;
                    return initialProbability;
                }

                throw new AssemblyException(exceptionMessage);
            };

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, performAssemblyFunc);

            // Precondition 
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);
            Assert.AreEqual(initialProbability, row.Probability);

            // When
            row.Update();

            // Then
            Assert.IsNaN(row.Probability);
            Assert.AreEqual(exceptionMessage, row.ColumnStateDefinitions[probabilityIndex].ErrorText);
        }

        [Test]
        public void GivenRowWithError_WhenUpdatingAndAssemblyRuns_ThenValueSetAndErrorCleared()
        {
            // Given
            const string exceptionMessage = "Something went wrong";

            var random = new Random(21);
            double probability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            int i = 0;
            Func<double> performAssemblyFunc = () =>
            {
                if (i == 0)
                {
                    i++;
                    throw new AssemblyException(exceptionMessage);
                }

                return probability;
            };

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, performAssemblyFunc);

            // Precondition 
            Assert.IsNaN(row.Probability);
            Assert.AreEqual(exceptionMessage, row.ColumnStateDefinitions[probabilityIndex].ErrorText);

            // When
            row.Update();

            // Then
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);
            Assert.AreEqual(probability, row.Probability);
        }
    }
}