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

using System;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.ExportInfos
{
    [TestFixture]
    public class RingtoetsExportInfoFactoryTest
    {
        [Test]
        public void CreateCalculationConfigurationExportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileExporter>();
            mocks.ReplayAll();

            Func<ICalculationContext<ICalculation, IFailureMechanism>, string, IFileExporter> createFileExporter = (context, s) => fileImporter;

            // Call
            ExportInfo<ICalculationContext<ICalculation, IFailureMechanism>> exportInfo =
                RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo(createFileExporter);

            // Assert
            Assert.AreSame(createFileExporter, exportInfo.CreateFileExporter);
            Assert.AreEqual("Ringtoets berekeningenconfiguratie", exportInfo.Name);
            Assert.AreEqual("Algemeen", exportInfo.Category);

            FileFilterGenerator fileFilterGenerator = exportInfo.FileFilterGenerator;
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, exportInfo.Image);
            Assert.IsTrue(exportInfo.IsEnabled(null));

            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationGroupConfigurationExportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileExporter>();
            mocks.ReplayAll();

            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, bool> isEnabled = context => false;
            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, string, IFileExporter> createFileExporter = (context, s) => fileImporter;

            // Call
            ExportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> exportInfo =
                RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo(createFileExporter, isEnabled);

            // Assert
            Assert.AreSame(isEnabled, exportInfo.IsEnabled);
            Assert.AreSame(createFileExporter, exportInfo.CreateFileExporter);
            Assert.AreEqual("Ringtoets berekeningenconfiguratie", exportInfo.Name);
            Assert.AreEqual("Algemeen", exportInfo.Category);

            FileFilterGenerator fileFilterGenerator = exportInfo.FileFilterGenerator;
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, exportInfo.Image);
        }
    }
}