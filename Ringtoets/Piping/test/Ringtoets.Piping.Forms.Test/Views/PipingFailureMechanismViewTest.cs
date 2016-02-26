using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var view = new PipingFailureMechanismView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
        }

        [Test]
        public void DefaultConstructor_Always_AddsBaseMap()
        {
            // Call
            var view = new PipingFailureMechanismView();

            // Assert
            Assert.AreEqual(1, view.Controls.Count);
            object mapObject = view.Controls[0];
            Assert.IsInstanceOf<BaseMap>(mapObject);

            var map = (BaseMap)mapObject;
            Assert.AreEqual(DockStyle.Fill, map.Dock);
            Assert.NotNull(view.Map);
        }

        [Test]
        public void Data_ReferenceLineNull_NoLineDataSet()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            // Call
            view.Data = pipingContext;

            // Assert
            var mapData = (MapDataCollection)map.Data;

            Assert.AreEqual(1, mapData.List.Count);
            Assert.IsNotInstanceOf<MapLineData>(mapData.List[0]);
        }

        [Test]
        public void Data_HydraulicBoundaryDatabaseNull_NoPointDataSet()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            // Call
            view.Data = pipingContext;

            // Assert
            var mapData = (MapDataCollection)map.Data;

            Assert.AreEqual(1, mapData.List.Count);
            Assert.IsNotInstanceOf<MapPointData>(mapData.List[0]);
        }

        [Test]
        public void Data_SetToNull_BaseMapNoFeatures()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            // Call
            TestDelegate testDelegate = () => view.Data = null;

            // Assert
            Assert.DoesNotThrow(testDelegate);
            Assert.IsNull(map.Data);
        }

        private class AssessmentSectionBaseTestClass : AssessmentSectionBase
        {
            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }
        }
    }
}
