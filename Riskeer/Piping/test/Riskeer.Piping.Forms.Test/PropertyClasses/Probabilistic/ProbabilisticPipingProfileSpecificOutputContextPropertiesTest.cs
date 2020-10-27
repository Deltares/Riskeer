// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;

namespace Riskeer.Piping.Forms.Test.PropertyClasses.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingProfileSpecificOutputContextPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingProfileSpecificOutputContextProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double reliability = random.NextDouble();
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            var partialProbabilisticPipingOutput = new PartialProbabilisticPipingOutput(reliability, generalResult);

            // Call
            var properties = new ProbabilisticPipingProfileSpecificOutputContextProperties(partialProbabilisticPipingOutput);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PartialProbabilisticPipingOutput>>(properties);
            Assert.AreSame(properties, properties.Data);
        }
    }
}