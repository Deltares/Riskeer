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
using Ringtoets.Revetment.IO;
using Ringtoets.Revetment.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Plugin;

namespace Ringtoets.Integration.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationContextExportInfoTest
    {
        [Test]
        public void CreateFileExporter_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                                                                                     failureMechanism, assessmentSection);
            using (WaveImpactAsphaltCoverPlugin plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = exportInfo.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<WaveConditionsExporterBase>(fileExporter);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnsFileFilter()
        {
            // Setup
            using (WaveImpactAsphaltCoverPlugin plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                ExpectedFile fileFilter = exportInfo.FileFilter;

                // Assert
                Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilter.Filter);
            }
        }

        [Test]
        public void IsEnabled_WaveImpactAsphaltCoverWaveConditionsCalculationHasOutputFalse_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                                                                                     failureMechanism, assessmentSection);
            using (WaveImpactAsphaltCoverPlugin plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_WaveImpactAsphaltCoverWaveConditionsCalculationHasOutputTrue_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var output = new[]
            {
                new TestWaveConditionsOutput()
            };

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(
                new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    Output = new WaveImpactAsphaltCoverWaveConditionsOutput(output)
                },
                failureMechanism, assessmentSection);

            using (WaveImpactAsphaltCoverPlugin plugin = new WaveImpactAsphaltCoverPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(WaveImpactAsphaltCoverPlugin plugin)
        {
            return Enumerable.First<ExportInfo>(plugin.GetExportInfos(), ei => ei.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationContext));
        }
    }
}