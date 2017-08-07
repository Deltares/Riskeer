// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Utils.Test
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

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Setup
            var generator = new FileFilterGenerator("txt", "descriptionA");

            // Call
            bool result = generator.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            var generator = new FileFilterGenerator("txt", "descriptionA");

            // Call
            bool result = generator.Equals(new object());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCaseSource(nameof(GeneratorCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(FileFilterGenerator generator,
                                                                    FileFilterGenerator otherGenerator,
                                                                    bool expectedEqual)
        {
            // Call
            bool areEqualOne = generator.Equals(otherGenerator);
            bool areEqualTwo = otherGenerator.Equals(generator);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        [Test]
        public void GetHashCode_FiltersAreEqual_FiltersHashesEqual()
        {
            // Setup
            const string extension = "txt";
            const string description = "text files";

            var generator = new FileFilterGenerator(extension, description);
            var otherGenerator = new FileFilterGenerator(extension, description);

            // Call
            int result = generator.GetHashCode();
            int otherResult = otherGenerator.GetHashCode();

            // Assert
            Assert.AreEqual(result, otherResult);
        }

        private static TestCaseData[] GeneratorCombinations()
        {
            var generatorA = new FileFilterGenerator("txt", "descriptionA");
            var generatorB = new FileFilterGenerator("txt", "descriptionA");
            var generatorC = new FileFilterGenerator("ext", "descriptionA");
            var generatorD = new FileFilterGenerator("txt", "descriptionB");

            return new[]
            {
                new TestCaseData(generatorA, generatorA, true)
                {
                    TestName = "Equals_FileFilterGeneratorAFileFilterGeneratorA_True"
                },
                new TestCaseData(generatorA, generatorB, true)
                {
                    TestName = "Equals_FileFilterGeneratorAFileFilterGeneratorB_True"
                },
                new TestCaseData(generatorB, generatorC, false)
                {
                    TestName = "Equals_FileFilterGeneratorBFileFilterGeneratorC_False"
                },
                new TestCaseData(generatorA, generatorD, false)
                {
                    TestName = "Equals_FileFilterGeneratorAFileFilterGeneratorD_False"
                }
            };
        }
    }
}