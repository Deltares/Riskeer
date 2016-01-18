using System;
using Core.Components.OxyPlot.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class ChartingInteractionControllerTest
    {
        [Test]
        public void Constructor_WithoutController_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartingInteractionController(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithController_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ChartingInteractionController(documentViewController);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void IsPanningEnabled_ActiveViewChartViewWithPanningOn_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            var view = new TestChartView();
            documentViewController.Expect(d => d.ActiveView).Return(view);

            mocks.ReplayAll();

            var baseChart = new BaseChart();
            baseChart.TogglePanning();
            view.Data = baseChart;
            var controller = new ChartingInteractionController(documentViewController);

            // Precondition
            Assert.True(view.Chart.IsPanning);

            // Call
            var isPanningEnabled = controller.IsPanningEnabled;

            // Assert
            Assert.IsTrue(isPanningEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsPanningEnabled_ActiveViewChartViewWithPanningOff_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            var view = new TestChartView();

            documentViewController.Expect(d => d.ActiveView).Return(view);

            mocks.ReplayAll();

            view.Data = new BaseChart();
            var controller = new ChartingInteractionController(documentViewController);

            // Precondition
            Assert.IsFalse(view.Chart.IsPanning);

            // Call
            var isPanningEnabled = controller.IsPanningEnabled;

            // Assert
            Assert.IsFalse(isPanningEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsPanningEnabled_ActiveViewNotChartView_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            var view = new TestView();
            documentViewController.Expect(d => d.ActiveView).Return(view);

            mocks.ReplayAll();

            var controller = new ChartingInteractionController(documentViewController);

            // Call 
            var isPanningEnabled = controller.IsPanningEnabled;

            // Assert
            Assert.IsFalse(isPanningEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void TogglePanning_ActiveViewChartViewWithPanningOn_ChartIsPanningToFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            var view = new TestChartView();
            documentViewController.Expect(d => d.ActiveView).Return(view);

            mocks.ReplayAll();

            var baseChart = new BaseChart();
            baseChart.TogglePanning();
            view.Data = baseChart;
            var controller = new ChartingInteractionController(documentViewController);

            // Precondition
            Assert.True(view.Chart.IsPanning);

            // Call
            controller.TogglePanning();

            // Assert
            Assert.IsFalse(view.Chart.IsPanning);
            mocks.VerifyAll();
        }

        [Test]
        public void TogglePanning_ActiveViewChartViewWithPanningOff_ChartIsPanningToTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            var view = new TestChartView();

            documentViewController.Expect(d => d.ActiveView).Return(view);

            mocks.ReplayAll();

            view.Data = new BaseChart();
            var controller = new ChartingInteractionController(documentViewController);

            // Precondition
            Assert.IsFalse(view.Chart.IsPanning);

            // Call
            controller.TogglePanning();

            // Assert
            Assert.IsTrue(view.Chart.IsPanning);
            mocks.VerifyAll();
        }

        [Test]
        public void TogglePanning_ActiveViewNotChartView_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();

            var view = new TestView();
            documentViewController.Expect(d => d.ActiveView).Return(view);

            mocks.ReplayAll();

            var controller = new ChartingInteractionController(documentViewController);

            // Call
            TestDelegate test = () => controller.TogglePanning();

            // Assert
            Assert.DoesNotThrow(test);
            mocks.VerifyAll();
        }
    }
}