// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanismType = new Random(145).NextEnumValue<FailureMechanismType>();

            // Call
            TestDelegate test = () => new StochasticSoilModel(null, failureMechanismType);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const string name = "some name";
            var failureMechanismType = new Random(145).NextEnumValue<FailureMechanismType>();

            // Call
            var stochasticSoilModel = new StochasticSoilModel(name, failureMechanismType);

            // Assert
            Assert.AreEqual(name, stochasticSoilModel.Name);
            Assert.AreEqual(failureMechanismType, stochasticSoilModel.FailureMechanismType);
            CollectionAssert.IsEmpty(stochasticSoilModel.Geometry);
            CollectionAssert.IsEmpty(stochasticSoilModel.StochasticSoilProfiles);
        }
    }
}