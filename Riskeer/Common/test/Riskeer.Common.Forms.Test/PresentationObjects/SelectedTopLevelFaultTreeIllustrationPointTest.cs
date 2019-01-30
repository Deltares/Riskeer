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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class SelectedTopLevelFaultTreeIllustrationPointTest
    {
        [Test]
        public void Constructor_TopLevelFaultTreeIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SelectedTopLevelFaultTreeIllustrationPoint(null,
                                                                                     Enumerable.Empty<string>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLevelFaultTreeIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SelectedTopLevelFaultTreeIllustrationPoint(CreateTopLevelFaultTreeIllustrationPoint(),
                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituations", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint = CreateTopLevelFaultTreeIllustrationPoint();
            IEnumerable<string> closingSituations = Enumerable.Empty<string>();

            // Call
            var selectedTopLevelFaultTreeIllustrationPoint = new SelectedTopLevelFaultTreeIllustrationPoint(topLevelFaultTreeIllustrationPoint,
                                                                                                            closingSituations);

            // Assert
            Assert.AreSame(topLevelFaultTreeIllustrationPoint, selectedTopLevelFaultTreeIllustrationPoint.TopLevelFaultTreeIllustrationPoint);
            Assert.AreSame(closingSituations, selectedTopLevelFaultTreeIllustrationPoint.ClosingSituations);
        }

        private static TopLevelFaultTreeIllustrationPoint CreateTopLevelFaultTreeIllustrationPoint()
        {
            return new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Closing situation",
                                                          new IllustrationPointNode(new TestIllustrationPoint()));
        }
    }
}