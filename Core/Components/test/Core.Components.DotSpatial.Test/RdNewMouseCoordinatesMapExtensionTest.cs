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
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test
{
    [TestFixture]
    public class RdNewMouseCoordinatesMapExtensionTest
    {
        [Test]
        public void Constructor_WithMap_ExpectedValues()
        {
            // Setup
            var map = new Map();

            // Call
            using (var extension = new RdNewMouseCoordinatesMapExtension(map))
            {
                // Assert
                Assert.IsInstanceOf<Extension>(extension);
            }
        }

        [Test]
        public void Constructor_WithoutMap_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RdNewMouseCoordinatesMapExtension(null);

            // Assert
            const string expectedMessage = "An extension cannot be initialized without map.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Activate_Always_AddsControlToMap()
        {
            // Setup
            using (var map = new Map())
            using (var extension = new RdNewMouseCoordinatesMapExtension(map))
            {
                // Call
                extension.Activate();

                // Assert
                Assert.AreEqual(1, map.Controls.Count);
            }
        }

        [Test]
        public void Deactivate_Always_RemovesControlFromMap()
        {
            // Setup
            using (var map = new Map())
            using (var extension = new RdNewMouseCoordinatesMapExtension(map))
            {
                extension.Activate();

                // Precondition
                Assert.AreEqual(1, map.Controls.Count);

                // Call
                extension.Deactivate();

                // Assert
                Assert.AreEqual(0, map.Controls.Count);
            }
        }
    }
}