// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var view = new AssessmentSectionView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
        }

        [Test]
        public void DefaultConstructor_Always_AddsBaseMap()
        {
            // Call
            var view = new AssessmentSectionView();

            // Assert
            Assert.AreEqual(1, view.Controls.Count);
            object mapObject = view.Controls[0];
            Assert.IsInstanceOf<BaseMap>(mapObject);

            var map = (BaseMap)mapObject;
            Assert.AreEqual(DockStyle.Fill, map.Dock);
            Assert.NotNull(view.Map);
        }

        [Test]
        public void Data_SetToNull_BaseMapNoFeatures()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];

            // Call
            TestDelegate testDelegate = () => view.Data = null;

            // Assert
            Assert.DoesNotThrow(testDelegate);
            Assert.IsNull(map.Data);
        }

        [Test]
        public void Data_SetToMapPointData_MapDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];
            var assessmentSectionBase = new AssessmentSectionBaseTestClass();
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0, "level"));

            // Call
            view.Data = assessmentSectionBase;

            // Assert
            Assert.AreSame(assessmentSectionBase, view.Data);
            Assert.IsInstanceOf<MapData>(map.Data);
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
