﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class ExportInfoTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var info = new ExportInfo();

            // Assert
            Assert.IsNull(info.DataType);
            Assert.IsNull(info.CreateFileExporter);
            Assert.IsNullOrEmpty(info.Name);
            Assert.IsNullOrEmpty(info.Category);
            Assert.IsNull(info.Image);
            Assert.IsNullOrEmpty(info.FileFilter);
        }

        [Test]
        public void DefaultGenericConstructor_ExpectedValues()
        {
            // Call
            var info = new ExportInfo<int>();

            // Assert
            Assert.AreEqual(typeof(int), info.DataType);
            Assert.IsNull(info.CreateFileExporter);
            Assert.IsNullOrEmpty(info.Name);
            Assert.IsNullOrEmpty(info.Category);
            Assert.IsNull(info.Image);
            Assert.IsNullOrEmpty(info.FileFilter);
        }

        [Test]
        public void ImplicitOperator_OptionalDelegatesAndPropertiesSet_ExportInfoFullyConverted()
        {
            // Setup
            var mocks = new MockRepository();
            var fileExporter = mocks.StrictMock<IFileExporter>();
            mocks.ReplayAll();

            const string name = "name";
            const string category = "category";
            Bitmap image = new Bitmap(16, 16);
            const string fileFilter = "fileFilter";

            var info = new ExportInfo<int>
            {
                CreateFileExporter = (data, filePath) => fileExporter,
                Name = name,
                Category = category,
                Image = image,
                FileFilter = fileFilter
            };

            // Precondition
            Assert.IsInstanceOf<ExportInfo<int>>(info);

            // Call
            ExportInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<ExportInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.IsNotNull(convertedInfo.CreateFileExporter);
            Assert.AreSame(fileExporter, convertedInfo.CreateFileExporter(12, string.Empty));
            Assert.AreEqual(name, info.Name);
            Assert.AreEqual(category, info.Category);
            Assert.AreSame(image, info.Image);
            Assert.AreEqual(fileFilter, info.FileFilter);

            mocks.VerifyAll();
        }

        [Test]
        public void ImplicitOperator_NoneOfTheOptionalDelegatesAndPropertiesSet_ExportInfoFullyConverted()
        {
            // Setup
            var info = new ExportInfo<int>();

            // Precondition
            Assert.IsInstanceOf<ExportInfo<int>>(info);

            // Call
            ExportInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<ExportInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.IsNotNull(convertedInfo.CreateFileExporter);
            Assert.IsNull(convertedInfo.CreateFileExporter(new object(), string.Empty));
            Assert.IsNullOrEmpty(info.Name);
            Assert.IsNullOrEmpty(info.Category);
            Assert.IsNull(info.Image);
            Assert.IsNullOrEmpty(info.FileFilter);
        }
    }
}