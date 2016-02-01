using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Selection;

using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    public class ApplicationFeatureCommandHandlerTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void CanShowPropertiesFor_PropertiesForObjectDefined_True()
        {
            // Setup
            var anObject = new AnObject();

            var propertyResolverMock = mocks.StrictMock<IPropertyResolver>();
            propertyResolverMock.Expect(pr => pr.GetObjectProperties(anObject)).Return(new object());

            var mainWindow = mocks.Stub<IMainWindow>();

            var applicationSelection = mocks.Stub<IApplicationSelection>();

            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolverMock, mainWindow, applicationSelection);

            // Call
            var canShowProperties = commandHandler.CanShowPropertiesFor(anObject);

            // Assert
            Assert.IsTrue(canShowProperties);

            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesFor_PropertiesForSuperObjectDefined_True()
        {
            // Setup
            var aSubObject = new ASubObject();

            var propertyResolverMock = mocks.StrictMock<IPropertyResolver>();
            propertyResolverMock.Expect(pr => pr.GetObjectProperties(aSubObject)).Return(new object());

            var mainWindow = mocks.Stub<IMainWindow>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();

            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolverMock, mainWindow, applicationSelection);

            // Call
            var canShowProperties = commandHandler.CanShowPropertiesFor(aSubObject);

            // Assert
            Assert.IsTrue(canShowProperties);

            mocks.VerifyAll();
        }

        public class AnObject {}

        public class ASubObject : AnObject {}
    }
}