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

using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test.SoilProfile
{
    [TestFixture]
    public class PipingStochasticSoilModelTest
    {
        [Test]
        public void Constructor_ValidName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var model = new PipingStochasticSoilModel(name);

            // Assert
            Assert.IsInstanceOf<Observable>(model);
            Assert.IsInstanceOf<IMechanismStochasticSoilModel>(model);

            Assert.AreEqual(name, model.Name);
            CollectionAssert.IsEmpty(model.Geometry);
            CollectionAssert.IsEmpty(model.StochasticSoilProfiles);
            Assert.AreEqual(name, model.ToString());
        }
    }
}