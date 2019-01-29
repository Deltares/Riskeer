// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.Forms.ViewHost;
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
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Expect(r => r.OpenViewForData(selectedObject)).Return(true);
            var viewController = mocks.Stub<IViewController>();
            viewController.Stub(c => c.DocumentViewController).Return(documentViewController);
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = selectedObject;
            var pluginsHost = mocks.Stub<IPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

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
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Expect(r => r.GetViewInfosFor(viewObject)).Return(viewInfos);
            var viewController = mocks.Stub<IViewController>();
            viewController.Stub(c => c.DocumentViewController).Return(documentViewController);
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var pluginsHost = mocks.Stub<IPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            bool hasViewDefinitionsForData = commandHandler.CanOpenViewFor(viewObject);

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
            for (var i = 0; i < viewInfos.Length; i++)
            {
                viewInfos[i] = new ViewInfo();
            }

            var mocks = new MockRepository();
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Expect(r => r.GetViewInfosFor(viewObject)).Return(viewInfos);
            var viewController = mocks.Stub<IViewController>();
            viewController.Stub(c => c.DocumentViewController).Return(documentViewController);
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var pluginsHost = mocks.Stub<IPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            bool hasViewDefinitionsForData = commandHandler.CanOpenViewFor(viewObject);

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
            var documentViewController = mocks.Stub<IDocumentViewController>();
            documentViewController.Expect(r => r.OpenViewForData(viewObject)).Return(true);
            var viewController = mocks.Stub<IViewController>();
            viewController.Stub(c => c.DocumentViewController).Return(documentViewController);
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var pluginsHost = mocks.Stub<IPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

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
            var viewController = mocks.StrictMock<IViewController>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var pluginsHost = mocks.StrictMock<IPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

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
            var viewController = mocks.StrictMock<IViewController>();
            viewController.Expect(c => c.ViewHost).Return(null);
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var pluginsHost = mocks.StrictMock<IPluginsHost>();
            mocks.ReplayAll();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

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
            var documentViewsResolver = mocks.StrictMock<IDocumentViewController>();
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

            var viewHost = mocks.StrictMock<IViewHost>();
            viewHost.Stub(ws => ws.DocumentViews).Return(viewsArray);

            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var pluginsHost = mocks.Stub<IPluginsHost>();
            pluginsHost.Expect(g => g.GetAllDataWithViewDefinitionsRecursively(data)).Return(new[]
            {
                childData
            });
            var viewController = mocks.Stub<IViewController>();
            viewController.Stub(g => g.ViewHost).Return(viewHost);
            viewController.Stub(g => g.DocumentViewController).Return(documentViewsResolver);
            mocks.ReplayAll();

            var viewCommandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            viewCommandHandler.RemoveAllViewsForItem(data);

            // Assert
            mocks.VerifyAll();
        }
    }
}