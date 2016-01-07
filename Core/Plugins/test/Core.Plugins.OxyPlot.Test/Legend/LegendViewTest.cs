using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class LegendViewTest
    {
        [Test]
        public void DefaultConstructor_CreatesUserControl()
        {
            // Call 
            var view = new LegendView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
        }

        [Test]
        public void Data_BaseChart_DataSet()
        {
            // Setup 
            var view = new LegendView();
            var baseChart = new BaseChart();

            // Call
            view.Data = baseChart;

            // Assert
            Assert.AreSame(baseChart, view.Data);
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            var view = new LegendView();

            // Call
            TestDelegate test = () => view.Data = new object();

            // Assert
            Assert.Throws<InvalidCastException>(test);
        }
    }
}