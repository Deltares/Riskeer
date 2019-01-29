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
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class IllustrationPointContextTest
    {
        [Test]
        public void Constructor_IllustationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            var node = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());

            // Call
            TestDelegate test = () => new IllustrationPointContext<IllustrationPointBase>(null, node, "", "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_IllustrationPointNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointContext<IllustrationPointBase>(new TestFaultTreeIllustrationPoint(), null, "", "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPointNode", exception.ParamName);
        }

        [Test]
        public void Constructor_WindDirectionNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var node = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());

            // Call
            TestDelegate test = () => new IllustrationPointContext<IllustrationPointBase>(new TestFaultTreeIllustrationPoint(), node, null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("windDirectionName", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Setup
            var node = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());

            // Call
            TestDelegate test = () => new IllustrationPointContext<IllustrationPointBase>(new TestFaultTreeIllustrationPoint(), node, "", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedProperties()
        {
            // Setup
            var node = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            const string windDirectionName = "wdn";
            const string closingSituation = "cs";

            // Call
            var context = new IllustrationPointContext<IllustrationPointBase>((FaultTreeIllustrationPoint) node.Data, node,
                                                                              windDirectionName, closingSituation);

            // Assert
            Assert.AreSame(node.Data, context.IllustrationPoint);
            Assert.AreSame(node, context.IllustrationPointNode);
            Assert.AreEqual(windDirectionName, context.WindDirectionName);
            Assert.AreEqual(closingSituation, context.ClosingSituation);
        }
    }
}