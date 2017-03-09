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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.IO;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.Revetment.IO;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.ExportInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextExportInfoTest
    {
        private ExportInfo waveConditionsExportInfo;
        private ExportInfo configurationExportInfo;

        [SetUp]
        public void Setup()
        {
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo[] exportInfos = plugin.GetExportInfos().Where(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)).ToArray();
                waveConditionsExportInfo = exportInfos.Single(ei => ei.Name.Equals("Golfcondities (*.csv)."));
                configurationExportInfo = exportInfos.Single(ei => ei.Name.Equals("Configuratie van de berekeningen (*.xml)."));
            }
        }

        [Test]
        public void WaveConditionsExportInfo_Initialized_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(waveConditionsExportInfo.CreateFileExporter);
            Assert.IsNotNull(waveConditionsExportInfo.IsEnabled);
            Assert.AreEqual("Golfcondities (*.csv).", waveConditionsExportInfo.Name);
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
            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);

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
        public void WaveConditionsExportInfo_NoGrassCoverErosionOutwardsWaveConditionsCalculation_IsEnabledFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void WaveConditionsExportInfo_CalculationWithOrWithoutOutput_IsEnabledTrueOrFalse(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            if (hasOutput)
            {
                var output = new[]
                {
                    new TestWaveConditionsOutput()
                };

                calculation.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(output);
            }

            calculationGroup.Children.Add(calculation);

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExportInfo.IsEnabled(context);

            // Assert
            Assert.AreEqual(hasOutput, isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void WaveConditionsExportInfo_CalculationInSubFolder_IsEnabledTrueIfHasOutput(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            GrassCoverErosionOutwardsWaveConditionsOutput grassCoverErosionOutwardsWaveConditionsOutput = null;
            if (hasOutput)
            {
                var output = new[]
                {
                    new TestWaveConditionsOutput()
                };
                grassCoverErosionOutwardsWaveConditionsOutput = new GrassCoverErosionOutwardsWaveConditionsOutput(output);
            }
            calculationGroup.Children.Add(
                new CalculationGroup
                {
                    Children =
                    {
                        new GrassCoverErosionOutwardsWaveConditionsCalculation
                        {
                            Output = grassCoverErosionOutwardsWaveConditionsOutput
                        }
                    }
                });

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExportInfo.IsEnabled(context);

            // Assert
            Assert.AreEqual(hasOutput, isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void ConfigurationExportInfo_Initialized_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(configurationExportInfo.CreateFileExporter);
            Assert.IsNotNull(configurationExportInfo.IsEnabled);
            Assert.AreEqual("Configuratie van de berekeningen (*.xml).", configurationExportInfo.Name);
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

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                             new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                             assessmentSection);

            // Call
            IFileExporter fileExporter = configurationExportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsConfigurationExporter>(fileExporter);
        }

        [Test]
        public void ConfigurationExportInfo_FileFilterGenerator_ReturnFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = configurationExportInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
        }

        [Test]
        public void ConfigurationExportInfo_CalculationGroupNoChildren_IsEnabledFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                             new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                             assessmentSection);

            // Call
            bool isEnabled = configurationExportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ConfigurationExportInfo_CalculationGroupWithChildren_IsEnabledTrue(bool hasNestedGroup, bool hasCalculation)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            if (hasNestedGroup)
            {
                calculationGroup.Children.Add(new CalculationGroup());
            }

            if (hasCalculation)
            {
                calculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());
            }

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup,
                                                                                             new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                             assessmentSection);

            // Call
            bool isEnabled = configurationExportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}