﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Util;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;

namespace Riskeer.Piping.Forms.Test.PropertyClasses.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingOutputPropertiesTest
    {
        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;

        private const string resultCategoryName = "\tResultaat";

        [Test]
        public void Constructor_OutputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new TestProbabilisticPipingOutputProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();

            // Call
            var properties = new TestProbabilisticPipingOutputProperties(output);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IPartialProbabilisticPipingOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null);

            // Call
            var properties = new TestProbabilisticPipingOutputProperties(output);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het faalmechanisme optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();

            // Call
            var properties = new TestProbabilisticPipingOutputProperties(output);

            // Assert
            Assert.AreEqual(ProbabilityFormattingHelper.Format(StatisticsConverter.ReliabilityToProbability(output.Reliability)), properties.Probability);
            Assert.AreEqual(output.Reliability, properties.Reliability, properties.Reliability.GetAccuracy());
        }

        private class TestProbabilisticPipingOutputProperties : ProbabilisticPipingOutputProperties
        {
            public TestProbabilisticPipingOutputProperties(IPartialProbabilisticPipingOutput output)
                : base(output) {}
        }
    }
}