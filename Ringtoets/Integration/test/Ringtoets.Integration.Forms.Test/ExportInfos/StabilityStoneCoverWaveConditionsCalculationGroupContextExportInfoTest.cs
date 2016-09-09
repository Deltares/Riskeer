﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Plugin;

namespace Ringtoets.Integration.Forms.Test.ExportInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationGroupContextExportInfoTest
    {
        [Test]
        public void CreateFileExporter_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (StabilityStoneCoverPlugin plugin = new StabilityStoneCoverPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<WaveConditionsExporterBase>(fileExporter);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnsFileFilter()
        {
            // Setup
            using (StabilityStoneCoverPlugin plugin = new StabilityStoneCoverPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                string fileFilter = info.FileFilter;

                // Assert
                Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilter);
            }
        }

        [Test]
        public void IsEnabled_NoStabilityStoneCoverWaveConditionsCalculation_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (StabilityStoneCoverPlugin plugin = new StabilityStoneCoverPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_StabilityStoneCoverWaveConditionsCalculationHasOutputFalse_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation());

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (StabilityStoneCoverPlugin plugin = new StabilityStoneCoverPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_StabilityStoneCoverWaveConditionsCalculationHasOutputTrue_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var columnsOutput = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
                new WaveConditionsOutput(8, 2, 6, 1)
            };

            var blocksOutput = new[]
            {
                new WaveConditionsOutput(6, 2, 9, 4),
                new WaveConditionsOutput(4, 1, 7, 3)
            };
            calculationGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput)
            });

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (StabilityStoneCoverPlugin plugin = new StabilityStoneCoverPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(StabilityStoneCoverPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext));
        }
    }
}