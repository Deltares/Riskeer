using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
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
            using (var view = new LegendView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(Resources.General_Chart, view.Text);
            }
        }

        [Test]
        public void Data_ChartControl_DataSet()
        {
            // Setup 
            using (var view = new LegendView())
            {
                var chartDataCollection = new ChartDataCollection(new List<ChartData>());

                // Call
                view.Data = chartDataCollection;

                // Assert
                Assert.AreSame(chartDataCollection, view.Data);
            }
        }

        [Test]
        public void Data_ForNull_NullSet()
        {
            // Setup 
            using (var view = new LegendView())
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            using (var view = new LegendView())
            {
                // Call
                TestDelegate test = () => view.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(test);
            }
        }

        [Test]
        public void Dispose_Always_DataSetToNull()
        {
            // Setup
            var legendView = new LegendView
            {
                Data = new ChartDataCollection(new List<ChartData>())
            };

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(legendView, "treeViewControl");

            // Call
            legendView.Dispose();

            // Assert
            Assert.IsNull(legendView.Data);
            Assert.IsNull(treeViewControl.Data);
            Assert.IsTrue(treeViewControl.IsDisposed);
        }
    }
}