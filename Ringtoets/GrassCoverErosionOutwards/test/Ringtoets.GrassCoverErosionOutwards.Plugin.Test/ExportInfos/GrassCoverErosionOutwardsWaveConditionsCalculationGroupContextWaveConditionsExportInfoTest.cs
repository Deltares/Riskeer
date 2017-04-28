﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.Revetment.IO.Exporters;
using Ringtoets.Revetment.TestUtil;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextWaveConditionsExportInfoTest
    {
        private ExportInfo exportInfo;

        [SetUp]
        public void Setup()
        {
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                exportInfo = plugin.GetExportInfos()
                                   .Single(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)
                                                 && ei.Name.Equals("Berekende belastingen bij verschillende waterstanden (*.csv)."));
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);

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
        public void IsEnabled_NoCalculation_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, failureMechanism, assessmentSection);

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
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.AreEqual(hasOutput, isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, TestName = "IsEnabled_WithOutputInSubFolder_ReturnExpectedEnabledState")]
        [TestCase(false, TestName = "IsEnabled_WithoutOutputInSubFolder_ReturnExpectedEnabledState")]
        public void IsEnabled_CalculationWithOrWithoutOutputInSubFolder_ReturnExpectedEnabledState(bool hasOutput)
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
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.AreEqual(hasOutput, isEnabled);
            mocks.VerifyAll();
        }
    }
}