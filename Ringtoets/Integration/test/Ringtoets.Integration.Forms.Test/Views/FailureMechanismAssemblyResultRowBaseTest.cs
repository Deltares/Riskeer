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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.Views;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismAssemblyResultRowBaseTest
    {
        private const int categoryIndex = 3;

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFailureMechanismAssemblyResultRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string failureMechanismName = "Failure Mechanism Name";
            const string failureMechanismCode = "Code";
            int failureMechanismGroup = random.Next();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Code).Return(failureMechanismCode);
            failureMechanism.Stub(fm => fm.Group).Return(failureMechanismGroup);
            mocks.ReplayAll();

            // Call
            var row = new TestFailureMechanismAssemblyResultRow(failureMechanism);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRowBase,
                NoProbabilityValueDoubleConverter>(
                nameof(FailureMechanismAssemblyResultRowBase.Probability));
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRowBase,
                EnumTypeConverter>(
                nameof(FailureMechanismAssemblyResultRowBase.CategoryGroup));

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(1, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, categoryIndex);

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(failureMechanismGroup, row.Group);
            Assert.AreEqual(0, row.Probability);
            Assert.AreEqual((FailureMechanismAssemblyCategoryGroup) 0, row.CategoryGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GetCategoryGroupColumnStateDefinition_Always_ReturnsColumnStateDefinition()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new TestFailureMechanismAssemblyResultRow(failureMechanism);

            // Call
            DataGridViewColumnStateDefinition columnStateDefinition = row.GetCategoryGrouColumnStateDefinition();

            // Assert
            Assert.AreSame(row.ColumnStateDefinitions[categoryIndex], columnStateDefinition);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismAssemblyCategoryGroupColorCases))]
        public void GivenRow_WhenUpdating_ThenCategoryColumnStateDefinitionSet(FailureMechanismAssemblyCategoryGroup categoryGroup,
                                                                               Color expectedBackgroundColor)
        {
            // Given
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new TestFailureMechanismAssemblyResultRow(failureMechanism, categoryGroup);

            // Precondition 
            Assert.IsFalse(row.TryGetDerivedDataExecuted);

            // When
            row.Update();

            // Then
            Assert.IsTrue(row.TryGetDerivedDataExecuted);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[categoryIndex],
                                                                                          expectedBackgroundColor);
        }

        [Test]
        public void GivenRow_WhenUpdating_ThenTryDerivedDataExecuted()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new TestFailureMechanismAssemblyResultRow(failureMechanism, random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            // Precondition 
            Assert.IsFalse(row.TryGetDerivedDataExecuted);

            // When
            row.Update();

            // Then
            Assert.IsTrue(row.TryGetDerivedDataExecuted);
        }

        [Test]
        public void GivenRowWithError_WhenUpdating_ClearsErrorText()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var row = new TestFailureMechanismAssemblyResultRow(failureMechanism, random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            row.ColumnStateDefinitions[categoryIndex].ErrorText = "An error text";

            // When
            row.Update();

            // Then
            Assert.IsEmpty(row.ColumnStateDefinitions[categoryIndex].ErrorText);
        }

        private class TestFailureMechanismAssemblyResultRow : FailureMechanismAssemblyResultRowBase
        {
            public TestFailureMechanismAssemblyResultRow(IFailureMechanism failureMechanism) : base(failureMechanism) {}

            public TestFailureMechanismAssemblyResultRow(IFailureMechanism failureMechanism,
                                                         FailureMechanismAssemblyCategoryGroup categoryGroup)
                : base(failureMechanism)
            {
                CategoryGroup = categoryGroup;
            }

            public bool TryGetDerivedDataExecuted { get; private set; }

            public DataGridViewColumnStateDefinition GetCategoryGrouColumnStateDefinition()
            {
                return GetCategoryGroupColumnStateDefinition();
            }

            protected override void TryGetDerivedData()
            {
                TryGetDerivedDataExecuted = true;
            }
        }
    }
}