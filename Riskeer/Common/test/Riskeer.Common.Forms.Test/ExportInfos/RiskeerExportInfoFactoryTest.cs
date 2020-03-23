// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using Core.Common.Base.IO;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.Common.Forms.Test.ExportInfos
{
    [TestFixture]
    public class RiskeerExportInfoFactoryTest
    {
        [Test]
        public void CreateCalculationConfigurationExportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileExporter>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            Func<ICalculationContext<ICalculation, IFailureMechanism>, string, IFileExporter> createFileExporter = (context, s) => fileImporter;

            // Call
            ExportInfo<ICalculationContext<ICalculation, IFailureMechanism>> exportInfo =
                RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo(createFileExporter, inquiryHelper);

            // Assert
            Assert.AreSame(createFileExporter, exportInfo.CreateFileExporter);
            Assert.AreEqual("Riskeer berekeningenconfiguratie", exportInfo.Name);
            Assert.AreEqual("xml", exportInfo.Extension);
            Assert.AreEqual("Algemeen", exportInfo.Category);

            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, exportInfo.Image);
            Assert.IsTrue(exportInfo.IsEnabled(null));

            Assert.IsNotNull(exportInfo.GetExportPath);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationGroupConfigurationExportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileExporter>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, bool> isEnabled = context => false;
            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, string, IFileExporter> createFileExporter = (context, s) => fileImporter;

            // Call
            ExportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> exportInfo =
                RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo(createFileExporter, isEnabled, inquiryHelper);

            // Assert
            Assert.AreSame(isEnabled, exportInfo.IsEnabled);
            Assert.AreSame(createFileExporter, exportInfo.CreateFileExporter);
            Assert.AreEqual("Riskeer berekeningenconfiguratie", exportInfo.Name);
            Assert.AreEqual("xml", exportInfo.Extension);
            Assert.AreEqual("Algemeen", exportInfo.Category);

            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, exportInfo.Image);

            Assert.IsNotNull(exportInfo.GetExportPath);

            mocks.VerifyAll();
        }
    }
}