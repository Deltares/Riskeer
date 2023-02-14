﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryDataExtensionsTest
    {
        [Test]
        public void IsLinked_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataExtensions.IsLinked(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", paramName);
        }

        [Test]
        public void EffectivePreprocessorDirectory_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataExtensions.EffectivePreprocessorDirectory(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", paramName);
        }

        [Test]
        public void EffectivePreprocessorDirectory_CanUsePreprocessorFalse_ReturnsExpectedValue()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            string effectivePreprocessorDirectory = hydraulicBoundaryData.EffectivePreprocessorDirectory();

            // Assert
            Assert.AreEqual("", effectivePreprocessorDirectory);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("Test", true)]
        public void IsLinked_SpecificFilePath_ReturnsExpectedValue(string filePath, bool expectedValue)
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = filePath
            };

            // Call
            bool isLinked = hydraulicBoundaryData.IsLinked();

            // Assert
            Assert.AreEqual(expectedValue, isLinked);
        }

        [TestCase(false, "Test", "")]
        [TestCase(true, "Test", "Test")]
        public void EffectivePreprocessorDirectory_CanUsePreprocessorTrue_ReturnsExpectedValue(bool usePreprocessor,
                                                                                               string preprocessorDirectory,
                                                                                               string expectedEffectivePreprocessorDirectory)
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = usePreprocessor,
                    PreprocessorDirectory = preprocessorDirectory
                }
            };

            // Call
            string effectivePreprocessorDirectory = hydraulicBoundaryData.EffectivePreprocessorDirectory();

            // Assert
            Assert.AreEqual(expectedEffectivePreprocessorDirectory, effectivePreprocessorDirectory);
        }
    }
}