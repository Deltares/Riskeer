using System.Collections.Generic;
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
        public void CanShowPropertiesForGuiSelection_PropertiesForObjectDefined_True()
        {
            // Setup
            var gui = mocks.DynamicMock<IGui>();
            var anObject = new AnObject();
            gui.Expect(g => g.Plugins).Return(new GuiPlugin[] {new TestGuiPlugin()});
            gui.Expect(g => g.Selection).Return(anObject);

            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);

            // Call
            var canShowProperties = guiCommandHandler.CanShowPropertiesFor(anObject);

            // Assert
            Assert.IsTrue(canShowProperties);

            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesForGuiSelection_PropertiesForSuperObjectDefined_True()
        {
            // Setup
            var gui = mocks.DynamicMock<IGui>();
            var aSubObject = new ASubObject();
            gui.Expect(g => g.Plugins).Return(new GuiPlugin[] {new TestGuiPlugin()});
            gui.Expect(g => g.Selection).Return(aSubObject);

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