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

using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Core.Components.PointedTree.Forms;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Forms.Test
{
    [TestFixture]
    public class PointedTreeGraphControlTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ExptedValues()
        {
            // Call
            using (var graphControl = new PointedTreeGraphControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(graphControl);
                Assert.IsInstanceOf<IPointedTreeGraphControl>(graphControl);

                Assert.IsNull(graphControl.Data);

                Assert.AreEqual(1, graphControl.Controls.Count);
                Assert.IsInstanceOf<ElementHost>(graphControl.Controls[0]);
            }
        }
    }
}