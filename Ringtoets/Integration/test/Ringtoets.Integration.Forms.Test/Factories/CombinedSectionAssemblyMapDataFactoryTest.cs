// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Drawing;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.Integration.Forms.Factories;

namespace Ringtoets.Integration.Forms.Test.Factories
{
    [TestFixture]
    public class CombinedSectionAssemblyMapDataFactoryTest
    {
        [Test]
        public void CreateCombinedSectionAssemblyResultMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = CombinedSectionAssemblyMapDataFactory.CreateCombinedSectionAssemblyResultMapData();

            // Assert
            Assert.AreEqual("Gecombineerd vakoordeel", data.Name);
            
            Assert.IsTrue(data.IsVisible);
            CollectionAssert.IsEmpty(data.Features);

            LineStyle lineStyle = data.Style;
            Assert.AreEqual(Color.Empty, lineStyle.Color);
            Assert.AreEqual(6, lineStyle.Width);
            Assert.AreEqual(LineDashStyle.Solid, lineStyle.DashStyle);

            Assert.AreEqual("Vaknummer", data.SelectedMetaDataAttribute);
        }
    }
}