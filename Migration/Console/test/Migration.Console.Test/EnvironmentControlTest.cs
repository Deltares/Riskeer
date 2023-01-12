﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;

namespace Migration.Console.Test
{
    [TestFixture]
    public class EnvironmentControlTest
    {
        [Test]
        public void Instance_ExpectedProperties()
        {
            // Call
            EnvironmentControl instance = EnvironmentControl.Instance;

            // Assert
            Assert.IsNotNull(instance);
            EnvironmentControl expectedInstance = EnvironmentControl.Instance;
            Assert.AreSame(expectedInstance, instance);
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
        public void Instance_ValidEnvironmentControl_ExpectedProperties()
        {
            // Setup
            EnvironmentControl expectedInstance = EnvironmentControl.Instance;

            // Call
            EnvironmentControl.Instance = new EnvironmentControl();

            // Assert
            Assert.IsNotNull(expectedInstance);
            EnvironmentControl newInstance = EnvironmentControl.Instance;
            Assert.AreNotSame(expectedInstance, newInstance);
        }
    }
}