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

using System;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Test.ImportInfos
{
    [TestFixture]
    public class RingtoetsImportInfoFactoryTest
    {
        [Test]
        public void CreateCalculationConfigurationImportInfo_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            mocks.ReplayAll();

            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, bool> isEnabled = context => false;
            Func<ICalculationContext<CalculationGroup, IFailureMechanism>, string, IFileImporter> createFileImporter = (context, s) => fileImporter;

            // Call
            ImportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> importInfo =
                RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo(isEnabled, createFileImporter);

            // Assert
            Assert.AreSame(isEnabled, importInfo.IsEnabled);
            Assert.AreSame(createFileImporter, importInfo.CreateFileImporter);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationConfigurationImportInfo_Always_ReturnsExpectedName()
        {
            // Call
            ImportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> importInfo =
                RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo
                    <ICalculationContext<CalculationGroup, IFailureMechanism>>(null, null);

            // Assert
            Assert.AreEqual("Ringtoets berekeningenconfiguratie", importInfo.Name);
        }

        [Test]
        public void CreateCalculationConfigurationImportInfo_Always_ReturnsExpectedCategory()
        {
            // Call
            ImportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> importInfo =
                RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo
                    <ICalculationContext<CalculationGroup, IFailureMechanism>>(null, null);

            // Assert
            Assert.AreEqual("Algemeen", importInfo.Category);
        }

        [Test]
        public void CreateCalculationConfigurationImportInfo_Always_ReturnsExpectedFileFilter()
        {
            // Call
            ImportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> importInfo =
                RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo
                    <ICalculationContext<CalculationGroup, IFailureMechanism>>(null, null);

            // Assert
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
        }

        [Test]
        public void CreateCalculationConfigurationImportInfo_Always_ReturnsExpectedIcon()
        {
            // Call
            ImportInfo<ICalculationContext<CalculationGroup, IFailureMechanism>> importInfo =
                RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo
                    <ICalculationContext<CalculationGroup, IFailureMechanism>>(null, null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, importInfo.Image);
        }
    }
}