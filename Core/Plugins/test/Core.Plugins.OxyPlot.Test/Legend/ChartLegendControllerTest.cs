using System;
using Core.Common.Gui;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutPlugin_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartLegendController(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithToolViewController_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ChartLegendController(toolViewController);

            // Assert
            Assert.DoesNotThrow(test);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsLegendViewOpen_LegendViewOpenAndClosedState_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new ChartLegendController(toolViewController);

            // Call
            var result = controller.IsLegendViewOpen();

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleLegendView_LegendViewOpenAndClosedState_TogglesStateOfLegendView(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            if (open)
            {
                toolViewController.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(false);
                toolViewController.Expect(p => p.OpenToolView(Arg<ChartLegendView>.Matches(c => true)));
                toolViewController.Expect(p => p.CloseToolView(Arg<ChartLegendView>.Matches(c => true)));
            }
            else
            {
                toolViewController.Expect(p => p.OpenToolView(Arg<ChartLegendView>.Matches(c => true)));
            }
            toolViewController.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new ChartLegendController(toolViewController);

            if (open)
            {
                controller.ToggleLegend();
            }

            // Call
            controller.ToggleLegend();

            // Assert
            mocks.VerifyAll();
        }
    }
}