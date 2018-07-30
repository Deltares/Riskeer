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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionReplaceStrategyTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionReplaceStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var importer = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionUpdateStrategy>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateSectionsWithImportedData_ImportedFailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionReplaceStrategy(failureMechanism).UpdateSectionsWithImportedData(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importedFailureMechanismSections", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateSectionsWithImportedData_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            // Call
            TestDelegate call = () => failureMechanismSectionReplaceStrategy.UpdateSectionsWithImportedData(
                Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourcePath", paramName);
            mocks.VerifyAll();
        }
    }
}