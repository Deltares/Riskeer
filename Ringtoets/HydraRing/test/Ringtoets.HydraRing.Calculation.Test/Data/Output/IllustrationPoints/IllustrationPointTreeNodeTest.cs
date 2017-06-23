// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Rhino.Mocks;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointTreeNodeTest
    {
        [Test]
        public void Constructor_WithoutData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointTreeNode(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_WithData_DataIsAssigned()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IIllustrationPoint>();
            mocks.ReplayAll();

            // Call
            var node = new IllustrationPointTreeNode(data);

            // Assert
            Assert.AreSame(data, node.Data);
            Assert.IsEmpty(node.Children);
            mocks.VerifyAll();
        }
    }
}