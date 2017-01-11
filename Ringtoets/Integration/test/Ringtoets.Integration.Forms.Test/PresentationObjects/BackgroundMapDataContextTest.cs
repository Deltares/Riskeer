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

using System;
using Core.Common.Controls.PresentationObjects;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class BackgroundMapDataContextTest
    {
        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Call
            var context = new BackgroundMapDataContext(mapData);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<WmtsMapData>>(context);
            Assert.AreSame(mapData, context.WrappedData);
        }

        [Test]
        public void Constructor_MapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new BackgroundMapDataContext(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }
    }
}