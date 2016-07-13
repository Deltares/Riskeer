// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Plugin
{
    [TestFixture]
    public class ApplicationCoreTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var applicationCore = new ApplicationCore();

            // Assert
            Assert.IsInstanceOf<IDisposable>(applicationCore);
        }

        [Test]
        public void GetSupportedDataItemInfos_SimpleApplicationPluginWithDataItemInfosAdded_ShouldOnlyProvideSupportedDataItemInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var supportedDataItemInfo = new DataItemInfo
            {
                AdditionalOwnerCheck = o => true
            };
            var unsupportedDataItemInfo = new DataItemInfo
            {
                AdditionalOwnerCheck = o => false // AdditionalOwnerCheck false
            };

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                DataItemInfos = new[]
                {
                    supportedDataItemInfo,
                    unsupportedDataItemInfo,
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedDataItemInfos = applicationCore.GetSupportedDataItemInfos(new object()).ToArray();

            // Assert
            Assert.AreEqual(1, supportedDataItemInfos.Length);
            Assert.AreSame(supportedDataItemInfo, supportedDataItemInfos[0]);
        }

        [Test]
        public void GetSupportedDataItemInfos_SimpleApplicationPluginWithDataItemInfosAdded_ShouldProvideNoDataItemInfosWhenTargetEqualsNull()
        {
            // Setup
            var dataItemInfo = new DataItemInfo
            {
                AdditionalOwnerCheck = o => true
            };

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                DataItemInfos = new[]
                {
                    dataItemInfo
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call / Assert
            CollectionAssert.IsEmpty(applicationCore.GetSupportedDataItemInfos(null));
        }

        private class SimpleApplicationPlugin : ApplicationPlugin
        {
            public IEnumerable<IFileExporter> FileExporters { private get; set; }

            public IEnumerable<DataItemInfo> DataItemInfos { private get; set; }

            public override IEnumerable<DataItemInfo> GetDataItemInfos()
            {
                return DataItemInfos;
            }
        }

        private class SimpleFileImporter<T> : FileImporterBase<T>
        {
            public override string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string Category
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Bitmap Image
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string FileFilter
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override ProgressChangedDelegate ProgressChanged { protected get; set; }

            public override bool Import(object targetItem, string filePath)
            {
                throw new NotImplementedException();
            }
        }

        private class A {}

        private class B : A {}

        private class C : B {}
    }
}