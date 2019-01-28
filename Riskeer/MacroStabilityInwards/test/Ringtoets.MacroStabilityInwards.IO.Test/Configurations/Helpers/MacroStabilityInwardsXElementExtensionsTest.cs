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
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class MacroStabilityInwardsXElementExtensionsTest
    {
        private static IEnumerable<TestCaseData> GetInvalidDoubleValues()
        {
            yield return new TestCaseData(string.Format(CultureInfo.InvariantCulture, "{0}9", double.MaxValue),
                                          typeof(OverflowException))
                .SetName("Larger than double.MaxValue");
            yield return new TestCaseData(string.Format(CultureInfo.InvariantCulture, "{0}9", double.MinValue),
                                          typeof(OverflowException))
                .SetName("Smaller than double.MinValue");
            yield return new TestCaseData("Tweeenveertig",
                                          typeof(FormatException))
                .SetName("Invalid format");
        }

        private static IEnumerable<TestCaseData> GetInvalidBoolValues()
        {
            yield return new TestCaseData("0.05",
                                          typeof(FormatException))
                .SetName("double format");
            yield return new TestCaseData("nope",
                                          typeof(FormatException))
                .SetName("String format");
        }

        private static IEnumerable<TestCaseData> GetInvalidIntegerValues()
        {
            yield return new TestCaseData(string.Format(CultureInfo.InvariantCulture, "{0}9", int.MaxValue),
                                          typeof(OverflowException))
                .SetName("Larger than int.MaxValue");
            yield return new TestCaseData(string.Format(CultureInfo.InvariantCulture, "{0}9", int.MinValue),
                                          typeof(OverflowException))
                .SetName("Smaller than int.MinValue");
            yield return new TestCaseData("Tweeenveertig",
                                          typeof(FormatException))
                .SetName("Invalid format");
        }

        #region GetMacroStabilityInwardsLocationInputConfiguration

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_XElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((XElement) null).GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationElement", exception.ParamName);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_DescendantElementNameNotFound_ReturnNull()
        {
            // Setup
            const string descendantElementName = "differentElementName";

            var element = new XElement("Root", new XElement(descendantElementName));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidDoubleValues))]
        public void GetMacroStabilityInwardsLocationInputConfiguration_DescendantElementNoValidDouble_ThrowException(string doubleValue, Type exceptionType)
        {
            var element = new XElement("Root", new XElement("dagelijks", new XElement("polderpeil", doubleValue)));

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.Throws(exceptionType, test);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidBoolValues))]
        public void GetMacroStabilityInwardsLocationInputConfiguration_DescendantElementNoValidBool_ThrowException(string boolValue, Type exceptionType)
        {
            var element = new XElement("Root", new XElement("dagelijks", new XElement("gebruikdefaults", boolValue)));

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.Throws(exceptionType, test);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithoutProperties_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var element = new XElement("Root", new XElement("dagelijks"));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithWaterLevelPolder_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            double waterLevelPolder = random.NextDouble();

            var element = new XElement("Root", new XElement("dagelijks", new XElement("polderpeil", waterLevelPolder)));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.AreEqual(waterLevelPolder, configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithUseDefaultOffsets_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            bool useDefaultOffsets = random.NextBoolean();

            var element = new XElement("Root", new XElement("dagelijks", new XElement("gebruikdefaults", useDefaultOffsets)));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithPhreaticLineOffsetBelowDikeTopAtRiver_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();

            var element = new XElement("Root", new XElement("dagelijks", new XElement("buitenkruin", phreaticLineOffsetBelowDikeTopAtRiver)));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithPhreaticLineOffsetBelowDikeTopAtPolder_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();

            var element = new XElement("Root", new XElement("dagelijks", new XElement("binnenkruin", phreaticLineOffsetBelowDikeTopAtPolder)));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithPhreaticLineOffsetBelowShoulderBaseInside_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();

            var element = new XElement("Root", new XElement("dagelijks", new XElement("insteekbinnenberm", phreaticLineOffsetBelowShoulderBaseInside)));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_WithPhreaticLineOffsetBelowDikeToeAtPolder_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

            var element = new XElement("Root", new XElement("dagelijks", new XElement("teendijkbinnenwaarts", phreaticLineOffsetBelowDikeToeAtPolder)));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputConfiguration_ValidDescendantElement_ReturnMacroStabilityInwardsLocationInputConfiguration()
        {
            // Setup
            var random = new Random(31);
            double waterLevelPolder = random.NextDouble();
            bool useDefaultOffsets = random.NextBoolean();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

            var waterLevelPolderElement = new XElement("polderpeil", waterLevelPolder);
            var useDefaultOffsetsElement = new XElement("gebruikdefaults", useDefaultOffsets);
            var phreaticLineOffsetBelowDikeTopAtRiverElement = new XElement("buitenkruin", phreaticLineOffsetBelowDikeTopAtRiver);
            var phreaticLineOffsetBelowDikeTopAtPolderElement = new XElement("binnenkruin", phreaticLineOffsetBelowDikeTopAtPolder);
            var phreaticLineOffsetBelowShoulderBaseInsideElement = new XElement("insteekbinnenberm", phreaticLineOffsetBelowShoulderBaseInside);
            var phreaticLineOffsetBelowDikeToeAtPolderElement = new XElement("teendijkbinnenwaarts", phreaticLineOffsetBelowDikeToeAtPolder);

            var element = new XElement("Root", new XElement("dagelijks",
                                                            waterLevelPolderElement,
                                                            useDefaultOffsetsElement,
                                                            phreaticLineOffsetBelowDikeTopAtRiverElement,
                                                            phreaticLineOffsetBelowDikeTopAtPolderElement,
                                                            phreaticLineOffsetBelowShoulderBaseInsideElement,
                                                            phreaticLineOffsetBelowDikeToeAtPolderElement));

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = element.GetMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.AreEqual(waterLevelPolder, configuration.WaterLevelPolder);
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        #endregion

        #region GetMacroStabilityInwardsLocationInputExtremeConfiguration

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_XElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((XElement) null).GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationElement", exception.ParamName);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_DescendantElementNameNotFound_ReturnNull()
        {
            // Setup
            const string descendantElementName = "differentElementName";

            var element = new XElement("Root", new XElement(descendantElementName));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidDoubleValues))]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_DescendantElementNoValidDouble_ThrowException(string doubleValue, Type exceptionType)
        {
            var element = new XElement("Root", new XElement("extreem", new XElement("polderpeil", doubleValue)));

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.Throws(exceptionType, test);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidBoolValues))]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_DescendantElementNoValidBool_ThrowException(string boolValue, Type exceptionType)
        {
            var element = new XElement("Root", new XElement("extreem", new XElement("gebruikdefaults", boolValue)));

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.Throws(exceptionType, test);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithoutProperties_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var element = new XElement("Root", new XElement("extreem"));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithPenetrationLength_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double penetrationLength = random.NextDouble();

            var element = new XElement("Root", new XElement("extreem", new XElement("indringingslengte", penetrationLength)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.AreEqual(penetrationLength, configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithWaterLevelPolder_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double waterLevelPolder = random.NextDouble();

            var element = new XElement("Root", new XElement("extreem", new XElement("polderpeil", waterLevelPolder)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.AreEqual(waterLevelPolder, configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithUseDefaultOffsets_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            bool useDefaultOffsets = random.NextBoolean();

            var element = new XElement("Root", new XElement("extreem", new XElement("gebruikdefaults", useDefaultOffsets)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithPhreaticLineOffsetBelowDikeTopAtRiver_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();

            var element = new XElement("Root", new XElement("extreem", new XElement("buitenkruin", phreaticLineOffsetBelowDikeTopAtRiver)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithPhreaticLineOffsetBelowDikeTopAtPolder_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();

            var element = new XElement("Root", new XElement("extreem", new XElement("binnenkruin", phreaticLineOffsetBelowDikeTopAtPolder)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithPhreaticLineOffsetBelowShoulderBaseInside_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();

            var element = new XElement("Root", new XElement("extreem", new XElement("insteekbinnenberm", phreaticLineOffsetBelowShoulderBaseInside)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_WithPhreaticLineOffsetBelowDikeToeAtPolder_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

            var element = new XElement("Root", new XElement("extreem", new XElement("teendijkbinnenwaarts", phreaticLineOffsetBelowDikeToeAtPolder)));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.IsNull(configuration.PenetrationLength);
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void GetMacroStabilityInwardsLocationInputExtremeConfiguration_ValidDescendantElement_ReturnMacroStabilityInwardsLocationInputExtremeConfiguration()
        {
            // Setup
            var random = new Random(31);
            double penetrationLength = random.NextDouble();
            double waterLevelPolder = random.NextDouble();
            bool useDefaultOffsets = random.NextBoolean();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

            var penetrationLengthElement = new XElement("indringingslengte", penetrationLength);
            var waterLevelPolderElement = new XElement("polderpeil", waterLevelPolder);
            var useDefaultOffsetsElement = new XElement("gebruikdefaults", useDefaultOffsets);
            var phreaticLineOffsetBelowDikeTopAtRiverElement = new XElement("buitenkruin", phreaticLineOffsetBelowDikeTopAtRiver);
            var phreaticLineOffsetBelowDikeTopAtPolderElement = new XElement("binnenkruin", phreaticLineOffsetBelowDikeTopAtPolder);
            var phreaticLineOffsetBelowShoulderBaseInsideElement = new XElement("insteekbinnenberm", phreaticLineOffsetBelowShoulderBaseInside);
            var phreaticLineOffsetBelowDikeToeAtPolderElement = new XElement("teendijkbinnenwaarts", phreaticLineOffsetBelowDikeToeAtPolder);

            var element = new XElement("Root", new XElement("extreem",
                                                            penetrationLengthElement,
                                                            waterLevelPolderElement,
                                                            useDefaultOffsetsElement,
                                                            phreaticLineOffsetBelowDikeTopAtRiverElement,
                                                            phreaticLineOffsetBelowDikeTopAtPolderElement,
                                                            phreaticLineOffsetBelowShoulderBaseInsideElement,
                                                            phreaticLineOffsetBelowDikeToeAtPolderElement));

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = element.GetMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.AreEqual(penetrationLength, configuration.PenetrationLength);
            Assert.AreEqual(waterLevelPolder, configuration.WaterLevelPolder);
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        #endregion

        #region GetMacroStabilityInwardsGridConfiguration

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_XElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((XElement) null).GetMacroStabilityInwardsGridConfiguration("element");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationElement", exception.ParamName);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsGridConfiguration(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_DescendantElementNameNotFound_ReturnNull()
        {
            // Setup
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration("differentElementName");

            // Assert
            Assert.IsNull(configuration);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidDoubleValues))]
        public void GetMacroStabilityInwardsGridConfiguration_DescendantElementNoValidDouble_ThrowException(string doubleValue, Type exceptionType)
        {
            // Setup
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName, new XElement("xlinks", doubleValue)));

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.Throws(exceptionType, test);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidIntegerValues))]
        public void GetMacroStabilityInwardsGridConfiguration_DescendantElementNoValidInteger_ThrowException(string integerValue, Type exceptionType)
        {
            // Setup
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName, new XElement("aantalpuntenhorizontaal", integerValue)));

            // Call
            TestDelegate test = () => element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.Throws(exceptionType, test);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithoutProperties_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithXLeft_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double xLeft = random.NextDouble();
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            new XElement("xlinks", xLeft)));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.AreEqual(xLeft, configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithXRight_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double xRight = random.NextDouble();
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            new XElement("xrechts", xRight)));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.AreEqual(xRight, configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithZTop_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double zTop = random.NextDouble();
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            new XElement("zboven", zTop)));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.AreEqual(zTop, configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithZBottom_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double zBottom = random.NextDouble();
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            new XElement("zonder", zBottom)));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.AreEqual(zBottom, configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithNumberOfHorizontalPoints_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double numberOfHorizontalPoints = random.Next();
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            new XElement("aantalpuntenhorizontaal", numberOfHorizontalPoints)));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.AreEqual(numberOfHorizontalPoints, configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_WithNumberOfVerticalPoints_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double numberOfVerticalPoints = random.Next();
            const string descendantElementName = "descendantElement";

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            new XElement("aantalpuntenverticaal", numberOfVerticalPoints)));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void GetMacroStabilityInwardsGridConfiguration_ValidDescendantElement_ReturnMacroStabilityInwardsGridConfiguration()
        {
            // Setup
            var random = new Random(31);
            double xLeft = random.NextDouble();
            double xRight = random.NextDouble();
            double zTop = random.NextDouble();
            double zBottom = random.NextDouble();
            double numberOfHorizontalPoints = random.Next();
            double numberOfVerticalPoints = random.Next();
            const string descendantElementName = "descendantElement";

            var xLeftElement = new XElement("xlinks", xLeft);
            var xRightElement = new XElement("xrechts", xRight);
            var zTopElement = new XElement("zboven", zTop);
            var zBottomElement = new XElement("zonder", zBottom);
            var numberOfHorizontalPointsElement = new XElement("aantalpuntenhorizontaal", numberOfHorizontalPoints);
            var numberOfVerticalPointsElement = new XElement("aantalpuntenverticaal", numberOfVerticalPoints);

            var element = new XElement("Root", new XElement(descendantElementName,
                                                            xLeftElement,
                                                            xRightElement,
                                                            zTopElement,
                                                            zBottomElement,
                                                            numberOfHorizontalPointsElement,
                                                            numberOfVerticalPointsElement));

            // Call
            MacroStabilityInwardsGridConfiguration configuration = element.GetMacroStabilityInwardsGridConfiguration(descendantElementName);

            // Assert
            Assert.AreEqual(xLeft, configuration.XLeft);
            Assert.AreEqual(xRight, configuration.XRight);
            Assert.AreEqual(zTop, configuration.ZTop);
            Assert.AreEqual(zBottom, configuration.ZBottom);
            Assert.AreEqual(numberOfHorizontalPoints, configuration.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, configuration.NumberOfVerticalPoints);
        }

        #endregion
    }
}