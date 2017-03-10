// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.IO;
using Ringtoets.Revetment.IO;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationContextExportInfoTest
    {
        private ExportInfo waveConditionsExportInfo;
        private ExportInfo configurationExportInfo;

        [SetUp]
        public void Setup()
        {
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo[] exportInfos = plugin.GetExportInfos().Where(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext)).ToArray();
                waveConditionsExportInfo = exportInfos.Single(ei => ei.Name.Equals("Berekende belastingen bij verschillende waterstanden (*.csv)."));
                configurationExportInfo = exportInfos.Single(ei => ei.Name.Equals("Ringtoets berekeningenconfiguratie (*.xml)"));
            }
        }

        [Test]
        public void WaveConditionsExportInfo_Initialized_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(waveConditionsExportInfo.CreateFileExporter);
            Assert.IsNotNull(waveConditionsExportInfo.IsEnabled);
            Assert.AreEqual("Berekende belastingen bij verschillende waterstanden (*.csv).", waveConditionsExportInfo.Name);
            Assert.IsNull(waveConditionsExportInfo.Category);
            Assert.IsNull(waveConditionsExportInfo.Image);
            Assert.IsNotNull(waveConditionsExportInfo.FileFilterGenerator);
        }

        [Test]
        public void WaveConditionsExportInfo_CreateFileExporter_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                                                                                        failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = waveConditionsExportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(fileExporter);
            mocks.VerifyAll();
        }

        [Test]
        public void WaveConditionsExportInfo_FileFilterGenerator_ReturnsFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = waveConditionsExportInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilterGenerator.Filter);
        }

        [Test]
        public void WaveConditionsExportInfo_GrassCoverErosionOutwardsWaveConditionsCalculationHasOutputFalse_IsEnabledFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                                                                                        failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void WaveConditionsExportInfo_GrassCoverErosionOutwardsWaveConditionsCalculationHasOutputTrue_IsEnabledTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var output = new[]
            {
                new TestWaveConditionsOutput()
            };

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(
                new GrassCoverErosionOutwardsWaveConditionsCalculation
                {
                    Output = new GrassCoverErosionOutwardsWaveConditionsOutput(output)
                },
                failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void ConfigurationExportInfo_Initialized_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(configurationExportInfo.CreateFileExporter);
            Assert.IsNotNull(configurationExportInfo.IsEnabled);
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)", configurationExportInfo.Name);
            Assert.IsNull(configurationExportInfo.Category);
            Assert.IsNull(configurationExportInfo.Image);
            Assert.IsNotNull(configurationExportInfo.FileFilterGenerator);
        }

        [Test]
        public void ConfigurationExportInfo_CreateFileExporter_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                                                                                        failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = configurationExportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsConfigurationExporter>(fileExporter);
            mocks.VerifyAll();
        }

        [Test]
        public void ConfigurationExportInfo_FileFilterGenerator_ReturnsFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = configurationExportInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
        }

        [Test]
        public void ConfigurationExportInfo_IsEnabled_True()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(
                new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                failureMechanism,
                assessmentSection);

            // Call
            bool isEnabled = configurationExportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }
    }
}