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
using System.Linq;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsGroupBasePropertiesTest
    {
        [Test]
        public void Constructor_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationCalculationGroupProperties(null,
                                                                                                  Enumerable.Empty<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationsPerCategoryBoundaryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationCalculationGroupProperties(Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationsPerCategoryBoundary", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationGroupProperties(locations,
                                                                                         Enumerable.Empty<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IEnumerable<HydraulicBoundaryLocation>>>(properties);
            Assert.AreSame(locations, properties.Data);
        }

        private class TestHydraulicBoundaryLocationCalculationGroupProperties : HydraulicBoundaryLocationCalculationsGroupBaseProperties
        {
            public TestHydraulicBoundaryLocationCalculationGroupProperties(IEnumerable<HydraulicBoundaryLocation> locations,
                                                                           IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary)
                : base(locations, calculationsPerCategoryBoundary) {}
        }
    }
}