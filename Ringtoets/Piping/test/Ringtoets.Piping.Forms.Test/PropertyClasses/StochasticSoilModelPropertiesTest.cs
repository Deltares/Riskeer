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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StochasticSoilModelPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new StochasticSoilModelProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StochasticSoilModel>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(1324, "Name", "SegmentName");
            stochasticSoilModel.Geometry.Add(new Point2D(1.0, 2.0));
            var pipingSoilProfile = new PipingSoilProfile("PipingSoilProfile", 0, new List<PipingSoilLayer>
            {
                new PipingSoilLayer(10)
            }, 0);
            var stochasticSoilProfile = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = pipingSoilProfile
            };
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            // Call
            var properties = new StochasticSoilModelProperties
            {
                Data = stochasticSoilModel
            };

            // Assert
            Assert.AreEqual(stochasticSoilModel.Name, properties.Name);
            Assert.AreEqual(stochasticSoilModel.SegmentName, properties.SegmentName);
            Assert.AreEqual(stochasticSoilModel.Id, properties.Id);
            Assert.AreEqual(stochasticSoilModel.Geometry[0], properties.Geometry[0]);

            Assert.IsInstanceOf<StochasticSoilProfileProperties[]>(properties.StochasticSoilProfiles);
            Assert.AreEqual(1, properties.StochasticSoilProfiles.Length);
        }
    }
}