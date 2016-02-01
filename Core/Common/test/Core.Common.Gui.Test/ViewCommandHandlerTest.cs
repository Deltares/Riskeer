using System.Collections.Generic;

using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Selection;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class ViewCommandHandlerTest
    {
        [Test]
        public void RemoveAllViewsForItem_GuiHasDocumentViews_CloseViewForDataAndChildren()
        {
            // Setup
            var data = new object();
            var childData = new object();

            var mocks = new MockRepository();
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

            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var guiPluginsHost = mocks.Stub<IGuiPluginsHost>();
            guiPluginsHost.Expect(g => g.GetAllDataWithViewDefinitionsRecursively(data)).Return(new[]
            {
                data,
                childData
            });
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Expect(g => g.DocumentViews).Return(toolWindows).Repeat.AtLeastOnce();
            documentViewController.Expect(g => g.DocumentViewsResolver).Return(documentViewsResolver).Repeat.AtLeastOnce();
            var toolViewController = mocks.Stub<IToolViewController>();
            toolViewController.Expect(g => g.ToolWindowViews).Return(toolWindows).Repeat.AtLeastOnce();
            mocks.ReplayAll();
            
            var viewCommandHandler = new ViewCommandHandler(documentViewController, toolViewController, applicationSelection, guiPluginsHost);

            // Call
            viewCommandHandler.RemoveAllViewsForItem(data);

            // Assert
            Assert.IsNull(dataView.Data);
            Assert.IsNull(childDataView.Data);
            mocks.VerifyAll();
        }
    }
}