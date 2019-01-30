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
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Test.ImportInfos
{
    [TestFixture]
    public class RiskeerImportInfoFactoryTest
    {
        [Test]
        public void CreateCalculationConfigurationImportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            mocks.ReplayAll();

            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, string, IFileImporter> createFileImporter = (context, s) => fileImporter;

            // Call
            ImportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> importInfo = RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo(createFileImporter);

            // Assert
            Assert.AreSame(createFileImporter, importInfo.CreateFileImporter);
            Assert.AreEqual("Riskeer berekeningenconfiguratie", importInfo.Name);
            Assert.AreEqual("Algemeen", importInfo.Category);

            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;
            Assert.AreEqual("Riskeer berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, importInfo.Image);
            Assert.IsTrue(importInfo.IsEnabled(null));

            mocks.VerifyAll();
        }
    }
}