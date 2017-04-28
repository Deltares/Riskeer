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

using System.Linq;
using System.Windows.Forms;
using Core.Components.Charting.Forms;
using NUnit.Framework;
using Ringtoets.Revetment.Forms.Views;

namespace Ringtoets.Revetment.Forms.Test.Views
{
    [TestFixture]
    public class WaveConditionsInputViewTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new WaveConditionsInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
            }
        }

        [Test]
        public void Constructor_Always_AddEmptyChartControl()
        {
            // Call
            using (var view = new WaveConditionsInputView())
            {
                // Assert
                var chartControl = (IChartControl)view.Controls.Find("chartControl", true).First();
                Assert.IsInstanceOf<Control>(chartControl);
                Assert.AreSame(chartControl, chartControl);
                Assert.AreEqual(DockStyle.Fill, ((Control)chartControl).Dock);
                Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
                Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);
                Assert.IsNull(chartControl.Data);
            }
        }
    }
}