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
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;
using System.Windows.Forms;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class IllustrationPointsControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var control = new IllustrationPointsControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(control);
            Assert.IsNull(control.Data);

            Assert.AreEqual(1, control.Controls.Count);

            var splitContainer = control.Controls[0] as SplitContainer;
            Assert.IsNotNull(splitContainer);
            Control.ControlCollection splitContainerPanel1Controls = splitContainer.Panel1.Controls;
            Assert.AreEqual(1, splitContainerPanel1Controls.Count);
            Assert.IsInstanceOf<IllustrationPointsChartControl>(splitContainerPanel1Controls[0]);

            Control.ControlCollection splitContainerPanel2Controls = splitContainer.Panel2.Controls;
            Assert.AreEqual(1, splitContainerPanel2Controls.Count);
            Assert.IsInstanceOf<IllustrationPointsTableControl>(splitContainerPanel2Controls[0]);
        }

        [Test]
        public void Data_ValueSet_DataSetToIllustrationPointsChartControl()
        {
            // Setup
            var data = new TestGeneralResult();
            var control = new IllustrationPointsControl();
            
            var chartControl = (IllustrationPointsChartControl) control.Controls.Find("IllustrationPointsChartControl", true).Single();

            // Call            
            control.Data = data;

            // Assert
            Assert.AreSame(data, control.Data);
            Assert.AreSame(data, chartControl.Data);
        }
    }
}