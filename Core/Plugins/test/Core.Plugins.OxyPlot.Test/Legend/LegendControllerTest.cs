using System;
using Core.Common.Gui;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class LegendControllerTest
    {
        [Test]
        public void Constructor_WithoutPlugin_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new LegendController(null);

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
            TestDelegate test = () => new LegendController(toolViewController);

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
            toolViewController.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new LegendController(toolViewController);

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
                toolViewController.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(false);
                toolViewController.Expect(p => p.OpenToolView(Arg<LegendView>.Matches(c => true)));
                toolViewController.Expect(p => p.CloseToolView(Arg<LegendView>.Matches(c => true)));
            }
            else
            {
                toolViewController.Expect(p => p.OpenToolView(Arg<LegendView>.Matches(c => true)));
            }
            toolViewController.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new LegendController(toolViewController);

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