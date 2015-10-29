using Core.Common.Utils.Editing;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Editors;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Styles;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Editors
{
    [TestFixture]
    public class FeatureInteractorTest
    {
        [Test]
        public void CanDeleteAndMoveDependsOnGroupLayerReadOnly()
        {
            var map = new Map.Map();
            var groupLayer1 = new GroupLayer();
            var groupLayer2 = new GroupLayer();
            var layer = new VectorLayer();
            groupLayer1.Layers.Add(groupLayer2);
            groupLayer2.Layers.Add(layer);
            map.Layers.Add(groupLayer1);

            var editor = new TestFeatureInteractor(layer, null, null, null)
            {
                IsEditable = true
            };

            Assert.IsTrue(editor.AllowDeletion());
            Assert.IsTrue(editor.AllowMove());

            groupLayer1.ReadOnly = true;

            Assert.IsFalse(editor.AllowDeletion());
            Assert.IsFalse(editor.AllowMove());
        }

        [Test]
        public void CanDeleteAndMoveDependsOnGroupLayerReadOnlyAndFeatureItself()
        {
            var map = new Map.Map();
            var groupLayer1 = new GroupLayer();
            var groupLayer2 = new GroupLayer();
            var layer = new VectorLayer();
            groupLayer1.Layers.Add(groupLayer2);
            groupLayer2.Layers.Add(layer);
            map.Layers.Add(groupLayer1);

            var editor = new TestFeatureInteractor(layer, null, null, null)
            {
                IsEditable = false
            };

            Assert.IsFalse(editor.AllowDeletion());
            Assert.IsFalse(editor.AllowMove());

            groupLayer1.ReadOnly = true;

            Assert.IsFalse(editor.AllowDeletion());
            Assert.IsFalse(editor.AllowMove());
        }

        [Test]
        public void CanDeleteAndMoveDoesNotCrashWithEmptyLayer()
        {
            var editor = new TestFeatureInteractor(null, null, null, null);

            Assert.IsFalse(editor.AllowDeletion());
            Assert.IsFalse(editor.AllowMove());
        }

        [Test]
        public void CanDeleteAndMoveDoesNotCrashWithEmptyMap()
        {
            var layer = new VectorLayer();

            var editor = new TestFeatureInteractor(layer, null, null, null);

            Assert.IsFalse(editor.AllowDeletion());
            Assert.IsFalse(editor.AllowMove());
        }

        private class TestFeatureInteractor : FeatureInteractor
        {
            public TestFeatureInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle, IEditableObject editableObject)
                : base(layer, feature, vectorStyle, editableObject) {}

            public bool IsEditable { get; set; }

            protected override bool AllowDeletionCore()
            {
                return IsEditable;
            }

            protected override void CreateTrackers() {}

            protected override bool AllowMoveCore()
            {
                return IsEditable;
            }
        }
    }
}