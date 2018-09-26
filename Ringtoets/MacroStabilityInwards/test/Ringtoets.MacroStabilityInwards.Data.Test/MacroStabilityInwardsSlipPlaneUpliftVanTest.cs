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
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSlipPlaneUpliftVanTest
    {
        [Test]
        public void Constructor_LeftGridNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlipPlaneUpliftVan(null, grid, Enumerable.Empty<RoundedDouble>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("leftGrid", exception.ParamName);
        }

        [Test]
        public void Constructor_RightGridNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlipPlaneUpliftVan(grid, null, Enumerable.Empty<RoundedDouble>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("rightGrid", exception.ParamName);
        }

        [Test]
        public void Constructor_TangentLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsGrid leftGrid = MacroStabilityInwardsGridTestFactory.Create();
            MacroStabilityInwardsGrid rightGrid = MacroStabilityInwardsGridTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tangentLines", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            MacroStabilityInwardsGrid leftGrid = MacroStabilityInwardsGridTestFactory.Create();
            MacroStabilityInwardsGrid rightGrid = MacroStabilityInwardsGridTestFactory.Create();
            IEnumerable<RoundedDouble> tangentLines = new[]
            {
                (RoundedDouble) 3.4567,
                (RoundedDouble) 0.1294
            };

            // Call
            var result = new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, tangentLines);

            // Assert
            Assert.IsInstanceOf<ICloneable>(result);

            Assert.AreSame(leftGrid, result.LeftGrid);
            Assert.AreSame(rightGrid, result.RightGrid);
            CollectionAssert.AreEqual(new[]
            {
                2,
                2
            }, result.TangentLines.Select(tl => tl.NumberOfDecimalPlaces));
            CollectionAssert.AreEqual(tangentLines.Select(tl => tl.Value), result.TangentLines.Select(tl => tl.Value), new DoubleWithToleranceComparer(1e-2));
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                       MacroStabilityInwardsGridTestFactory.Create(),
                                                                       new[]
                                                                       {
                                                                           random.NextRoundedDouble()
                                                                       });

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}