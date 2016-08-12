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

using System.Windows.Forms;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationDesignWaterLevelsViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.IsNull(view.AssessmentSection);
            }
        }

        [Test]
        public void Data_HydraulicBoundaryDatabase_DataSet()
        {
            // Setup
            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView())
            {
                var hydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();

                // Call
                view.Data = hydraulicBoundaryDatabase;

                // Assert
                Assert.AreSame(hydraulicBoundaryDatabase, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanHydraulicBoundaryDatabase_DataNull()
        {
            // Setup
            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }
    }

    public class TestHydraulicBoundaryDatabase : HydraulicBoundaryDatabase
    {
        public TestHydraulicBoundaryDatabase()
        {
            Locations.Add(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0));
            Locations.Add(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0));
            Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0));
        }
    }
}