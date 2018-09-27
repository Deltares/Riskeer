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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelImporterConfigurationTest
    {
        [Test]
        public void Constructor_ValidParameters_ValuesAsExpected()
        {
            // Setup
            var mocks = new MockRepository();
            var transformer = mocks.Stub<IStochasticSoilModelTransformer<IMechanismStochasticSoilModel>>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var strategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            // Call
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, strategy);

            // Assert
            Assert.AreSame(transformer, configuration.Transformer);
            Assert.AreSame(filter, configuration.MechanismFilter);
            Assert.AreSame(strategy, configuration.UpdateStrategy);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_TransformerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var strategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(null, filter, strategy);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("transformer", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FilterNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var transformer = mocks.Stub<IStochasticSoilModelTransformer<IMechanismStochasticSoilModel>>();
            var strategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            // Setup
            TestDelegate test = () => new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, null, strategy);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("mechanismFilter", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var transformer = mocks.Stub<IStochasticSoilModelTransformer<IMechanismStochasticSoilModel>>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("updateStrategy", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}