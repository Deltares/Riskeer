﻿using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
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
            Assert.IsNull(view.Data);
            Assert.AreEqual(Resources.General_Chart, view.Text);
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
        public void Data_ForNull_NullSet()
        {
            // Setup 
            var view = new LegendView();

            // Call
            view.Data = null;

            // Assert
            Assert.IsNull(view.Data);
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