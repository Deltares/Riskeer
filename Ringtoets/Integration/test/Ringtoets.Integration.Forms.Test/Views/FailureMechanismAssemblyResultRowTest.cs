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
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismAssemblyResultRowTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            // Call
            TestDelegate call = () => new FailureMechanismAssemblyResultRow(null, () => failureMechanismAssembly);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

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
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRow,
                NoProbabilityValueDoubleConverter>(
                nameof(FailureMechanismAssemblyResultRow.Probablity));
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyResultRow,
                EnumTypeConverter>(
                nameof(FailureMechanismAssemblyResultRow.CategoryGroup));

            Assert.AreEqual(failureMechanismName, row.Name);
            Assert.AreEqual(failureMechanismCode, row.Code);
            Assert.AreEqual(failureMechanismGroup, row.Group);
            Assert.AreEqual(failureMechanismAssembly.Probability, row.Probablity);
            Assert.AreEqual(failureMechanismAssembly.Group, row.CategoryGroup);

            mocks.VerifyAll();
        }
    }
}