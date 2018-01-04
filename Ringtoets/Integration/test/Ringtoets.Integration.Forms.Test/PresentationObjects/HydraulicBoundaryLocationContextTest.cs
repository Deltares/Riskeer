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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HydraulicBoundaryLocationContextTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationContext(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            // Call
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<HydraulicBoundaryLocation>>(context);
            Assert.AreSame(hydraulicBoundaryLocation, context.WrappedData);
        }

        private class TestHydraulicBoundaryLocationContext : HydraulicBoundaryLocationContext
        {
            public TestHydraulicBoundaryLocationContext(HydraulicBoundaryLocation wrappedData)
                : base(wrappedData) {}
        }
    }
}