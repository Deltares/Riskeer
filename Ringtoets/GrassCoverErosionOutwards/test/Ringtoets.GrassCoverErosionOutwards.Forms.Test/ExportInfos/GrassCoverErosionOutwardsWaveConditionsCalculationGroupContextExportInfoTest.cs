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
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.ExportInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextExportInfoTest
    {
        [Test]
        public void CreateFileExporter_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
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
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                string fileFilter = exportInfo.FileFilter;

                // Assert
                Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilter);
            }
        }

        [Test]
        public void IsEnabled_NoGrassCoverErosionOutwardsWaveConditionsCalculation_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
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
        public void IsEnabled_GrassCoverErosionOutwardsWaveConditionsCalculationHasOutputFalse_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
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
        public void IsEnabled_GrassCoverErosionOutwardsWaveConditionsCalculationHasOutputTrue_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var output = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
                new WaveConditionsOutput(8, 2, 6, 1)
            };

            calculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(output)
            });

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsEnabled_GrassCoverErosionOutwardsWaveConditionsCalculationInSubFolder_ReturnsTrueIfHasOutput(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var output = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
                new WaveConditionsOutput(8, 2, 6, 1)
            };

            GrassCoverErosionOutwardsWaveConditionsOutput grassCoverErosionOutwardsWaveConditionsOutput = null;
            if (hasOutput)
            {
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
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo exportInfo = GetExportInfo(plugin);

                // Call
                bool isEnabled = exportInfo.IsEnabled(context);

                // Assert
                Assert.AreEqual(hasOutput, isEnabled);
            }
            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext));
        }
    }
}