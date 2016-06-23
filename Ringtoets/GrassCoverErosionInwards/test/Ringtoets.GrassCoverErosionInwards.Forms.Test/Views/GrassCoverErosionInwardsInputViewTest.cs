// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Windows.Forms;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.Forms;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddChartControlWithEmptyCollectionData()
        {
            // Call
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                ChartControl chartControl = view.Controls[0] as ChartControl;
                Assert.IsNotNull(chartControl);
                Assert.AreEqual(DockStyle.Fill, chartControl.Dock);
                Assert.IsNotNull(chartControl.Data);
                CollectionAssert.IsEmpty(chartControl.Data.List);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Distance_DisplayName, chartControl.BottomAxisTitle);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Height_DisplayName, chartControl.LeftAxisTitle);
            }
        }
    }
}