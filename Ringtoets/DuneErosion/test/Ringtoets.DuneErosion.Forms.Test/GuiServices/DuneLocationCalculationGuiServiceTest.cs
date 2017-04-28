// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Forms.Test.GuiServices
{
    [TestFixture]
    public class DuneLocationCalculationGuiServiceTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        [Test]
        public void Constructor_ViewParentNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneLocationCalculationGuiService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("viewParent", exception.ParamName);
        }

        [Test]
        public void Calculate_LocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.Calculate(null,
                                                               failureMechanism,
                                                               "path",
                                                               1.0 / 30000);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("locations", exception.ParamName);
            }
        }

        [Test]
        public void Calculate_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.Calculate(Enumerable.Empty<DuneLocation>(),
                                                               null,
                                                               "path",
                                                               1.0 / 30000);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("failureMechanism", exception.ParamName);
            }
        }

        [Test]
        public void Calculate_ValidData_ScheduleAllLocations()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            failureMechanism.DuneLocations.AddRange(
                new[]
                {
                    new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    }),
                    new DuneLocation(1300002, "B", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    })
                });

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestHelper.AssertLogMessages(() => guiService.Calculate(failureMechanism.DuneLocations,
                                                                        failureMechanism,
                                                                        validFilePath,
                                                                        1.0 / 200),
                                             messages =>
                                             {
                                                 List<string> messageList = messages.ToList();

                                                 // Assert
                                                 Assert.AreEqual(10, messageList.Count);
                                                 StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[0]);
                                                 Assert.AreEqual("Duinafslag berekening voor locatie 'A' is niet geconvergeerd.", messageList[1]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", messageList[2]);
                                                 StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[3]);
                                                 StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[4]);
                                                 Assert.AreEqual("Duinafslag berekening voor locatie 'B' is niet geconvergeerd.", messageList[5]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", messageList[6]);
                                                 StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[7]);
                                                 Assert.AreEqual("Uitvoeren van 'A' is gelukt.", messageList[8]);
                                                 Assert.AreEqual("Uitvoeren van 'B' is gelukt.", messageList[9]);
                                             });
            }
        }
    }
}