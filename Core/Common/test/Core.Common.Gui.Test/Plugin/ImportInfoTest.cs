// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Drawing;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class ImportInfoTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var info = new ImportInfo();

            // Assert
            Assert.IsNull(info.DataType);
            Assert.IsNull(info.CreateFileImporter);
            Assert.IsNull(info.IsEnabled);
            Assert.IsNull(info.Name);
            Assert.AreEqual("Importeren gegevens", info.Category);
            TestHelper.AssertImagesAreEqual(Resources.brick, info.Image);
            Assert.IsNull(info.FileFilterGenerator);
            Assert.IsNull(info.VerifyUpdates);
        }

        [Test]
        public void DefaultGenericConstructor_ExpectedValues()
        {
            // Call
            var info = new ImportInfo<int>();

            // Assert
            Assert.AreEqual(typeof(int), info.DataType);
            Assert.IsNull(info.CreateFileImporter);
            Assert.IsNull(info.IsEnabled);
            Assert.IsNull(info.Name);
            Assert.AreEqual("Importeren gegevens", info.Category);
            TestHelper.AssertImagesAreEqual(Resources.brick, info.Image);
            Assert.IsNull(info.FileFilterGenerator);
            Assert.IsNull(info.VerifyUpdates);
        }

        [Test]
        public void ImplicitOperator_OptionalDelegatesAndPropertiesSet_ImportInfoFullyConverted()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.StrictMock<IFileImporter>();
            mocks.ReplayAll();

            const string name = "name";
            const string category = "category";
            var image = new Bitmap(16, 16);
            var generator = new FileFilterGenerator();

            var info = new ImportInfo<int>
            {
                CreateFileImporter = (data, filePath) => fileImporter,
                IsEnabled = data => false,
                Name = name,
                Category = category,
                Image = image,
                FileFilterGenerator = generator,
                VerifyUpdates = i => true
            };

            // Precondition
            Assert.IsInstanceOf<ImportInfo<int>>(info);

            // Call
            ImportInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<ImportInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.IsNotNull(convertedInfo.CreateFileImporter);
            Assert.AreSame(fileImporter, convertedInfo.CreateFileImporter(12, ""));
            Assert.IsNotNull(convertedInfo.IsEnabled);
            Assert.IsFalse(convertedInfo.IsEnabled(12));
            Assert.AreEqual(name, info.Name);
            Assert.AreEqual(category, info.Category);
            Assert.AreSame(image, info.Image);
            Assert.AreEqual(generator, info.FileFilterGenerator);
            Assert.IsNotNull(info.VerifyUpdates);
            Assert.IsTrue(convertedInfo.VerifyUpdates(12));

            mocks.VerifyAll();
        }

        [Test]
        public void ImplicitOperator_NoneOfTheOptionalDelegatesAndPropertiesSet_ImportInfoFullyConverted()
        {
            // Setup
            var info = new ImportInfo<int>();

            // Precondition
            Assert.IsInstanceOf<ImportInfo<int>>(info);

            // Call
            ImportInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<ImportInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.IsNotNull(convertedInfo.CreateFileImporter);
            Assert.IsNull(convertedInfo.CreateFileImporter(12, ""));
            Assert.IsNotNull(convertedInfo.IsEnabled);
            Assert.IsTrue(convertedInfo.IsEnabled(12));
            Assert.IsNull(info.Name);
            Assert.AreEqual("Importeren gegevens", info.Category);
            TestHelper.AssertImagesAreEqual(Resources.brick, info.Image);
            Assert.IsNull(info.FileFilterGenerator);
            Assert.IsNull(info.VerifyUpdates);
        }
    }
}