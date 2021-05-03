﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.Forms.MainWindow;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Revetment.IO.WaveConditions;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Data.TestUtil;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextWaveConditionsExportInfoTest
    {
        private StabilityStoneCoverPlugin plugin;
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

            plugin = new StabilityStoneCoverPlugin
            {
                Gui = gui
            };

            info = plugin.GetExportInfos()
                         .Single(ei => ei.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)
                                       && ei.Name.Equals("Berekende belastingen bij verschillende waterstanden"));
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
            Assert.AreEqual("csv", info.Extension);
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
            mocks.ReplayAll();

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(new StabilityStoneCoverWaveConditionsCalculation(),
                                                                                  new CalculationGroup(),
                                                                                  new StabilityStoneCoverFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            IFileExporter fileExporter = info.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(fileExporter);
        }

        [Test]
        public void IsEnabled_CalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(new StabilityStoneCoverWaveConditionsCalculation(),
                                                                                  new CalculationGroup(),
                                                                                  new StabilityStoneCoverFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void IsEnabled_CalculationWithOutput_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(
                new StabilityStoneCoverWaveConditionsCalculation
                {
                    Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
                },
                new CalculationGroup(),
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}