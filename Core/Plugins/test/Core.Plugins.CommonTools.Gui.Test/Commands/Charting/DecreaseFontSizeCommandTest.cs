using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Gui;
using Core.Common.Test.TestObjects;
using Core.Plugins.CommonTools.Gui.Commands.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Commands.Charting
{
    [TestFixture]
    public class DecreaseFontSizeCommandTest
    {
        private MockRepository mocks;
        private IGui gui;
        private IViewList documentsView;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.StrictMock<IGui>();
            documentsView = mocks.StrictMock<IViewList>();
            gui.Expect(g => g.DocumentViews).Return(documentsView);
        }

        [Test]
        public void Execute_WithActiveChartView_FontSizesDecreasedByOne()
        {
            // Setup
            IChart chart = new Chart();
            var view = new ChartView
            {
                Chart = chart
            };
            var command = new DecreaseFontSizeCommand
            {
                Gui = gui
            };

            documentsView.Expect(dv => dv.ActiveView).Return(view);

            var family = FontFamily.GenericSansSerif;
            var size = 12;

            chart.Font = new Font(family, size);
            chart.Legend.Font = new Font(family, size);
            chart.LeftAxis.LabelsFont = new Font(family, size);
            chart.LeftAxis.TitleFont = new Font(family, size);
            chart.BottomAxis.LabelsFont = new Font(family, size);
            chart.BottomAxis.TitleFont = new Font(family, size);
            chart.RightAxis.LabelsFont = new Font(family, size);
            chart.RightAxis.TitleFont = new Font(family, size);

            mocks.ReplayAll();

            // Call
            command.Execute();

            // Assert
            var expectedSize = size - 1;
            Assert.AreEqual(expectedSize, chart.Font.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.Legend.Font.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.LeftAxis.LabelsFont.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.LeftAxis.TitleFont.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.BottomAxis.LabelsFont.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.BottomAxis.TitleFont.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.RightAxis.LabelsFont.SizeInPoints);
            Assert.AreEqual(expectedSize, chart.RightAxis.TitleFont.SizeInPoints);

            mocks.VerifyAll();
        }

        [Test]
        public void Execute_WithActiveOtherView_DoesNotThrow()
        {
            // Setup
            var view = new TestView();
            var command = new DecreaseFontSizeCommand
            {
                Gui = gui
            };

            documentsView.Expect(dv => dv.ActiveView).Return(view);

            mocks.ReplayAll();

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}