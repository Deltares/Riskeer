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
using System.Reflection;
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.IO.Exporters;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            var exporter = new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), persistenceFactory, "ValidFilePath");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationExporter(null, persistenceFactory, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PersistenceFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("persistenceFactory", exception.ParamName);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_FilePathInvalid_ThrowsArgumentException(string filePath)
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();


            // Call
            void Call() => new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), persistenceFactory, filePath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void Export_PersistenceFactoryThrowsException_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string filePath = "ValidFilePath";
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                ThrowException = true
            };

            var exporter = new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), persistenceFactory, filePath);

            // Call
            var exportResult = true;
            void Call() => exportResult = exporter.Export();

            // Assert
            string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. Er is geen D-GEO Suite Stability Project geëxporteerd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(exportResult);
        }

        [Test]
        public void Export_RunsSuccessful_SetsDataCorrectlyAndReturnsTrue()
        {
            // Setup
            const string filePath = "ValidFilePath";
            var calculation = new MacroStabilityInwardsCalculation();
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, persistenceFactory, filePath);

            // Call
            bool exportResult = exporter.Export();

            // Assert
            AssertPersistableDataModel(calculation, persistenceFactory.PersistableDataModel, filePath);
            Assert.AreEqual(filePath, persistenceFactory.FilePath);
            Assert.IsTrue(exportResult);
        }

        private void AssertPersistableDataModel(MacroStabilityInwardsCalculation calculation, PersistableDataModel persistableDataModel, string filePath)
        {
            Assert.AreEqual(filePath, persistableDataModel.Info.Path);
            Assert.AreEqual(calculation.Name, persistableDataModel.Info.Project);
            Assert.AreEqual(calculation.InputParameters.SurfaceLine?.Name, persistableDataModel.Info.CrossSection);
            Assert.AreEqual($"Riskeer {AssemblyUtils.GetAssemblyInfo(Assembly.GetAssembly(GetType())).Version}", persistableDataModel.Info.ApplicationCreated);
            Assert.AreEqual("Export from Riskeer", persistableDataModel.Info.Remarks);
            Assert.IsNotNull(persistableDataModel.Info.Created);
            Assert.IsNull(persistableDataModel.Info.Date);
            Assert.IsNull(persistableDataModel.Info.LastModified);
            Assert.IsNull(persistableDataModel.Info.LastModifier);
            Assert.IsNull(persistableDataModel.Info.Analyst);
            
            Assert.IsNull(persistableDataModel.AssessmentResults);
            Assert.IsNull(persistableDataModel.CalculationSettings);
            Assert.IsNull(persistableDataModel.Decorations);
            Assert.IsNull(persistableDataModel.Geometry);
            Assert.IsNull(persistableDataModel.Loads);
            Assert.IsNull(persistableDataModel.NailPropertiesForSoils);
            Assert.IsNull(persistableDataModel.Reinforcements);
            Assert.IsNull(persistableDataModel.SoilCorrelations);
            Assert.IsNull(persistableDataModel.SoilLayers);
            Assert.IsNull(persistableDataModel.SoilVisualizations);
            Assert.IsNull(persistableDataModel.Soils);
            Assert.IsNull(persistableDataModel.Stages);
            Assert.IsNull(persistableDataModel.WaternetCreatorSettings);
            Assert.IsNull(persistableDataModel.Waternets);
            Assert.IsNull(persistableDataModel.StateCorrelations);
            Assert.IsNull(persistableDataModel.States);
        }
    }
}