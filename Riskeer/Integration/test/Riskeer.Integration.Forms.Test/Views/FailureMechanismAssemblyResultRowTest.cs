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
using Riskeer.Common.Data.FailurePath;
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
            void Call() => new FailureMechanismAssemblyResultRow(null, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ConstructorWithErrorMessage_ErrorMessageNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismAssemblyResultRow(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("errorMessage", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("I am an errorMessage")]
        public void ConstructorWithErrorMessage_WithArguments_ExpectedProperties(string errorMessage)
        {
            // Setup
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
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
            var failureMechanism = mocks.Stub<IFailurePath>();
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

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
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
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            });
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
        [TestCase(FailurePathAssemblyProbabilityResultType.Automatic, FailurePathAssemblyProbabilityResultType.Manual)]
        [TestCase(FailurePathAssemblyProbabilityResultType.Manual, FailurePathAssemblyProbabilityResultType.Automatic)]
        public void GivenRow_WhenChangingProbabilityResultTypeAndRowUpdated_ThenProbabilityUpdated(
            FailurePathAssemblyProbabilityResultType initialResultType,
            FailurePathAssemblyProbabilityResultType newResultType)
        {
            // Given
            var random = new Random(21);
            double manualAssemblyResult = random.NextDouble();
            double automaticAssemblyResult = random.NextDouble();

            var assemblyResult = new FailurePathAssemblyResult
            {
                ProbabilityResultType = initialResultType,
                ManualFailurePathAssemblyProbability = manualAssemblyResult
            };

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(assemblyResult);
            mocks.ReplayAll();

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, () => automaticAssemblyResult);

            // Precondition
            double expectedProbability = assemblyResult.IsManualProbability()
                                             ? manualAssemblyResult
                                             : automaticAssemblyResult;
            Assert.AreEqual(expectedProbability, row.Probability);

            // When
            assemblyResult.ProbabilityResultType = newResultType;
            row.Update();

            // Then
            expectedProbability = assemblyResult.IsManualProbability()
                                      ? manualAssemblyResult
                                      : automaticAssemblyResult;
            Assert.AreEqual(expectedProbability, row.Probability);
            mocks.VerifyAll();
        }

        #region ManualAssembly

        [Test]
        public void GivenRowWithManualAssemblyResultAndWithoutError_WhenUpdatingAndInvalidAssemblyResult_ThenDefaultValueAndErrorSet()
        {
            // Given
            var random = new Random(21);
            double initialProbability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            var assemblyResult = new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual,
                ManualFailurePathAssemblyProbability = initialProbability
            };
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(assemblyResult);
            mocks.ReplayAll();

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, () => random.NextDouble());

            // Precondition 
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);
            Assert.AreEqual(initialProbability, row.Probability);

            // When
            assemblyResult.ManualFailurePathAssemblyProbability = double.NaN;
            row.Update();

            // Then
            Assert.AreEqual("Er moet een waarde worden ingevuld voor de faalkans.", row.ColumnStateDefinitions[probabilityIndex].ErrorText);
            Assert.IsNaN(row.Probability);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithManualAssemblyResultAndWithError_WhenUpdatingAndValidAssemblyResult_ThenValueSetAndErrorCleared()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            var assemblyResult = new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual,
                ManualFailurePathAssemblyProbability = double.NaN
            };
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(assemblyResult);
            mocks.ReplayAll();

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, () => random.NextDouble());

            // Precondition 
            Assert.IsNotEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);
            Assert.IsNaN(row.Probability);

            // When
            double validProbability = random.NextDouble();
            assemblyResult.ManualFailurePathAssemblyProbability = validProbability;
            row.Update();

            // Then
            Assert.IsEmpty(row.ColumnStateDefinitions[probabilityIndex].ErrorText);
            Assert.AreEqual(validProbability, row.Probability);
            mocks.VerifyAll();
        }

        #endregion

        #region AutomaticAssembly

        [Test]
        public void GivenRowWithAutomaticAssemblyResult_WhenUpdating_ThenAssemblyFuncExecuted()
        {
            // Given
            var random = new Random(21);
            double initialProbability = random.NextDouble();
            double afterUpdateProbability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            });
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
            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAutomaticAssemblyResultAndWithoutError_WhenUpdatingAndAssemblyThrowsAssemblyException_ThenDefaultValueAndErrorSet()
        {
            // Given
            const string exceptionMessage = "Something went wrong";

            var random = new Random(21);
            double initialProbability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            });
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
            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAutomaticAssemblyResultAndWithError_WhenUpdatingAndAssemblyRuns_ThenValueSetAndErrorCleared()
        {
            // Given
            const string exceptionMessage = "Something went wrong";

            var random = new Random(21);
            double probability = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            failureMechanism.Stub(fm => fm.AssemblyResult).Return(new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            });
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

            mocks.VerifyAll();
        }

        #endregion
    }
}