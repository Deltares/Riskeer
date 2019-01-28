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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.IO.WaveConditions;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationGroupContextWaveConditionsExportInfoTest
    {
        private ExportInfo exportInfo;

        [SetUp]
        public void Setup()
        {
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                exportInfo = plugin.GetExportInfos()
                                   .Single(ei => ei.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext)
                                                 && ei.Name.Equals("Berekende belastingen bij verschillende waterstanden"));
            }
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(exportInfo.CreateFileExporter);
            Assert.IsNotNull(exportInfo.IsEnabled);
            Assert.AreEqual("Algemeen", exportInfo.Category);
            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, exportInfo.Image);
            Assert.IsNotNull(exportInfo.FileFilterGenerator);
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = exportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(fileExporter);

            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = exportInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilterGenerator.Filter);
        }

        [Test]
        public void IsEnabled_NoWaveImpactAsphaltCoverWaveConditionsCalculation_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, TestName = "IsEnabled_WithOutput_ReturnExpectedEnabledState")]
        [TestCase(false, TestName = "IsEnabled_WithoutOutput_ReturnExpectedEnabledState")]
        public void IsEnabled_CalculationWithOrWithoutOutput_ReturnExpectedEnabledState(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            if (hasOutput)
            {
                var output = new[]
                {
                    new TestWaveConditionsOutput()
                };

                calculation.Output = new WaveImpactAsphaltCoverWaveConditionsOutput(output);
            }

            calculationGroup.Children.Add(calculation);

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.AreEqual(hasOutput, isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, TestName = "IsEnabled_WithOutputInSubFolder_ReturnExpectedEnabledState")]
        [TestCase(false, TestName = "IsEnabled_WithoutOutputInSubFolder_ReturnExpectedEnabledState")]
        public void IsEnabled_CalculationInSubFolder_ReturnExpectedEnabledState(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            WaveImpactAsphaltCoverWaveConditionsOutput waveImpactAsphaltCoverWaveConditionsOutput = null;
            if (hasOutput)
            {
                var output = new[]
                {
                    new TestWaveConditionsOutput()
                };
                waveImpactAsphaltCoverWaveConditionsOutput = new WaveImpactAsphaltCoverWaveConditionsOutput(output);
            }

            calculationGroup.Children.Add(
                new CalculationGroup
                {
                    Children =
                    {
                        new WaveImpactAsphaltCoverWaveConditionsCalculation
                        {
                            Output = waveImpactAsphaltCoverWaveConditionsOutput
                        }
                    }
                });

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.AreEqual(hasOutput, isEnabled);
            mocks.VerifyAll();
        }
    }
}