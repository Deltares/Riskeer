using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.Selection;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class ApplicationFeatureCommandHandlerTest
    {
        [Test]
        public void ShowPropertiesFor_InitializeAndShowPropertyGridAndUpdateSelection()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            var mainWindow = mocks.Stub<IMainWindow>();
            mainWindow.Expect(w => w.InitPropertiesWindowAndActivate());
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow, applicationSelection);

            // Call
            commandHandler.ShowPropertiesFor(target);

            // Assert
            Assert.AreSame(target, applicationSelection.Selection);
            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesFor_ObjectHasProperties_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(target))
                            .Return(mocks.Stub<IObjectProperties>());
            var mainWindow = mocks.Stub<IMainWindow>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow, applicationSelection);

            // Call
            var result = commandHandler.CanShowPropertiesFor(target);

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesFor_ObjectDoesNotHaveProperties_ReturnFalse()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(target))
                            .Return(null);
            var mainWindow = mocks.Stub<IMainWindow>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow, applicationSelection);

            // Call
            var result = commandHandler.CanShowPropertiesFor(target);

            // Assert
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }
    }
}