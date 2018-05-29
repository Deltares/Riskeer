﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.ComponentModel;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Revetment.IO.Configurations.Helpers;

namespace Ringtoets.Revetment.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationAssessmentSectionCategoryTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertFrom_AssessmentSectionCategoryType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(AssessmentSectionCategoryType));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }
    }
}