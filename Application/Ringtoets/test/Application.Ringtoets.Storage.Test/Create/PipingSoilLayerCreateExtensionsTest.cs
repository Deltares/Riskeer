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
using Application.Ringtoets.Storage.Create;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingSoilLayerCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(new Random(21).NextDouble());

            // Call
            TestDelegate test = () => soilLayer.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollector_ReturnsFailureMechanismEntityWithPropertiesSet(bool isAquifer)
        {
            // Setup
            double top = new Random(21).NextDouble();
            var soilLayer = new PipingSoilLayer(top)
            {
                IsAquifer = isAquifer
            };
            var collector = new PersistenceRegistry();

            // Call
            var entity = soilLayer.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(top), entity.Top);
            Assert.AreEqual(Convert.ToByte(isAquifer), entity.IsAquifer);
        } 
    }
}