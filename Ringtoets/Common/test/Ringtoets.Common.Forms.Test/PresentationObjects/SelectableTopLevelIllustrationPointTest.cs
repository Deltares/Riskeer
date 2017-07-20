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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class SelectableTopLevelIllustrationPointTest
    {
        [Test]
        public void Constructor_TopLevelIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SelectableTopLevelIllustrationPoint(null,
                                                                              Enumerable.Empty<string>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLevelIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SelectableTopLevelIllustrationPoint(new TestTopLevelIllustrationPoint(),
                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituations", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            var topLevelIllustrationPoint = new TestTopLevelIllustrationPoint();
            IEnumerable<string> closingSituations = Enumerable.Empty<string>();

            // Call
            var illustrationPoint = new SelectableTopLevelIllustrationPoint(topLevelIllustrationPoint,
                                                                            closingSituations);

            // Assert
            Assert.AreSame(topLevelIllustrationPoint, illustrationPoint.TopLevelIllustrationPoint);
            Assert.AreSame(closingSituations, illustrationPoint.ClosingSituations);
        }
    }
}