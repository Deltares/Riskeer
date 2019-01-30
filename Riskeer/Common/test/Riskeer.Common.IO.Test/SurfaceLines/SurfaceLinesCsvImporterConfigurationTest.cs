// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.IO.SurfaceLines;

namespace Riskeer.Common.IO.Test.SurfaceLines
{
    [TestFixture]
    public class SurfaceLinesCsvImporterConfigurationTest
    {
        [Test]
        public void Constructor_ValidParameters_ValuesAsExpected()
        {
            // Setup
            var mocks = new MockRepository();
            var strategy = mocks.Stub<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            var transformer = mocks.Stub<ISurfaceLineTransformer<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            // Call
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, strategy);

            // Assert
            Assert.AreSame(strategy, configuration.UpdateStrategy);
            Assert.AreSame(transformer, configuration.Transformer);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Call
            var mocks = new MockRepository();
            var transformer = mocks.Stub<ISurfaceLineTransformer<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            TestDelegate test = () => new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("updateStrategy", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_TransformerNull_ThrowsArgumentNullException()
        {
            // Call
            var mocks = new MockRepository();
            var strategy = mocks.Stub<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            TestDelegate test = () => new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(null, strategy);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("transformer", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}