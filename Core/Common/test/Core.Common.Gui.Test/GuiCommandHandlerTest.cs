using System.Collections.Generic;
using Core.Common.Gui.Forms.PropertyGridView;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    public class GuiCommandHandlerTest
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
            var gui = mocks.DynamicMock<IGui>();
            var propertyResolverMock = mocks.StrictMock<IPropertyResolver>();
            var anObject = new AnObject();

            propertyResolverMock.Expect(pr => pr.GetObjectProperties(anObject)).Return(new object());
            gui.Expect(g => g.PropertyResolver).Return(propertyResolverMock);

            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);

            // Call
            var canShowProperties = guiCommandHandler.CanShowPropertiesFor(anObject);

            // Assert
            Assert.IsTrue(canShowProperties);

            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesFor_PropertiesForSuperObjectDefined_True()
        {
            // Setup
            var gui = mocks.DynamicMock<IGui>();
            var aSubObject = new ASubObject();
            var propertyResolverMock = mocks.StrictMock<IPropertyResolver>();

            propertyResolverMock.Expect(pr => pr.GetObjectProperties(aSubObject)).Return(new object());
            gui.Expect(g => g.PropertyResolver).Return(propertyResolverMock);

            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);

            // Call
            var canShowProperties = guiCommandHandler.CanShowPropertiesFor(aSubObject);

            // Assert
            Assert.IsTrue(canShowProperties);

            mocks.VerifyAll();
        }
    }

    public class TestGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<AnObject, AnObjectProperties>();
        }
    }

    public class AnObjectProperties : IObjectProperties {
        public object Data { get; set; }
    }

    public class AnObject {}

    public class ASubObject : AnObject { }
}