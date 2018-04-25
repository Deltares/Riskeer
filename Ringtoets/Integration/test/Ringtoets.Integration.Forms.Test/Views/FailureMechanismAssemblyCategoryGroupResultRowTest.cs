// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoryGroupResultRowTest
    {
        private const int categoryIndex = 3;

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoryGroupResultRow(null, () => failureMechanismAssemblyCategoryGroup);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_GetFailureMechanismAssemblyCategoryGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoryGroupResultRow(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getFailureMechanismAssemblyCategoryGroup", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";
            int failureMechanismGroup = random.Next();

            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.Group).Return(failureMechanismGroup);
            mocks.ReplayAll();

            // Call
            var row = new FailureMechanismAssemblyCategoryGroupResultRow(failureMechanism, () => failureMechanismAssemblyCategoryGroup);

            // Assert
            Assert.IsInstanceOf<FailureMechanismAssemblyResultRowBase>(row);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(1, columnStateDefinitions.Count);
            FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, categoryIndex);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(failureMechanismGroup, row.Group);
            Assert.AreEqual(failureMechanismAssemblyCategoryGroup, row.CategoryGroup);
            Assert.IsNaN(row.Probablity);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingThrowsException_ThenCategorySetToNone()
        {
            // Given
            var random = new Random(21);
            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var throwException = false;
            const string exceptionMessage = "Message";
            Func<FailureMechanismAssemblyCategoryGroup> getFailureMechanismAssembly = () =>
            {
                if (!throwException)
                {
                    return failureMechanismAssemblyCategoryGroup;
                }

                throw new AssemblyException(exceptionMessage);
            };

            var row = new FailureMechanismAssemblyCategoryGroupResultRow(failureMechanism, getFailureMechanismAssembly);

            // Precondition
            Assert.IsNaN(row.Probablity);
            Assert.AreEqual(failureMechanismAssemblyCategoryGroup, row.CategoryGroup);

            // When
            throwException = true;
            row.Update();

            // Then
            Assert.AreEqual(exceptionMessage, row.ColumnStateDefinitions[categoryIndex].ErrorText);

            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.None, row.CategoryGroup);
            Assert.IsNaN(row.Probablity);
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingDoesNotThrowException_ThenExpectedColumnStates()
        {
            // Given
            var random = new Random(21);
            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var throwException = true;
            const string exceptionMessage = "Message";
            Func<FailureMechanismAssemblyCategoryGroup> getFailureMechanismAssembly = () =>
            {
                if (throwException)
                {
                    throw new AssemblyException(exceptionMessage);
                }

                return failureMechanismAssemblyCategoryGroup;
            };

            var row = new FailureMechanismAssemblyCategoryGroupResultRow(failureMechanism, getFailureMechanismAssembly);

            // Precondition
            Assert.AreEqual(exceptionMessage, row.ColumnStateDefinitions[categoryIndex].ErrorText);

            // When
            throwException = false;
            row.Update();

            // Then
            Assert.IsEmpty(row.ColumnStateDefinitions[categoryIndex].ErrorText);
        }

        [Test]
        public void GivenValidRow_WhenUpdating_ThenColumnStyleAndReadOnlyRemainsUnchanged()
        {
            // Given
            var random = new Random(21);
            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new FailureMechanismAssemblyCategoryGroupResultRow(failureMechanism, () => failureMechanismAssemblyCategoryGroup);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[categoryIndex],
                                                                         true,
                                                                         true);

            // When 
            row.Update();

            // Then
            FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[categoryIndex],
                                                                         true,
                                                                         true);
        }
    }
}