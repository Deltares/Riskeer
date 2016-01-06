using System.Collections.Generic;

using Core.Common.Controls.Views;
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

        [Test]
        public void RemoveAllViewsForItem_GuiHasDocumentViews_CloseViewForDataAndChildren()
        {
            // Setup
            var data = new object();
            var childData = new object();

            var documentViewsResolver = mocks.StrictMock<IViewResolver>();
            documentViewsResolver.Expect(vr => vr.CloseAllViewsFor(data));
            documentViewsResolver.Expect(vr => vr.CloseAllViewsFor(childData));

            var dataView = mocks.Stub<IView>();
            dataView.Data = data;
            var childDataView = mocks.Stub<IView>();
            childDataView.Data = childData;

            var viewsArray = new List<IView>
            {
                dataView,
                childDataView
            };
            var toolWindows = mocks.StrictMock<IViewList>();
            toolWindows.Stub(ws => ws.GetEnumerator()).WhenCalled(invocation => invocation.ReturnValue = viewsArray.GetEnumerator()).Return(null);
            toolWindows.Expect(ws => ws.Count).Return(viewsArray.Count);

            var gui = mocks.Stub<IGui>();
            gui.Expect(g => g.GetAllDataWithViewDefinitionsRecursively(data)).Return(new[]
            {
                data,
                childData
            });
            gui.Expect(g => g.DocumentViews).Return(toolWindows).Repeat.AtLeastOnce();
            gui.Expect(g => g.DocumentViewsResolver).Return(documentViewsResolver).Repeat.AtLeastOnce();
            gui.Expect(g => g.ToolWindowViews).Return(toolWindows).Repeat.AtLeastOnce();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            gui.Stub(g => g.MainWindow).Return(null);
            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);

            // Call
            guiCommandHandler.RemoveAllViewsForItem(data);

            // Assert
            Assert.IsNull(dataView.Data);
            Assert.IsNull(childDataView.Data);
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