﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class CalculatableRowTest
    {
        [Test]
        public void Constructor_CalculatableObjectNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SimpleCalculatableRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculatableObject", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculatableObject = new TestHydraulicBoundaryLocation();

            // Call
            var row = new SimpleCalculatableRow(calculatableObject);

            // Assert
            Assert.IsFalse(row.ShouldCalculate);
            Assert.AreSame(calculatableObject, row.CalculatableObject);
        }

        private class SimpleCalculatableRow : CalculatableRow<HydraulicBoundaryLocation>
        {
            public SimpleCalculatableRow(HydraulicBoundaryLocation calculatableObject)
                : base(calculatableObject) {}
        }
    }
}