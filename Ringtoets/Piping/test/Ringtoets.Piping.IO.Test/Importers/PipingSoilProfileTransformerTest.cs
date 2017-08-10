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
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingSoilProfileTransformerTest
    {
        [Test]
        public void Transform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Transform_InvalidSoilProfile_ReturnsNull()
        {
            // Setup
            var invalidType = new TestSoilProfile();

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(invalidType);

            // Assert
            Assert.IsNull(transformed);
        }

        private class TestSoilProfile : ISoilProfile
        {
            public string Name { get; }
        }
    }
}