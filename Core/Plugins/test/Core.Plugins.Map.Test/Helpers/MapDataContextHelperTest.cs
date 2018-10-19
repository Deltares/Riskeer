// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Helpers;
using Core.Plugins.Map.PresentationObjects;
using NUnit.Framework;

namespace Core.Plugins.Map.Test.Helpers
{
    [TestFixture]
    public class MapDataContextHelperTest
    {
        [Test]
        public void GetParentsFromContext_ContextNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MapDataContextHelper.GetParentsFromContext(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("context", exception.ParamName);
        }

        [Test]
        public void GetParentsFromContext_ContextParentMapDataNull_ReturnsEmptyCollection()
        {
            // Setup
            var context = new MapDataCollectionContext(new MapDataCollection("test"), null);

            // Call
            IEnumerable<MapDataCollection> parents = MapDataContextHelper.GetParentsFromContext(context);

            // Assert
            CollectionAssert.IsEmpty(parents);
        }

        [Test]
        public void GetParentsFromContext_ContextWithParents_ReturnsCollectionWithAllParents()
        {
            // Setup
            var rootContext = new MapDataCollectionContext(new MapDataCollection("root"), null);
            var nestedContext1 = new MapDataCollectionContext(new MapDataCollection("nested1"), rootContext);
            var nestedContext2 = new MapDataCollectionContext(new MapDataCollection("test"), nestedContext1);

            // Call
            IEnumerable<MapDataCollection> parents = MapDataContextHelper.GetParentsFromContext(nestedContext2);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                nestedContext1.WrappedData,
                rootContext.WrappedData
            }, parents);
        }
    }
}