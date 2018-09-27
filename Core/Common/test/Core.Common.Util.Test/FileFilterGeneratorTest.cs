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

using System;
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Util.Test
{
    [TestFixture]
    public class FileFilterGeneratorTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var generator = new FileFilterGenerator();

            // Assert
            Assert.AreEqual("*", generator.Extension);
            Assert.AreEqual("Alle bestanden", generator.Description);
            Assert.AreEqual("Alle bestanden (*.*)|*.*", generator.Filter);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WithoutExtension_ThrowArgumentNullException(string extension)
        {
            // Call
            TestDelegate test = () => new FileFilterGenerator(extension);

            // Assert
            const string expectedMessage = "Value required for the 'typeExtension'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test,
                expectedMessage);
            Assert.AreEqual("typeExtension", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WithoutExtensionWithDescription_ThrowArgumentException(string extension)
        {
            // Call
            TestDelegate test = () => new FileFilterGenerator(extension, "description");

            // Assert
            const string expectedMessage = "Value required for the 'typeExtension'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test,
                expectedMessage);
            Assert.AreEqual("typeExtension", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WithExtensionWithoutDescription_ThrowArgumentException(string description)
        {
            // Call
            TestDelegate test = () => new FileFilterGenerator("txt", description);

            // Assert
            const string expectedMessage = "Value required for the 'typeDescription'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test,
                expectedMessage);
            Assert.AreEqual("typeDescription", exception.ParamName);
        }

        [Test]
        [TestCase("some filter")]
        [TestCase("txt")]
        public void Constructor_WithExtension_ExpectedValues(string extension)
        {
            // Call
            var generator = new FileFilterGenerator(extension);

            // Assert
            Assert.AreEqual(extension, generator.Extension);
            string description = $"{extension.ToUpperInvariant()}-bestanden";
            Assert.AreEqual(description, generator.Description);
            Assert.AreEqual($"{description} (*.{extension})|*.{extension}", generator.Filter);
        }

        [Test]
        [TestCase("some filter", "some description")]
        [TestCase("txt", "text file")]
        public void Constructor_WithExtensionWithDescription_ExpectedValues(string extension, string description)
        {
            // Call
            var generator = new FileFilterGenerator(extension, description);

            // Assert
            Assert.AreEqual(extension, generator.Extension);
            Assert.AreEqual(description, generator.Description);
            Assert.AreEqual($"{description} (*.{extension})|*.{extension}", generator.Filter);
        }

        [TestFixture]
        private class FileFilterGeneratorEqualsTest : EqualsTestFixture<FileFilterGenerator, DerivedFileFilterGenerator>
        {
            protected override FileFilterGenerator CreateObject()
            {
                return CreateFileFilterGenerator();
            }

            protected override DerivedFileFilterGenerator CreateDerivedObject()
            {
                return new DerivedFileFilterGenerator(CreateFileFilterGenerator());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                FileFilterGenerator baseGenerator = CreateFileFilterGenerator();

                yield return new TestCaseData(new FileFilterGenerator("ext", baseGenerator.Description))
                    .SetName("Extension");
                yield return new TestCaseData(new FileFilterGenerator(baseGenerator.Extension, "Different Description"))
                    .SetName("Description");
            }

            private static FileFilterGenerator CreateFileFilterGenerator()
            {
                return new FileFilterGenerator("txt", "description");
            }
        }

        private class DerivedFileFilterGenerator : FileFilterGenerator
        {
            public DerivedFileFilterGenerator(FileFilterGenerator generator)
                : base(generator.Extension, generator.Description) {}
        }
    }
}