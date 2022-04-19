﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.IO.Configurations;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverCalculationGroupContextConfigurationExportInfoTest
    {
        private WaveImpactAsphaltCoverPlugin plugin;
        private ExportInfo info;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.Replay(gui);
            mocks.Replay(mainWindow);

            plugin = new WaveImpactAsphaltCoverPlugin
            {
                Gui = gui
            };

            info = plugin.GetExportInfos()
                         .Single(ei => ei.DataType == typeof(WaveImpactAsphaltCoverCalculationGroupContext)
                                       && ei.Name(null).Equals("Riskeer berekeningenconfiguratie"));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual("xml", info.Extension);
            Assert.IsNotNull(info.CreateFileExporter);
            Assert.IsNotNull(info.IsEnabled);
            Assert.AreEqual("Algemeen", info.Category);
            TestHelper.AssertImagesAreEqual(CoreGuiResources.ExportIcon, info.Image);
            Assert.IsNotNull(info.GetExportPath);
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(new FailureMechanismContribution(0.1, 0.1));
            mocks.ReplayAll();

            var context = new WaveImpactAsphaltCoverCalculationGroupContext(new CalculationGroup(),
                                                                            null,
                                                                            new WaveImpactAsphaltCoverFailureMechanism(),
                                                                            assessmentSection);

            // Call
            IFileExporter fileExporter = info.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationExporter>(fileExporter);
        }

        [Test]
        public void IsEnabled_CalculationGroupNoChildren_ReturnFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new WaveImpactAsphaltCoverCalculationGroupContext(new CalculationGroup(),
                                                                            null,
                                                                            new WaveImpactAsphaltCoverFailureMechanism(),
                                                                            assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void IsEnabled_CalculationGroupWithChildren_ReturnTrue(bool hasNestedGroup, bool hasCalculation)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            if (hasNestedGroup)
            {
                calculationGroup.Children.Add(new CalculationGroup());
            }

            if (hasCalculation)
            {
                calculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation());
            }

            var context = new WaveImpactAsphaltCoverCalculationGroupContext(calculationGroup,
                                                                            null,
                                                                            new WaveImpactAsphaltCoverFailureMechanism(),
                                                                            assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}