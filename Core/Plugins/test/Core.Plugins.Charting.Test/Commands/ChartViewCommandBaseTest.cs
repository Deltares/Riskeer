using System;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Gui;
using Core.Plugins.Charting.Commands;
using Core.Plugins.CommonTools.Gui.Test.TestObjects;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Charting.Test.Commands
{
    [TestFixture]
    public class ChartViewCommandBaseTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        [TestCase(11)]
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-11)]
        [TestCase(-5)]
        public void GetChangedFontSize_StartWithSize12ModifiedWithPoints_ReturnsSmallerFont(int points)
        {
            // Setup
            var command = new TestChartViewCommandBase();
            var startSize = 12f;
            var font = new Font(FontFamily.GenericSansSerif, startSize);

            // Call
            var newFont = command.GetChangedFontSize(font, points);

            // Assert
            Assert.AreEqual(startSize + points, newFont.SizeInPoints);
        }

        [Test]
        [TestCase(-12)]
        [TestCase(-13)]
        public void GetChangedFontSize_StartWithSize12ModifiedWithPointsLargerEqualTo12_ThrowsArgumentException(int points)
        {
            // Setup
            var command = new TestChartViewCommandBase();
            var startSize = 12f;
            var font = new Font(FontFamily.GenericSansSerif, startSize);

            // Call
            TestDelegate test = () => command.GetChangedFontSize(font, points);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void View_WithoutGui_Null()
        {
            // Setup
            var command = new TestChartViewCommandBase();

            // Call
            var result = command.GetView();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void View_GuiWithoutDocumentView_ThrowsNullReferenceException()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            guiMock.Expect(g => g.DocumentViews).Return(null);

            mocks.ReplayAll();

            var command = new TestChartViewCommandBase
            {
                Gui = guiMock
            };

            // Call
            TestDelegate test = () => command.GetView();

            // Assert
            Assert.Throws<NullReferenceException>(test);

            mocks.VerifyAll();
        }

        [Test]
        public void View_GuiWithDocumentViewActiveViewNull_CallsActiveViewReturnsNull()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var viewListMock = mocks.StrictMock<IViewList>();
            guiMock.Expect(g => g.DocumentViews).Return(viewListMock);
            viewListMock.Expect(vl => vl.ActiveView).Return(null);

            mocks.ReplayAll();

            var command = new TestChartViewCommandBase
            {
                Gui = guiMock
            };

            // Call
            var result = command.GetView();

            // Assert
            Assert.IsNull(result);

            mocks.VerifyAll();
        }

        [Test]
        public void View_GuiWithDocumentViewActiveViewNotChartView_CallsActiveViewReturnsNull()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var viewListMock = mocks.StrictMock<IViewList>();
            guiMock.Expect(g => g.DocumentViews).Return(viewListMock);
            viewListMock.Expect(vl => vl.ActiveView).Return(new TestView());

            mocks.ReplayAll();

            var command = new TestChartViewCommandBase
            {
                Gui = guiMock
            };

            // Call
            var result = command.GetView();

            // Assert
            Assert.IsNull(result);

            mocks.VerifyAll();
        }

        [Test]
        public void View_GuiWithDocumentViewActiveViewChartView_CallsActiveViewReturnsChartView()
        {
            // Setup
            var expectedView = new ChartView();

            var guiMock = mocks.StrictMock<IGui>();
            var viewListMock = mocks.StrictMock<IViewList>();
            guiMock.Expect(g => g.DocumentViews).Return(viewListMock);
            viewListMock.Expect(vl => vl.ActiveView).Return(expectedView);

            mocks.ReplayAll();

            var command = new TestChartViewCommandBase
            {
                Gui = guiMock
            };

            // Call
            var result = command.GetView();

            // Assert
            Assert.AreSame(expectedView, result);

            mocks.VerifyAll();
        }

        private class TestChartViewCommandBase : ChartViewCommandBase {
            public override void Execute(params object[] args)
            {
                throw new NotImplementedException();
            }

            public new Font GetChangedFontSize(Font font, int points)
            {
                return base.GetChangedFontSize(font, points);
            }

            public IChartView GetView()
            {
                return View;
            }
        } 
    }
}