// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class AssessmentSectionExtendedViewTest : NUnitFormTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;

        private Form testForm;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            // Call
            AssessmentSectionExtendedView extendedView = ShowCalculationsView(assessmentSection);

            // Assert
            Assert.IsInstanceOf<AssessmentSectionReferenceLineView>(extendedView);
        }

        [Test]
        public void Constructor_WithReferenceLineAndHydraulicBoundaryDatabase_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = referenceLine
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
            });

            // Call
            AssessmentSectionExtendedView extendedView = ShowCalculationsView(assessmentSection);

            // Assert
            Assert.IsInstanceOf<MapDataCollection>(extendedView.Map.Data);
            MapDataCollection mapData = extendedView.Map.Data;
            Assert.IsNotNull(mapData);

            MapData hydraulicBoundaryLocationsMapData = mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex);
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);

            MapData referenceLineMapData = mapData.Collection.ElementAt(referenceLineIndex);
            MapDataTestHelper.AssertReferenceLineMapData(referenceLine, referenceLineMapData);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = referenceLine
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
            });

            AssessmentSectionExtendedView extendedView = ShowCalculationsView(assessmentSection);
            MapDataCollection mapData = extendedView.Map.Data;

            MapData dataToMove = mapData.Collection.ElementAt(0);
            mapData.Remove(dataToMove);
            mapData.Add(dataToMove);

            // Precondition
            var referenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);

            var hrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex - 1);
            Assert.AreEqual("Hydraulische belastingen", hrLocationsMapData.Name);

            // Call
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
            });
            assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

            // Assert
            var actualReferenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
            Assert.AreEqual("Referentielijn", actualReferenceLineMapData.Name);

            var actualHrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex - 1);
            Assert.AreEqual("Hydraulische belastingen", actualHrLocationsMapData.Name);
        }

        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        public override void TearDown()
        {
            base.TearDown();

            testForm.Dispose();
        }

        private AssessmentSectionExtendedView ShowCalculationsView(IAssessmentSection assessmentSection)
        {
            var assessmentSectionView = new AssessmentSectionExtendedView(assessmentSection);
            testForm.Controls.Add(assessmentSectionView);
            testForm.Show();

            return assessmentSectionView;
        }
    }
}