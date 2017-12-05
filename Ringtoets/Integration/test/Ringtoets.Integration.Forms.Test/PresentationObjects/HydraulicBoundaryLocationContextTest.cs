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
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HydraulicBoundaryLocationContextTest
    {
        [Test]
        public void Constructor_NullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationContext(null, hydraulicBoundaryDatabase);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_NullHydraulicBoundariesDatabase_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationContext(hydraulicBoundaryLocation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryDatabase", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            // Call
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryLocation, hydraulicBoundaryDatabase);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<HydraulicBoundaryLocation>>(context);
            Assert.AreSame(hydraulicBoundaryDatabase, context.HydraulicBoundaryDatabase);
            Assert.AreSame(hydraulicBoundaryLocation, context.WrappedData);
        }

        [TestFixture]
        private class HydraulicBoundaryLocationContextEqualsTest
            : EqualsTestFixture<TestHydraulicBoundaryLocationContext, DerivedTestHydraulicBoundaryLocationContext>
        {
            private static readonly HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            private static readonly HydraulicBoundaryLocation hydraulicLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            protected override TestHydraulicBoundaryLocationContext CreateObject()
            {
                return new TestHydraulicBoundaryLocationContext(hydraulicLocation, hydraulicBoundaryDatabase);
            }

            protected override DerivedTestHydraulicBoundaryLocationContext CreateDerivedObject()
            {
                return new DerivedTestHydraulicBoundaryLocationContext(hydraulicLocation, hydraulicBoundaryDatabase);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new TestHydraulicBoundaryLocationContext(hydraulicLocation, new HydraulicBoundaryDatabase()))
                    .SetName("Hydraulic Boundary Database");
                yield return new TestCaseData(new TestHydraulicBoundaryLocationContext(new HydraulicBoundaryLocation(1, "Different location name", 2.0, 3.0),
                                                                                       hydraulicBoundaryDatabase))
                    .SetName("Location");
            }
        }

        private class TestHydraulicBoundaryLocationContext : HydraulicBoundaryLocationContext
        {
            public TestHydraulicBoundaryLocationContext(HydraulicBoundaryLocation wrappedData, HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
                : base(wrappedData, hydraulicBoundaryDatabase) {}
        }

        private class DerivedTestHydraulicBoundaryLocationContext : TestHydraulicBoundaryLocationContext
        {
            public DerivedTestHydraulicBoundaryLocationContext(HydraulicBoundaryLocation wrappedData, HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
                : base(wrappedData, hydraulicBoundaryDatabase) {}
        }
    }
}