// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableWaternetFactoryTest
    {
        [Test]
        public void Create_DailyWaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetFactory.Create(null, new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]),
                                                             new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dailyWaternet", exception.ParamName);
        }
        [Test]
        public void Create_ExtremeWaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetFactory.Create(new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]),
                                                             null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("extremeWaternet", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]);

            // Call
            void Call() => PersistableWaternetFactory.Create(waternet, waternet, null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]);

            // Call
            void Call() => PersistableWaternetFactory.Create(waternet, waternet, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableWaternetCollection()
        {
            // Setup
            var phreaticLine1 = new MacroStabilityInwardsPhreaticLine("Phreatic Line", Enumerable.Empty<Point2D>());
            var waternetLine1 = new MacroStabilityInwardsWaternetLine("Waternet Line", Enumerable.Empty<Point2D>(), phreaticLine1);
            var phreaticLine2 = new MacroStabilityInwardsPhreaticLine("Phreatic Line", Enumerable.Empty<Point2D>());
            var waternetLine2 = new MacroStabilityInwardsWaternetLine("Waternet Line", Enumerable.Empty<Point2D>(), phreaticLine2);

            var dailyWaternet = new MacroStabilityInwardsWaternet(new[]
            {
                phreaticLine1
            }, new[]
            {
                waternetLine1
            });
            
            var extremeWaternet = new MacroStabilityInwardsWaternet(new[]
            {
                phreaticLine2
            }, new[]
            {
                waternetLine2
            });

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            IEnumerable<PersistableWaternet> persistableWaternets = PersistableWaternetFactory.Create(dailyWaternet, extremeWaternet, idFactory, registry);

            // Assert
            AssertWaternets(new[]
            {
                dailyWaternet,
                extremeWaternet
            }, persistableWaternets);

            var stages = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            Assert.AreEqual(2, registry.Waternets.Count);

            for (var i = 0; i < stages.Length; i++)
            {
                Assert.AreEqual(registry.Waternets[stages[i]], persistableWaternets.ElementAt(i).Id);
            }
        }

        private static void AssertWaternets(IEnumerable<MacroStabilityInwardsWaternet> originalWaternets, IEnumerable<PersistableWaternet> actualWaternets)
        {
            Assert.AreEqual(originalWaternets.Count(), actualWaternets.Count());

            for (var i = 0; i < originalWaternets.Count(); i++)
            {
                MacroStabilityInwardsWaternet originalWaternet = originalWaternets.ElementAt(i);
                PersistableWaternet actualWaternet = actualWaternets.ElementAt(i);

                Assert.IsNotNull(actualWaternet.Id);
                Assert.AreEqual(9.81, actualWaternet.UnitWeightWater);

                PersistableHeadLine firstHeadLine = actualWaternet.HeadLines.First();
                Assert.AreEqual(actualWaternet.PhreaticLineId, firstHeadLine.Id);

                Assert.AreEqual(originalWaternet.PhreaticLines.Count(), actualWaternet.HeadLines.Count());

                for (var j = 0; j < originalWaternet.PhreaticLines.Count(); j++)
                {
                    MacroStabilityInwardsPhreaticLine phreaticLine = originalWaternet.PhreaticLines.ElementAt(j);
                    PersistableHeadLine headLine = actualWaternet.HeadLines.ElementAt(j);

                    Assert.IsNotNull(headLine.Id);
                    Assert.AreEqual(phreaticLine.Name, headLine.Label);
                    CollectionAssert.AreEqual(phreaticLine.Geometry.Select(p => new PersistablePoint(p.X, p.Y)), headLine.Points);
                }

                Assert.AreEqual(originalWaternet.WaternetLines.Count(), actualWaternet.ReferenceLines.Count());

                for (var j = 0; j < originalWaternet.WaternetLines.Count(); j++)
                {
                    MacroStabilityInwardsWaternetLine waternetLine = originalWaternet.WaternetLines.ElementAt(j);
                    PersistableReferenceLine referenceLine = actualWaternet.ReferenceLines.ElementAt(j);

                    Assert.IsNotNull(referenceLine.Id);
                    Assert.AreEqual(waternetLine.Name, referenceLine.Label);
                    CollectionAssert.AreEqual(waternetLine.Geometry.Select(p => new PersistablePoint(p.X, p.Y)), referenceLine.Points);

                    Assert.AreEqual(firstHeadLine.Id, referenceLine.TopHeadLineId);
                    Assert.AreEqual(firstHeadLine.Id, referenceLine.BottomHeadLineId);
                }
            }
        }
    }
}