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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;

namespace Ringtoets.DuneErosion.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DuneLocationContextTest
    {
        [Test]
        public void Constructor_DuneLocationNull_ThrowArgumentNullException()
        {
            // Setup
            var locations = new ObservableList<DuneLocation>();

            // Call
            TestDelegate test = () => new DuneLocationContext(locations, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var locations = new ObservableList<DuneLocation>();
            var location = new TestDuneLocation();

            // Call
            var context = new DuneLocationContext(locations, location);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<DuneLocation>>>(context);
            Assert.AreSame(locations, context.WrappedData);
            Assert.AreSame(location, context.DuneLocation);
        }

        [TestFixture]
        private class DuneLocationContextEqualsTest : EqualsGuidelinesTestFixture<DuneLocationContext, DerivedDuneLocationContext>
        {
            private static readonly ObservableList<DuneLocation> duneLocations = new ObservableList<DuneLocation>();
            private static readonly DuneLocation duneLocation = new TestDuneLocation();

            protected override DuneLocationContext CreateObject()
            {
                return new DuneLocationContext(duneLocations, duneLocation);
            }

            protected override DerivedDuneLocationContext CreateDerivedObject()
            {
                return new DerivedDuneLocationContext(duneLocations, duneLocation);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new DuneLocationContext(new ObservableList<DuneLocation>(), duneLocation))
                    .SetName("LocationsList");
                yield return new TestCaseData(new DuneLocationContext(duneLocations, new TestDuneLocation()))
                    .SetName("Locations");
            }
        }

        private class DerivedDuneLocationContext : DuneLocationContext
        {
            public DerivedDuneLocationContext(ObservableList<DuneLocation> wrappedList, DuneLocation duneLocation) : base(wrappedList, duneLocation) {}
        }
    }
}