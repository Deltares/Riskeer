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
            gui.Expect(g => g.Plugins).Return(new GuiPlugin[] {new TestGuiPlugin()});
            gui.Expect(g => g.Selection).Return(new AnObject());

            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);

            // Call
            var canShowProperties = guiCommandHandler.CanShowPropertiesForGuiSelection();

            // Assert
            Assert.IsTrue(canShowProperties);

            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesForGuiSelection_PropertiesForSuperObjectDefined_True()
        {
            // Setup
            var gui = mocks.DynamicMock<IGui>();
            gui.Expect(g => g.Plugins).Return(new GuiPlugin[] {new TestGuiPlugin()});
            gui.Expect(g => g.Selection).Return(new ASubObject());

            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);

            // Call
            var canShowProperties = guiCommandHandler.CanShowPropertiesForGuiSelection();

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