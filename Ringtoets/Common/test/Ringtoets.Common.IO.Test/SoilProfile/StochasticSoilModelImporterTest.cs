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
using Core.Common.Base;
using Core.Common.Base.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelImporterTest
    {
        private MockRepository mocks;
        private IStochasticSoilModelTransformer<IMechanismStochasticSoilModel> transformer;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            transformer = mocks.Stub<IStochasticSoilModelTransformer<IMechanismStochasticSoilModel>>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            string filePath = string.Empty;
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                null,
                filePath,
                messageProvider,
                configuration);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", parameter);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            var collection = new TestStochasticSoilModelCollection();
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                null,
                messageProvider,
                configuration);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", parameter);
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            var collection = new TestStochasticSoilModelCollection();
            string filePath = string.Empty;
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                filePath,
                null,
                configuration);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("messageProvider", parameter);
        }

        [Test]
        public void Constructor_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var collection = new TestStochasticSoilModelCollection();
            string filePath = string.Empty;

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                filePath,
                messageProvider,
                null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("configuration", parameter);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            var collection = new TestStochasticSoilModelCollection();
            string filePath = string.Empty;
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                filePath,
                messageProvider,
                configuration);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<ObservableUniqueItemCollectionWithSourcePath<IMechanismStochasticSoilModel>>>(importer);
        }

        private class TestStochasticSoilModelCollection : ObservableUniqueItemCollectionWithSourcePath<IMechanismStochasticSoilModel>
        {
            public TestStochasticSoilModelCollection()
                : base(s => s.ToString(), "something", "something else") {}
        }
    }
}