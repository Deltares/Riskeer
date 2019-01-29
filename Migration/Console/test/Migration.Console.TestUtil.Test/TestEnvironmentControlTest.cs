// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Migration.Console.TestUtil.Test
{
    [TestFixture]
    public class TestEnvironmentControlTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var testEnvironmentControl = new TestEnvironmentControl();

            // Assert
            Assert.IsInstanceOf<EnvironmentControl>(testEnvironmentControl);
        }

        [Test]
        public void ErrorCodeCalled_ExitCalled_ReturnsTheErrorCodeByWhichExitWasCalled()
        {
            // Setup
            var testEnvironmentControl = new TestEnvironmentControl();
            var errorCodeEnum = new Random(74).NextEnumValue<ErrorCode>();
            testEnvironmentControl.Exit(errorCodeEnum);

            // Call
            ErrorCode called = testEnvironmentControl.ErrorCodeCalled;

            // Assert
            Assert.AreEqual(errorCodeEnum, called);
        }
    }
}