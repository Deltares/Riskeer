// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Migration.Console.TestUtil;
using NUnit.Framework;

namespace Migration.Console.Test
{
    [TestFixture]
    public class EnvironmentControlTest
    {
        [Test]
        public void CurrentInstance_ExpectedProperties()
        {
            // Call
            EnvironmentControl instance = EnvironmentControl.Instance;

            // Assert
            Assert.IsInstanceOf<DefaultEnvironmentControl>(instance);
        }

        [Test]
        public void Instance_InstanceNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => EnvironmentControl.Instance = null;

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void ResetToDefault_InstanceReplaced_SetsDefaultInstance()
        {
            // Setup
            EnvironmentControl.Instance = new TestEnvironmentControl();

            // Call
            EnvironmentControl.ResetToDefault();

            // Assert
            Assert.IsInstanceOf<DefaultEnvironmentControl>(EnvironmentControl.Instance);
        }

        [Test]
        public void Exit_WithErrorCode_SetsDefaultInstance()
        {
            // Setup
            var testEnvironmentControl = new TestEnvironmentControl();
            EnvironmentControl.Instance = testEnvironmentControl;
            const int exitCode = 100;

            // Call
            EnvironmentControl.Instance.Exit(exitCode);

            // Assert
            int errorCodeCalled = testEnvironmentControl.ErrorCodeCalled;
            Assert.AreEqual(exitCode, errorCodeCalled);
        }
    }
}