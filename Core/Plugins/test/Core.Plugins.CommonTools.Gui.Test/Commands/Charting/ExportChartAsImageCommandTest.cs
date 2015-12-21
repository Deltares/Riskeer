using Core.Common.Controls.Charting;
using Core.Common.Gui;
using Core.Common.Test.TestObjects;
using Core.Plugins.Charting.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Commands.Charting
{
    [TestFixture]
    public class ExportChartAsImageCommandTest
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
        public void Execute_WithActiveChartView_CallsExportAsImage()
        {
            // Setup
            var chartView = mocks.StrictMock<IChartView>();
            chartView.Expect(cv => cv.ExportAsImage());
            documentsView.Expect(dv => dv.ActiveView).Return(chartView);

            mocks.ReplayAll();

            var command = new ExportChartAsImageCommand
            {
                Gui = gui
            };

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_WithActiveOtherView_DoesNotThrow()
        {
            // Setup
            var view = new TestView();
            var command = new IncreaseFontSizeCommand
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