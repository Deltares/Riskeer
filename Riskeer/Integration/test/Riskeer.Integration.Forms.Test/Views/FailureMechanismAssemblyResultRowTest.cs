// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismAssemblyResultRowTest
    {
        private const int categoryIndex = 3;

        [Test]
        public void Constructor_GetFailureMechanismAssemblyNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismAssemblyResultRow(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getFailureMechanismAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";
            int failureMechanismGroup = random.Next();

            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.Group).Return(failureMechanismGroup);
            mocks.ReplayAll();

            // Call
            var row = new FailureMechanismAssemblyResultRow(failureMechanism, () => failureMechanismAssembly);

            // Assert
            Assert.IsInstanceOf<FailureMechanismAssemblyResultRowBase>(row);

            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRow, EnumTypeConverter>(
                nameof(FailureMechanismAssemblyResultRow.CategoryGroup));

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(1, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, categoryIndex);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(failureMechanismGroup, row.Group);
            Assert.AreEqual(failureMechanismAssembly.Probability, row.Probability);
            Assert.AreEqual(failureMechanismAssembly.Group, row.CategoryGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingThrowsException_ThenCategorySetToNone()
        {
            // Given
            var random = new Random(21);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var throwException = false;
            const string exceptionMessage = "Message";
            Func<FailureMechanismAssembly> getFailureMechanismAssembly = () =>
            {
                if (!throwException)
                {
                    return failureMechanismAssembly;
                }

                throw new AssemblyException(exceptionMessage);
            };

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, getFailureMechanismAssembly);

            // Precondition
            Assert.AreEqual(failureMechanismAssembly.Probability, row.Probability);
            Assert.AreEqual(failureMechanismAssembly.Group, row.CategoryGroup);

            // When
            throwException = true;
            row.Update();

            // Then
            Assert.AreEqual(exceptionMessage, row.ColumnStateDefinitions[categoryIndex].ErrorText);

            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.None, row.CategoryGroup);
            Assert.IsNaN(row.Probability);
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingDoesNotThrowException_ThenExpectedColumnStates()
        {
            // Given
            var random = new Random(21);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var throwException = true;
            const string exceptionMessage = "Message";
            Func<FailureMechanismAssembly> getFailureMechanismAssembly = () =>
            {
                if (throwException)
                {
                    throw new AssemblyException(exceptionMessage);
                }

                return failureMechanismAssembly;
            };

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, getFailureMechanismAssembly);

            // Precondition
            Assert.AreEqual(exceptionMessage, row.ColumnStateDefinitions[categoryIndex].ErrorText);

            // When
            throwException = false;
            row.Update();

            // Then
            Assert.IsEmpty(row.ColumnStateDefinitions[categoryIndex].ErrorText);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismAssemblyCategoryGroupColorCases))]
        public void GivenRow_WhenUpdating_ThenCategoryColumnStateDefinitionSet(FailureMechanismAssemblyCategoryGroup categoryGroup,
                                                                               Color expectedBackgroundColor)
        {
            // Given
            var random = new Random(21);
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new FailureMechanismAssemblyResultRow(failureMechanism,
                                                            () => new FailureMechanismAssembly(random.NextDouble(),
                                                                                               categoryGroup));
            // When
            row.Update();

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[categoryIndex],
                                                                                          expectedBackgroundColor);
        }

        [Test]
        public void GivenValidRow_WhenUpdating_ThenColumnReadOnlyStateRemainsUnchanged()
        {
            // Given
            var random = new Random(21);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new FailureMechanismAssemblyResultRow(failureMechanism, () => failureMechanismAssembly);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.IsTrue(columnStateDefinitions[categoryIndex].ReadOnly);

            // When 
            row.Update();

            // Then
            Assert.IsTrue(columnStateDefinitions[categoryIndex].ReadOnly);
        }
    }
}