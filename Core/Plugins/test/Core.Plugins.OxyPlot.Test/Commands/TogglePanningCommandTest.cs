using System;
using Core.Common.Controls.Commands;
using Core.Common.Controls.Views;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Commands
{
    [TestFixture]
    public class TogglePanningCommandTest
    {
        [Test]
        public void Constructor_WithoutController_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TogglePanningCommand(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithController_NewICommand()
        {
            // Call
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            
            mocks.ReplayAll();

            var command = new TogglePanningCommand(new ChartingInteractionController(documentViewController));

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            mocks.VerifyAll();
        }

        [Test]
        public void Enabled_Always_True()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            
            mocks.ReplayAll();

            var command = new TogglePanningCommand(new ChartingInteractionController(documentViewController));

            // Call & Assert
            Assert.IsTrue(command.Enabled);
        }

        [Test]
        public void Checked_ActiveViewChartPanningOn_True()
        {
            // Setup
            var view = new ChartDataView();
            view.Chart.TogglePanning();
            var command = new TogglePanningCommand(new ChartingInteractionController(new TestDocumentViewController(view)));

            // Precondition
            Assert.IsTrue(view.Chart.IsPanning);

            // Call & Assert
            Assert.IsTrue(command.Checked);
        }

        [Test]
        public void Checked_ActiveViewChartPanningOff_False()
        {
            // Setup
            var view = new ChartDataView();
            var command = new TogglePanningCommand(new ChartingInteractionController(new TestDocumentViewController(view)));

            // Precondition
            Assert.IsFalse(view.Chart.IsPanning);
            
            // Call & Assert
            Assert.IsFalse(command.Checked);
        }

        [Test]
        public void Checked_ActiveViewWithoutChart_False()
        {
            // Setup
            var view = new TestView();
            var command = new TogglePanningCommand(new ChartingInteractionController(new TestDocumentViewController(view)));

            // Call & Assert
            Assert.IsFalse(command.Checked);
        }

        [Test]
        public void Execute_ActiveViewChartPanningOn_IsPanningChangesToFalse()
        {
            // Setup
            var view = new ChartDataView();
            view.Chart.TogglePanning();
            var command = new TogglePanningCommand(new ChartingInteractionController(new TestDocumentViewController(view)));

            // Precondition
            Assert.IsTrue(view.Chart.IsPanning);

            // Call
            command.Execute();
            
            // Assert
            Assert.IsFalse(command.Checked);
        }

        [Test]
        public void Execute_ActiveViewChartPanningOff_IsPanningChangesToTrue()
        {
            // Setup
            var view = new ChartDataView();
            var command = new TogglePanningCommand(new ChartingInteractionController(new TestDocumentViewController(view)));

            // Precondition
            Assert.IsFalse(view.Chart.IsPanning);

            // Call
            command.Execute();

            // Assert
            Assert.IsTrue(command.Checked);
        }

        [Test]
        public void Execute_ActiveViewWithoutChart_DoesNotThrow()
        {
            // Setup
            var view = new TestView();
            var command = new TogglePanningCommand(new ChartingInteractionController(new TestDocumentViewController(view)));

            // Call & Assert
            Assert.DoesNotThrow(() => command.Execute());
        }
    }

    public class TestDocumentViewController : IDocumentViewController
    {
        private readonly IView view;

        public TestDocumentViewController(IView view)
        {
            this.view = view;
        }

        public IView ActiveView
        {
            get
            {
                return view;
            } 
        }
    }
}