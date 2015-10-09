using System.Collections.Generic;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class MapTreeViewNodePresenterTest
    {
        private readonly MockRepository mocks = new MockRepository();

        [Test]
        public void TestGetChildNodeObjects()
        {
            ITreeView tv;
            var guiPlugin = CreateGuiPluginMock(out tv);

            var map = new Map();
            map.Layers.Add(new VectorLayer
            {
                Name = "Test1", ShowInTreeView = true
            });
            map.Layers.Add(new VectorLayer
            {
                Name = "Test2", ShowInTreeView = false
            });

            var mapTreeViewNodePresenter = new MapTreeViewNodePresenter(guiPlugin);
            var childNodes = mapTreeViewNodePresenter.GetChildNodeObjects(map, null);

            var enumerator = childNodes.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Test1", ((VectorLayer) enumerator.Current).Name);
            Assert.IsFalse(enumerator.MoveNext());
        }

        private GuiPlugin CreateGuiPluginMock(out ITreeView tv)
        {
            //create mocks because we are only testing presenter here
            var guiPlugin = mocks.Stub<GuiPlugin>();
            var gui = mocks.Stub<IGui>();
            var app = mocks.Stub<IApplication>();
            tv = mocks.Stub<ITreeView>();

            gui.Application = app;
            Expect.Call(gui.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            }).Repeat.Any();

            guiPlugin.Gui = gui;
            return guiPlugin;
        }
    }
}