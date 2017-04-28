// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.IO.Exporters;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextConfigurationExportInfoTest
    {
        private ExportInfo exportInfo;

        [SetUp]
        public void Setup()
        {
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                exportInfo = plugin.GetExportInfos()
                                   .Single(ei => ei.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)
                                                 && ei.Name.Equals("Ringtoets berekeningenconfiguratie (*.xml)"));
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

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(new StabilityStoneCoverWaveConditionsCalculation(),
                                                                                  failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = exportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverCalculationConfigurationExporter>(fileExporter);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = exportInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
        }

        [Test]
        public void IsEnabled_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(
                new StabilityStoneCoverWaveConditionsCalculation(),
                failureMechanism,
                assessmentSection);

            // Call
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }
    }
}