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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingInputViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            PipingInputView view = new PipingInputView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IChartView>(view);
            Assert.IsNotNull(view.Chart);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void DefaultConstructor_Always_AddChartControl()
        {
            // Call
            PipingInputView view = new PipingInputView();

            // Assert
            Assert.AreEqual(1, view.Controls.Count);
            ChartControl chartControl = view.Controls[0] as ChartControl;
            Assert.IsNotNull(chartControl);
            Assert.AreEqual(DockStyle.Fill, chartControl.Dock);
            Assert.IsNull(chartControl.Data);
            Assert.AreEqual(Resources.PipingInputView_Distance_DisplayName, chartControl.BottomAxisTitle);
            Assert.AreEqual(Resources.PipingInputView_Height_DisplayName, chartControl.LeftAxisTitle);
        }

        [Test]
        public void Data_PipingInput_DataSet()
        {
            // Setup
            PipingInput input = new PipingInput(new GeneralPipingInput());
            PipingInputView view = new PipingInputView();

            // Call
            view.Data = input;

            // Assert
            Assert.AreSame(input, view.Data);
        }

        [Test]
        public void Data_OtherThanPipingInput_DataNull()
        {
            // Setup
            object input = new object();
            PipingInputView view = new PipingInputView();

            // Call
            view.Data = input;

            // Assert
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Calculation_Always_SetsCalculationAndUpdateChartTitle()
        {
            // Setup
            PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = "Test name"
            };
            PipingInputView view = new PipingInputView();

            // Call
            view.Calculation = calculation;

            // Assert
            Assert.AreSame(calculation, view.Calculation);
            Assert.AreEqual(calculation.Name, view.Chart.ChartTitle);
        }
    }
}