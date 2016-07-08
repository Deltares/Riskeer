// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;

using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Selection;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class ViewCommandHandlerTest
    {
        [Test]
        public void OpenViewForSelection_OpenViewDialogForSelection()
        {
            // Setup
            var selectedObject = new object();

            var mocks = new MockRepository();
            var viewResolver = mocks.Stub<IViewResolver>();
            viewResolver.Expect(r => r.OpenViewForData(selectedObject)).Return(true);
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Stub(c => c.DocumentViewsResolver).Return(viewResolver);
            var toolViewController = mocks.Stub<IToolViewController>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = selectedObject;
            var guiPluginsHost = mocks.Stub<IGuiPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(documentViewController, toolViewController,
                                                        applicationSelection, guiPluginsHost);

            // Call
            commandHandler.OpenViewForSelection();

            // Assert
            mocks.VerifyAll(); // Expect open view method is called
        }

        [Test]
        public void CanOpenViewFor_NoViewInfosForTarget_ReturnFalse()
        {
            // Setup
            var viewObject = new object();

            var viewInfos = new ViewInfo[0];

            var mocks = new MockRepository();
            var viewResolver = mocks.Stub<IViewResolver>();
            viewResolver.Expect(r => r.GetViewInfosFor(viewObject)).Return(viewInfos);
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Stub(c => c.DocumentViewsResolver).Return(viewResolver);
            var toolViewController = mocks.Stub<IToolViewController>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var guiPluginsHost = mocks.Stub<IGuiPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(documentViewController, toolViewController,
                                                        applicationSelection, guiPluginsHost);

            // Call
            var hasViewDefinitionsForData = commandHandler.CanOpenViewFor(viewObject);

            // Assert
            Assert.IsFalse(hasViewDefinitionsForData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(11)]
        public void CanOpenViewFor_HasViewInfoDefinedForData_ReturnTrue(int numberOfViewDefinitions)
        {
            // Setup
            var viewObject = new object();

            var viewInfos = new ViewInfo[numberOfViewDefinitions];
            for (int i = 0; i < viewInfos.Length; i++)
            {
                viewInfos[i] = new ViewInfo();
            }

            var mocks = new MockRepository();
            var viewResolver = mocks.Stub<IViewResolver>();
            viewResolver.Expect(r => r.GetViewInfosFor(viewObject)).Return(viewInfos);
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Stub(c => c.DocumentViewsResolver).Return(viewResolver);
            var toolViewController = mocks.Stub<IToolViewController>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var guiPluginsHost = mocks.Stub<IGuiPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(documentViewController, toolViewController,
                                                        applicationSelection, guiPluginsHost);

            // Call
            var hasViewDefinitionsForData = commandHandler.CanOpenViewFor(viewObject);

            // Assert
            Assert.IsTrue(hasViewDefinitionsForData);
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_OpenViewDialogForSelection()
        {
            // Setup
            var viewObject = new object();

            var mocks = new MockRepository();
            var viewResolver = mocks.Stub<IViewResolver>();
            viewResolver.Expect(r => r.OpenViewForData(viewObject)).Return(true);
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Stub(c => c.DocumentViewsResolver).Return(viewResolver);
            var toolViewController = mocks.Stub<IToolViewController>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var guiPluginsHost = mocks.Stub<IGuiPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(documentViewController, toolViewController,
                                                        applicationSelection, guiPluginsHost);

            // Call
            commandHandler.OpenView(viewObject);

            // Assert
            mocks.VerifyAll(); // Expect open view method is called
        }

        [Test]
        public void RemoveAllViewsForItem_DataObjectNull_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var guiPluginsHost = mocks.StrictMock<IGuiPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(documentViewController, toolViewController,
                                                        applicationSelection, guiPluginsHost);

            // Call
            commandHandler.RemoveAllViewsForItem(null);

            // Assert
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        [Test]
        public void RemoveAllViewsForItem_DocumentViewsListNull_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            documentViewController.Expect(c => c.DocumentViews).Return(null);
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var guiPluginsHost = mocks.StrictMock<IGuiPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(documentViewController, toolViewController,
                                                        applicationSelection, guiPluginsHost);

            // Call
            commandHandler.RemoveAllViewsForItem(new object());

            // Assert
            mocks.VerifyAll(); // Expect no calls on mocks
        }

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
            toolWindows.Stub(ws => ws.GetEnumerator())
                       .WhenCalled(invocation => invocation.ReturnValue = viewsArray.GetEnumerator())
                       .Return(null);

            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var guiPluginsHost = mocks.Stub<IGuiPluginsHost>();
            guiPluginsHost.Expect(g => g.GetAllDataWithViewDefinitionsRecursively(data)).Return(new[]
            {
                childData
            });
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Stub(g => g.DocumentViews).Return(toolWindows);
            documentViewController.Stub(g => g.DocumentViewsResolver).Return(documentViewsResolver);
            var toolViewController = mocks.Stub<IToolViewController>();
            toolViewController.Stub(g => g.ToolWindowViews).Return(toolWindows);
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