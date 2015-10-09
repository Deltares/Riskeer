using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using GeoAPI.Extensions.Feature;
using NUnit.Framework;
using SharpMap.Api.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class MapViewTabControlTest
    {
        [Test]
        public void OpeningViewCallsActivate()
        {
            var view = new TestLayerEditorView();

            var mapTabControl = new MapViewTabControl();

            int view1Activated = 0, view1Deactivated = 0;
            view.OnActivatedCalled += (s, e) => view1Activated++;
            view.OnDeactivatedCalled += (s, e) => view1Deactivated++;

            mapTabControl.AddView(view);

            Assert.AreEqual(1, view1Activated);
            Assert.AreEqual(0, view1Deactivated);
        }

        [Test]
        public void RemovingViewDoesNotLeakView()
        {
            var mapTabControl = new MapViewTabControl();
            var weakRef = AddRemoveView(mapTabControl);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.IsFalse(weakRef.IsAlive);
        }

        [Test]
        public void DisposingTabWithOpenViewDoesNotLeakView()
        {
            var mapTabControl = new MapViewTabControl();
            var weakRef = AddViewDispose(mapTabControl);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.IsFalse(weakRef.IsAlive);
        }

        [Test]
        public void OpeningSecondViewCallsDeactivateAndActivate()
        {
            var view = new TestLayerEditorView();
            var view2 = new TestLayerEditorView();

            var mapTabControl = new MapViewTabControl();

            mapTabControl.AddView(view);

            int view1Activated = 0, view1Deactivated = 0, view2Activated = 0, view2Deactivated = 0;
            view.OnActivatedCalled += (s, e) => view1Activated++;
            view.OnDeactivatedCalled += (s, e) => view1Deactivated++;
            view2.OnActivatedCalled += (s, e) => view2Activated++;
            view2.OnDeactivatedCalled += (s, e) => view2Deactivated++;

            mapTabControl.AddView(view2);

            Assert.AreEqual(0, view1Activated);
            Assert.AreEqual(1, view1Deactivated);
            Assert.AreEqual(1, view2Activated);
            Assert.AreEqual(0, view2Deactivated);
        }

        [Test]
        public void ClosingInactivateViewCallsNothing()
        {
            var view = new TestLayerEditorView();
            var view2 = new TestLayerEditorView();

            var mapTabControl = new MapViewTabControl();

            mapTabControl.AddView(view);
            mapTabControl.AddView(view2);

            int view1Activated = 0, view1Deactivated = 0, view2Activated = 0, view2Deactivated = 0;
            view.OnActivatedCalled += (s, e) => view1Activated++;
            view.OnDeactivatedCalled += (s, e) => view1Deactivated++;
            view2.OnActivatedCalled += (s, e) => view2Activated++;
            view2.OnDeactivatedCalled += (s, e) => view2Deactivated++;

            mapTabControl.RemoveView(view);

            Assert.AreEqual(0, view1Activated);
            Assert.AreEqual(0, view1Deactivated);
            Assert.AreEqual(0, view2Activated);
            Assert.AreEqual(0, view2Deactivated);
        }

        [Test]
        public void ClosingViewCallsDeactivateAndActivate()
        {
            var view = new TestLayerEditorView();
            var view2 = new TestLayerEditorView();

            var mapTabControl = new MapViewTabControl();

            mapTabControl.AddView(view);
            mapTabControl.AddView(view2);

            int view1Activated = 0, view1Deactivated = 0, view2Activated = 0, view2Deactivated = 0;
            view.OnActivatedCalled += (s, e) => view1Activated++;
            view.OnDeactivatedCalled += (s, e) => view1Deactivated++;
            view2.OnActivatedCalled += (s, e) => view2Activated++;
            view2.OnDeactivatedCalled += (s, e) => view2Deactivated++;

            mapTabControl.RemoveView(view2);

            Assert.AreEqual(1, view1Activated);
            Assert.AreEqual(0, view1Deactivated);
            Assert.AreEqual(0, view2Activated);
            Assert.AreEqual(1, view2Deactivated);
        }

        [Test]
        public void SwitchingTabsCallsDeactivateAndActivateOnlyOnce()
        {
            var view = new TestLayerEditorView();
            var view2 = new TestLayerEditorView();

            var mapTabControl = new MapViewTabControl();

            mapTabControl.AddView(view);
            mapTabControl.AddView(view2);
            int view1Activated = 0, view1Deactivated = 0, view2Activated = 0, view2Deactivated = 0;
            view.OnActivatedCalled += (s, e) => view1Activated++;
            view.OnDeactivatedCalled += (s, e) => view1Deactivated++;
            view2.OnActivatedCalled += (s, e) => view2Activated++;
            view2.OnDeactivatedCalled += (s, e) => view2Deactivated++;

            mapTabControl.ActiveView = view;

            Assert.AreEqual(1, view1Activated);
            Assert.AreEqual(0, view1Deactivated);
            Assert.AreEqual(0, view2Activated);
            Assert.AreEqual(1, view2Deactivated);
        }

        private static WeakReference AddRemoveView(MapViewTabControl mapTabControl)
        {
            var view = new TestLayerEditorView();
            var weakRef = new WeakReference(view);
            mapTabControl.AddView(view);
            mapTabControl.RemoveView(view);
            return weakRef;
        }

        private static WeakReference AddViewDispose(MapViewTabControl mapTabControl)
        {
            var view = new TestLayerEditorView();
            var weakRef = new WeakReference(view);
            mapTabControl.AddView(view);
            mapTabControl.Dispose();
            return weakRef;
        }

        private class TestLayerEditorView : UserControl, ILayerEditorView
        {
            public event EventHandler SelectedFeaturesChanged;

            public event EventHandler OnActivatedCalled;
            public event EventHandler OnDeactivatedCalled;
            public object Data { get; set; }
            public string Text { get; set; }
            public Image Image { get; set; }

            public bool Visible { get; private set; }
            public ViewInfo ViewInfo { get; set; }
            public IEnumerable<IFeature> SelectedFeatures { get; set; }
            public ILayer Layer { set; get; }

            public void EnsureVisible(object item)
            {
                throw new NotImplementedException();
            }

            public void OnActivated()
            {
                if (OnActivatedCalled != null)
                {
                    OnActivatedCalled(this, EventArgs.Empty);
                }
            }

            public void OnDeactivated()
            {
                if (OnDeactivatedCalled != null)
                {
                    OnDeactivatedCalled(this, EventArgs.Empty);
                }
            }
        }
    }
}