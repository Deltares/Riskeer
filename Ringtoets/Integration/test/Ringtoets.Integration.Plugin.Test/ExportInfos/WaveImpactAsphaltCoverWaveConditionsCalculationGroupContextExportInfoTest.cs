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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Integration.Data;
using Ringtoets.Revetment.IO;
using Ringtoets.Revetment.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Plugin;

namespace Ringtoets.Integration.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationGroupContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                ExportInfo info = GetExportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.IsNull(info.Name);
                Assert.IsNull(info.Category);
                Assert.IsNull(info.Image);
                Assert.IsNotNull(info.FileFilterGenerator);
            }
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = exportInfo.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<WaveConditionsExporterBase>(fileExporter);
            }
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnsFileFilter()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = exportInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void IsEnabled_NoWaveImpactAsphaltCoverWaveConditionsCalculation_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        public void IsEnabled_WaveImpactAsphaltCoverWaveConditionsCalculationHasOutputFalse_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation());

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        public void IsEnabled_WaveImpactAsphaltCoverWaveConditionsCalculationHasOutputTrue_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var output = new[]
            {
                new TestWaveConditionsOutput()
            };

            calculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(output)
            });

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsEnabled_WaveImpactAsphaltCoverWaveConditionsCalculationInSubFolder_ReturnsTrueIfHasOutput(bool hasOutput)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
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

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.AreEqual(hasOutput, isEnabled);
            }
        }

        private static ExportInfo GetExportInfo(WaveImpactAsphaltCoverPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext));
        }
    }
}