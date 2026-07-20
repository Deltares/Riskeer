// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Core.Common.Controls.Views;
using Core.Gui.Commands;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.Selection;
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    public class ViewCommandHandlerTest
    {
        [Test]
        public void OpenViewForSelection_OpenViewDialogForSelection()
        {
            // Setup
            var selectedObject = new object();

            var documentViewController = Substitute.For<IDocumentViewController>();
            documentViewController.OpenViewForData(selectedObject).Returns(true);
            var viewController = Substitute.For<IViewController>();
            viewController.DocumentViewController.Returns(documentViewController);
            var applicationSelection = Substitute.For<IApplicationSelection>();
            applicationSelection.Selection = selectedObject;
            var pluginsHost = Substitute.For<IPluginsHost>();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            commandHandler.OpenViewForSelection();

            // Assert
            documentViewController.Received().OpenViewForData(selectedObject);
        }

        [Test]
        public void CanOpenViewFor_NoViewInfosForTarget_ReturnFalse()
        {
            // Setup
            var viewObject = new object();

            var viewInfos = new ViewInfo[0];

            var documentViewController = Substitute.For<IDocumentViewController>();
            documentViewController.GetViewInfosFor(viewObject).Returns(viewInfos);
            var viewController = Substitute.For<IViewController>();
            viewController.DocumentViewController.Returns(documentViewController);
            var applicationSelection = Substitute.For<IApplicationSelection>();
            var pluginsHost = Substitute.For<IPluginsHost>();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            bool hasViewDefinitionsForData = commandHandler.CanOpenViewFor(viewObject);

            // Assert
            Assert.IsFalse(hasViewDefinitionsForData);
            documentViewController.Received().GetViewInfosFor(viewObject);
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

            var documentViewController = Substitute.For<IDocumentViewController>();
            documentViewController.GetViewInfosFor(viewObject).Returns(viewInfos);
            var viewController = Substitute.For<IViewController>();
            viewController.DocumentViewController.Returns(documentViewController);
            var applicationSelection = Substitute.For<IApplicationSelection>();
            var pluginsHost = Substitute.For<IPluginsHost>();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            bool hasViewDefinitionsForData = commandHandler.CanOpenViewFor(viewObject);

            // Assert
            Assert.IsTrue(hasViewDefinitionsForData);
            documentViewController.Received().GetViewInfosFor(viewObject);
        }

        [Test]
        public void OpenView_OpenViewDialogForSelection()
        {
            // Setup
            var viewObject = new object();

            var documentViewController = Substitute.For<IDocumentViewController>();
            documentViewController.OpenViewForData(viewObject).Returns(true);
            var viewController = Substitute.For<IViewController>();
            viewController.DocumentViewController.Returns(documentViewController);
            var applicationSelection = Substitute.For<IApplicationSelection>();
            var pluginsHost = Substitute.For<IPluginsHost>();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            commandHandler.OpenView(viewObject);

            // Assert
            documentViewController.Received().OpenViewForData(viewObject);
        }

        [Test]
        public void RemoveAllViewsForItem_DataObjectNull_DoNothing()
        {
            // Setup
            var viewController = Substitute.For<IViewController>();
            var applicationSelection = Substitute.For<IApplicationSelection>();
            var pluginsHost = Substitute.For<IPluginsHost>();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            commandHandler.RemoveAllViewsForItem(null);

            // Assert
            _ = viewController.DidNotReceive().ViewHost;
        }

        [Test]
        public void RemoveAllViewsForItem_DocumentViewsListNull_DoNothing()
        {
            // Setup
            var viewController = Substitute.For<IViewController>();
            viewController.ViewHost.Returns((IViewHost) null);
            var applicationSelection = Substitute.For<IApplicationSelection>();
            var pluginsHost = Substitute.For<IPluginsHost>();

            var commandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            commandHandler.RemoveAllViewsForItem(new object());

            // Assert
            pluginsHost.DidNotReceive().GetAllDataWithViewDefinitionsRecursively(Arg.Any<object>());
        }

        [Test]
        public void RemoveAllViewsForItem_GuiHasDocumentViews_CloseViewForDataAndChildren()
        {
            // Setup
            var data = new object();
            var childData = new object();

            var documentViewsResolver = Substitute.For<IDocumentViewController>();

            var dataView = Substitute.For<IView>();
            dataView.Data.Returns(data);
            var childDataView = Substitute.For<IView>();
            childDataView.Data.Returns(childData);

            var viewsArray = new List<IView>
            {
                dataView,
                childDataView
            };

            var viewHost = Substitute.For<IViewHost>();
            viewHost.DocumentViews.Returns(viewsArray);

            var applicationSelection = Substitute.For<IApplicationSelection>();
            var pluginsHost = Substitute.For<IPluginsHost>();
            pluginsHost.GetAllDataWithViewDefinitionsRecursively(data).Returns(new[]
            {
                childData
            });
            var viewController = Substitute.For<IViewController>();
            viewController.ViewHost.Returns(viewHost);
            viewController.DocumentViewController.Returns(documentViewsResolver);

            var viewCommandHandler = new ViewCommandHandler(viewController, applicationSelection, pluginsHost);

            // Call
            viewCommandHandler.RemoveAllViewsForItem(data);

            // Assert
            documentViewsResolver.Received().CloseAllViewsFor(data);
            documentViewsResolver.Received().CloseAllViewsFor(childData);
            pluginsHost.Received().GetAllDataWithViewDefinitionsRecursively(data);
        }
    }
}